
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayManagementSystem.Data;
using RaceDayManagementSystem.Models;

namespace RaceDayManagementSystem.Controllers;

[ApiController]
[Route("api/results")]
public class ResultsController : ControllerBase
{
    private readonly RaceDayContext _db;

    public ResultsController(RaceDayContext db) => _db = db;

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var item = await _db.Results.FindAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Result input)
    {
        if (!await _db.Enrolments.AnyAsync(
                x => x.EnrolmentId == input.EnrolmentId))
            return BadRequest("Enrolment does not exist.");

        if (await _db.Results.AnyAsync(
                x => x.EnrolmentId == input.EnrolmentId))
            return Conflict("A result already exists for this enrolment.");

        if (input.FinishPosition <= 0 || input.TotalFinishers <= 0)
            return BadRequest("Positions and total finishers must be positive.");

        input.ResultId = 0;
        input.CapturedDate = DateTime.Now;

        _db.Results.Add(input);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get),
            new { id = input.ResultId }, input);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Result input)
    {
        var item = await _db.Results.FindAsync(id);
        if (item == null) return NotFound();

        if (input.FinishPosition <= 0 || input.TotalFinishers <= 0)
            return BadRequest("Positions and total finishers must be positive.");

        item.FinishTime = input.FinishTime;
        item.FinishPosition = input.FinishPosition;
        item.TotalFinishers = input.TotalFinishers;

        await _db.SaveChangesAsync();
        return Ok(item);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.Results.FindAsync(id);
        if (item == null) return NotFound();

        _db.Results.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
