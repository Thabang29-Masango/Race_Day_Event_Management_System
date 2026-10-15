
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayManagementSystem.Data;
using RaceDayManagementSystem.Models;

namespace RaceDayManagementSystem.Controllers;

[ApiController]
[Route("api/participants")]
public class ParticipantsController : ControllerBase
{
    private readonly RaceDayContext _db;

    public ParticipantsController(RaceDayContext db) => _db = db;

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var item = await _db.Participants
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == id);

        return item == null ? NotFound() : Ok(item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Participant input)
    {
        var item = await _db.Participants.FindAsync(id);
        if (item == null) return NotFound();

        item.FirstName = input.FirstName;
        item.LastName = input.LastName;
        item.DateOfBirth = input.DateOfBirth;
        item.ProfilePictureUrl = input.ProfilePictureUrl;

        await _db.SaveChangesAsync();
        return Ok(item);
    }
}
