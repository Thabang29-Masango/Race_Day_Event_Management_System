
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayManagementSystem.Data;
using RaceDayManagementSystem.Models;

namespace RaceDayManagementSystem.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly RaceDayContext _db;

    public EventsController(RaceDayContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _db.Events.AsNoTracking().ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var item = await _db.Events.FindAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Event item)
    {
        if (item.DistanceKm <= 0)
            return BadRequest("Distance must be greater than zero.");

        if (!await _db.Organisers.AnyAsync(
                x => x.UserId == item.OrganiserId))
            return BadRequest("Organiser does not exist.");

        item.EventId = 0;
        _db.Events.Add(item);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get),
            new { id = item.EventId }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Event input)
    {
        var item = await _db.Events.FindAsync(id);
        if (item == null) return NotFound();

        if (input.DistanceKm <= 0)
            return BadRequest("Distance must be greater than zero.");

        item.Name = input.Name;
        item.Description = input.Description;
        item.EventDate = input.EventDate;
        item.Location = input.Location;
        item.DistanceKm = input.DistanceKm;
        item.EventType = input.EventType;
        item.BannerImageUrl = input.BannerImageUrl;

        await _db.SaveChangesAsync();
        return Ok(item);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.Events.FindAsync(id);
        if (item == null) return NotFound();

        _db.Events.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
