
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayManagementSystem.Data;
using RaceDayManagementSystem.Models;

namespace RaceDayManagementSystem.Controllers;

[ApiController]
[Route("api/organisers")]
public class OrganisersController : ControllerBase
{
    private readonly RaceDayContext _db;

    public OrganisersController(RaceDayContext db) => _db = db;

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var item = await _db.Organisers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == id);

        return item == null ? NotFound() : Ok(item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Organiser input)
    {
        var item = await _db.Organisers.FindAsync(id);
        if (item == null) return NotFound();

        item.FirstName = input.FirstName;
        item.LastName = input.LastName;
        item.PhoneNumber = input.PhoneNumber;
        item.OrganisationName = input.OrganisationName;

        await _db.SaveChangesAsync();
        return Ok(item);
    }
}
