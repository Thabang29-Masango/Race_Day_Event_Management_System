
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayManagementSystem.Data;
using RaceDayManagementSystem.Models;

namespace RaceDayManagementSystem.Controllers;

[ApiController]
[Route("api")]
public class CategoriesController : ControllerBase
{
    private readonly RaceDayContext _db;

    public CategoriesController(RaceDayContext db) => _db = db;

    [HttpGet("events/{eventId:int}/categories")]
    public async Task<IActionResult> GetByEvent(int eventId)
    {
        if (!await _db.Events.AnyAsync(x => x.EventId == eventId))
            return NotFound();

        return Ok(await _db.Categories
            .AsNoTracking()
            .Where(x => x.EventId == eventId)
            .ToListAsync());
    }

    [HttpGet("categories/{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var item = await _db.Categories.FindAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost("events/{eventId:int}/categories")]
    public async Task<IActionResult> Create(int eventId, Category input)
    {
        if (!await _db.Events.AnyAsync(x => x.EventId == eventId))
            return NotFound("Event does not exist.");

        if (input.MinAge < 0 ||
            input.MaxAge < input.MinAge ||
            input.DistanceKm <= 0)
            return BadRequest("Check the age range and distance.");

        input.CategoryId = 0;
        input.EventId = eventId;
        _db.Categories.Add(input);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get),
            new { id = input.CategoryId }, input);
    }

    [HttpPut("categories/{id:int}")]
    public async Task<IActionResult> Update(int id, Category input)
    {
        var item = await _db.Categories.FindAsync(id);
        if (item == null) return NotFound();

        if (input.MinAge < 0 ||
            input.MaxAge < input.MinAge ||
            input.DistanceKm <= 0)
            return BadRequest("Check the age range and distance.");

        item.Name = input.Name;
        item.MinAge = input.MinAge;
        item.MaxAge = input.MaxAge;
        item.DistanceKm = input.DistanceKm;

        await _db.SaveChangesAsync();
        return Ok(item);
    }

    [HttpDelete("categories/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.Categories.FindAsync(id);
        if (item == null) return NotFound();

        _db.Categories.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
