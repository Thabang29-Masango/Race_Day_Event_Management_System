
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayManagementSystem.Data;
using RaceDayManagementSystem.Models;

namespace RaceDayManagementSystem.Controllers;

[ApiController]
[Route("api/enrolments")]
public class EnrolmentsController : ControllerBase
{
    private readonly RaceDayContext _db;

    public EnrolmentsController(RaceDayContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _db.Enrolments.AsNoTracking().ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var item = await _db.Enrolments.FindAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Enrolment input)
    {
        if (!await _db.Participants.AnyAsync(
                x => x.UserId == input.ParticipantId))
            return BadRequest("Participant does not exist.");

        if (!await _db.Events.AnyAsync(
                x => x.EventId == input.EventId))
            return BadRequest("Event does not exist.");

        if (!await _db.Categories.AnyAsync(
                x => x.CategoryId == input.CategoryId &&
                     x.EventId == input.EventId))
            return BadRequest("Category does not belong to this event.");

        if (await _db.Enrolments.AnyAsync(
                x => x.ParticipantId == input.ParticipantId &&
                     x.EventId == input.EventId))
            return Conflict("Participant is already enrolled.");

        input.EnrolmentId = 0;
        input.EnrolmentDate = DateTime.Now;
        input.Status = "Active";

        _db.Enrolments.Add(input);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get),
            new { id = input.EnrolmentId }, input);
    }

    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var item = await _db.Enrolments.FindAsync(id);
        if (item == null) return NotFound();

        item.Status = "Cancelled";
        await _db.SaveChangesAsync();

        return Ok(item);
    }
}
