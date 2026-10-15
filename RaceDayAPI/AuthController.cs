
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RaceDayManagementSystem.Data;
using RaceDayManagementSystem.DTOs;
using RaceDayManagementSystem.Models;

namespace RaceDayManagementSystem.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly RaceDayContext _db;
    private readonly IConfiguration _configuration;

    public AuthController(
        RaceDayContext db,
        IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var role = request.Role.Trim();

        if (role != "Organiser" && role != "Participant")
            return BadRequest("Role must be Organiser or Participant.");

        if (await _db.Users.AnyAsync(x => x.Email == request.Email))
            return Conflict("Email is already registered.");

        if (string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.LastName))
            return BadRequest("First and last name are required.");

        if (role == "Organiser" &&
            (string.IsNullOrWhiteSpace(request.PhoneNumber) ||
             string.IsNullOrWhiteSpace(request.OrganisationName)))
            return BadRequest(
                "Phone number and organisation name are required.");

        if (role == "Participant" && request.DateOfBirth == null)
            return BadRequest("Date of birth is required.");

        var user = new User
        {
            Email = request.Email.Trim(),
            Role = role,
            DateCreated = DateTime.UtcNow
        };

        user.PasswordHash =
            new PasswordHasher<User>().HashPassword(user, request.Password);

        await using var transaction =
            await _db.Database.BeginTransactionAsync();

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        if (role == "Organiser")
        {
            _db.Organisers.Add(new Organiser
            {
                UserId = user.UserId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber!,
                OrganisationName = request.OrganisationName!
            });
        }
        else
        {
            _db.Participants.Add(new Participant
            {
                UserId = user.UserId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth!.Value
            });
        }

        await _db.SaveChangesAsync();
        await transaction.CommitAsync();

        return StatusCode(201, new
        {
            user.UserId,
            user.Email,
            user.Role
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _db.Users
            .SingleOrDefaultAsync(x => x.Email == request.Email);

        if (user == null)
            return Unauthorized("Invalid email or password.");

        var result = new PasswordHasher<User>()
            .VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed)
            return Unauthorized("Invalid email or password.");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var keyText = _configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(keyText) || keyText.Length < 32)
            return StatusCode(500, "Configure a secure JWT key.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyText));

        var credentials = new SigningCredentials(
            key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            user.UserId,
            user.Email,
            user.Role
        });
    }
}
