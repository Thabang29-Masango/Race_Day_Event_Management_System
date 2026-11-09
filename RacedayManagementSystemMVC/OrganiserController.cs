using Microsoft.AspNetCore.Mvc;
using RaceDayManagementSystemMVC.Filters;
using RaceDayManagementSystemMVC.Helpers;
using RaceDayManagementSystemMVC.Models;
using RaceDayManagementSystemMVC.Services;
using RaceDayManagementSystemMVC.Controllers;

using System.Globalization;

namespace RaceDayManagementSystemMVC.Controllers;

[RequireRole("Organiser")]
public class OrganiserController : BaseController
{
    public OrganiserController(RaceDayApiService api) : base(api) { }

    // ------------------------------------------------------------ helpers
    /// <summary>Loads an event only if it belongs to the logged-in organiser.</summary>
    private async Task<EventDto?> GetOwnedEventAsync(int id)
    {
        var result = await Api.GetEventAsync(id);
        if (!result.Success || result.Data == null || result.Data.OrganiserId != CurrentUserId)
            return null;
        return result.Data;
    }

    private IActionResult EventNotFound()
    {
        TempData["Error"] = "That event could not be found.";
        return RedirectToAction(nameof(Events));
    }

    private async Task<List<EventDto>?> GetMyEventsAsync()
    {
        var result = await Api.GetEventsAsync();
        if (!result.Success || result.Data == null) return null;
        return result.Data.Where(e => e.OrganiserId == CurrentUserId)
            .OrderBy(e => e.EventDate).ToList();
    }

    // ------------------------------------------------------------ dashboard
    public async Task<IActionResult> Dashboard()
    {
        var mine = await GetMyEventsAsync();
        if (mine == null)
        {
            ViewBag.LoadError = "We could not load your events right now.";
            return View(new DashboardViewModel());
        }

        var counts = await Task.WhenAll(mine.Select(async e =>
        {
            var r = await Api.GetEventEnrolmentsAsync(e.EventId);
            return r.Success && r.Data != null
                ? r.Data.Count(x => !x.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
                : 0;
        }));

        var model = new DashboardViewModel
        {
            Events = mine.Select((e, i) => new EventSummary { Event = e, Enrolments = counts[i] }).ToList()
        };
        return View(model);
    }

    // ------------------------------------------------------------ events CRUD
    public async Task<IActionResult> Events()
    {
        var mine = await GetMyEventsAsync();
        if (mine == null)
        {
            ViewBag.LoadError = "We could not load your events right now.";
            return View(new List<EventDto>());
        }
        return View(mine);
    }

    [HttpGet]
    public IActionResult CreateEvent() =>
        View(new EventFormViewModel { EventDate = DateTime.Today.AddDays(30) });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEvent(EventFormViewModel model)
    {
        ValidateBanner(model);
        if (!ModelState.IsValid) return View(model);

        var created = await Api.CreateEventAsync(ToDto(model));

        var expired = RedirectIfSessionExpired(created);
        if (expired != null) return expired;

        if (!created.Success || created.Data == null)
        {
            ModelState.AddModelError(string.Empty, created.Error);
            return View(model);
        }

        if (model.Banner != null && model.Banner.Length > 0)
        {
            var upload = await Api.UploadBannerAsync(created.Data.EventId, model.Banner);
            if (!upload.Success)
            {
                TempData["Error"] = $"The event was created, but the banner could not be uploaded. {upload.Error}";
                return RedirectToAction(nameof(EditEvent), new { id = created.Data.EventId });
            }
        }

        TempData["Success"] = "Event created. Add categories so participants can enrol.";
        return RedirectToAction(nameof(Categories), new { eventId = created.Data.EventId });
    }

    [HttpGet]
    public async Task<IActionResult> EditEvent(int id)
    {
        var ev = await GetOwnedEventAsync(id);
        if (ev == null) return EventNotFound();

        return View(new EventFormViewModel
        {
            EventId = ev.EventId,
            Name = ev.Name,
            Description = ev.Description,
            EventDate = ev.EventDate,
            Location = ev.Location,
            DistanceKm = ev.DistanceKm,
            EventType = ev.EventType,
            BannerImageUrl = ev.BannerImageUrl
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditEvent(int id, EventFormViewModel model)
    {
        if (id != model.EventId) return BadRequest();

        var existing = await GetOwnedEventAsync(id);
        if (existing == null) return EventNotFound();

        ValidateBanner(model);
        if (!ModelState.IsValid)
        {
            model.BannerImageUrl = existing.BannerImageUrl;
            return View(model);
        }

        // PUT first (keeping the current banner URL), then replace the banner if one was chosen.
        model.BannerImageUrl = existing.BannerImageUrl;
        var updated = await Api.UpdateEventAsync(id, ToDto(model));

        var expired = RedirectIfSessionExpired(updated);
        if (expired != null) return expired;

        if (!updated.Success)
        {
            ModelState.AddModelError(string.Empty, updated.Error);
            return View(model);
        }

        if (model.Banner != null && model.Banner.Length > 0)
        {
            var upload = await Api.UploadBannerAsync(id, model.Banner);
            if (!upload.Success)
            {
                TempData["Error"] = $"Your changes were saved, but the banner could not be uploaded. {upload.Error}";
                return RedirectToAction(nameof(EditEvent), new { id });
            }
        }

        TempData["Success"] = "Event updated.";
        return RedirectToAction(nameof(Events));
    }

    [HttpGet]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var ev = await GetOwnedEventAsync(id);
        return ev == null ? EventNotFound() : View(ev);
    }

    [HttpPost, ActionName("DeleteEvent")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteEventConfirmed(int id)
    {
        var ev = await GetOwnedEventAsync(id);
        if (ev == null) return EventNotFound();

        var result = await Api.DeleteEventAsync(id);
        if (result.Success)
            TempData["Success"] = "Event deleted.";
        else
            TempData["Error"] = result.StatusCode >= 500
                ? "This event could not be deleted. It may still have categories or enrolments."
                : result.Error;

        return RedirectToAction(nameof(Events));
    }

    private void ValidateBanner(EventFormViewModel model)
    {
        if (model.Banner != null && model.Banner.Length > 0 &&
            !IsAllowedImage(model.Banner, out var error))
            ModelState.AddModelError(nameof(model.Banner), error);
    }

    private EventDto ToDto(EventFormViewModel m) => new()
    {
        EventId = m.EventId,
        OrganiserId = CurrentUserId,
        Name = m.Name.Trim(),
        Description = string.IsNullOrWhiteSpace(m.Description) ? null : m.Description.Trim(),
        EventDate = m.EventDate!.Value.Date,
        Location = m.Location.Trim(),
        DistanceKm = m.DistanceKm,
        EventType = m.EventType,
        BannerImageUrl = m.BannerImageUrl
    };

    // ------------------------------------------------------------ categories
    [HttpGet]
    public async Task<IActionResult> Categories(int eventId)
    {
        var ev = await GetOwnedEventAsync(eventId);
        if (ev == null) return EventNotFound();

        return View(await BuildCategoriesModelAsync(ev, new CategoryDto { DistanceKm = ev.DistanceKm }));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCategory(int eventId, [Bind(Prefix = "NewCategory")] CategoryDto newCategory)
    {
        var ev = await GetOwnedEventAsync(eventId);
        if (ev == null) return EventNotFound();

        if (!ModelState.IsValid)
            return View(nameof(Categories), await BuildCategoriesModelAsync(ev, newCategory));

        newCategory.Name = newCategory.Name.Trim();
        var result = await Api.CreateCategoryAsync(eventId, newCategory);

        var expired = RedirectIfSessionExpired(result);
        if (expired != null) return expired;

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error);
            return View(nameof(Categories), await BuildCategoriesModelAsync(ev, newCategory));
        }

        TempData["Success"] = "Category added.";
        return RedirectToAction(nameof(Categories), new { eventId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int eventId, int categoryId)
    {
        var ev = await GetOwnedEventAsync(eventId);
        if (ev == null) return EventNotFound();

        var result = await Api.DeleteCategoryAsync(categoryId);
        if (result.Success)
            TempData["Success"] = "Category removed.";
        else
            TempData["Error"] = result.StatusCode >= 500
                ? "This category could not be removed. Participants may already be enrolled in it."
                : result.Error;

        return RedirectToAction(nameof(Categories), new { eventId });
    }

    private async Task<CategoriesViewModel> BuildCategoriesModelAsync(EventDto ev, CategoryDto newCategory)
    {
        var categories = await Api.GetCategoriesAsync(ev.EventId);
        if (!categories.Success) ViewBag.LoadError = categories.Error;

        return new CategoriesViewModel
        {
            Event = ev,
            Categories = categories.Data?.OrderBy(c => c.DistanceKm).ThenBy(c => c.MinAge).ToList() ?? new(),
            NewCategory = newCategory
        };
    }

    // ------------------------------------------------------------ enrolments
    public async Task<IActionResult> Enrolments(int eventId)
    {
        var ev = await GetOwnedEventAsync(eventId);
        if (ev == null) return EventNotFound();

        var result = await Api.GetEventEnrolmentsAsync(eventId);

        var expired = RedirectIfSessionExpired(result);
        if (expired != null) return expired;

        if (!result.Success) ViewBag.LoadError = result.Error;

        return View(new EventEnrolmentsViewModel
        {
            Event = ev,
            Enrolments = result.Data?.OrderBy(e => e.ParticipantName).ToList() ?? new()
        });
    }

    // ------------------------------------------------------------ results
    [HttpGet]
    public async Task<IActionResult> Results(int eventId)
    {
        var ev = await GetOwnedEventAsync(eventId);
        if (ev == null) return EventNotFound();

        var result = await Api.GetEventEnrolmentsAsync(eventId);

        var expired = RedirectIfSessionExpired(result);
        if (expired != null) return expired;

        if (!result.Success) ViewBag.LoadError = result.Error;

        var active = (result.Data ?? new())
            .Where(e => !e.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
            .OrderBy(e => e.ParticipantName)
            .ToList();

        return View(new ResultsEntryViewModel
        {
            Event = ev,
            TotalFinishers = active.FirstOrDefault(e => e.TotalFinishers != null)?.TotalFinishers,
            Rows = active.Select(e => new ResultRow
            {
                EnrolmentId = e.EnrolmentId,
                ParticipantName = e.ParticipantName,
                CategoryName = e.CategoryName,
                ResultId = e.ResultId,
                FinishTime = e.FinishTime == null ? null : Formatting.Time(e.FinishTime),
                FinishPosition = e.FinishPosition
            }).ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Results(int eventId, ResultsEntryViewModel model)
    {
        var ev = await GetOwnedEventAsync(eventId);
        if (ev == null) return EventNotFound();

        if (ev.EventDate.Date > DateTime.Today)
        {
            TempData["Error"] = "Results can only be captured once the event has taken place.";
            return RedirectToAction(nameof(Enrolments), new { eventId });
        }

        var enrolments = await Api.GetEventEnrolmentsAsync(eventId);

        var expired = RedirectIfSessionExpired(enrolments);
        if (expired != null) return expired;

        if (!enrolments.Success || enrolments.Data == null)
        {
            TempData["Error"] = enrolments.Error;
            return RedirectToAction(nameof(Enrolments), new { eventId });
        }

        // Trust only what the API says belongs to this event, not the posted hidden fields.
        var known = enrolments.Data
            .Where(e => !e.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(e => e.EnrolmentId);

        model.Event = ev;
        model.Rows ??= new();

        var entries = new List<(int EnrolmentId, int? ResultId, TimeSpan Time, int Position)>();

        for (var i = 0; i < model.Rows.Count; i++)
        {
            var row = model.Rows[i];
            if (!known.TryGetValue(row.EnrolmentId, out var enrolment)) continue;

            row.ParticipantName = enrolment.ParticipantName;
            row.CategoryName = enrolment.CategoryName;
            row.ResultId = enrolment.ResultId;

            var hasTime = !string.IsNullOrWhiteSpace(row.FinishTime);
            var hasPosition = row.FinishPosition != null;
            if (!hasTime && !hasPosition) continue;

            if (!hasTime || !hasPosition)
            {
                ModelState.AddModelError($"Rows[{i}].FinishTime",
                    $"Enter both a finish time and a position for {row.ParticipantName}.");
                continue;
            }

            if (!TimeSpan.TryParseExact(row.FinishTime!.Trim(), @"h\:mm\:ss",
                    CultureInfo.InvariantCulture, out var time) || time <= TimeSpan.Zero)
            {
                ModelState.AddModelError($"Rows[{i}].FinishTime",
                    $"Use hh:mm:ss for {row.ParticipantName} (for example 01:23:45).");
                continue;
            }

            if (row.FinishPosition < 1)
            {
                ModelState.AddModelError($"Rows[{i}].FinishPosition", "Position must be 1 or higher.");
                continue;
            }

            entries.Add((row.EnrolmentId, enrolment.ResultId, time, row.FinishPosition!.Value));
        }

        if (entries.Count > 0)
        {
            if (model.TotalFinishers == null || model.TotalFinishers < 1)
                ModelState.AddModelError(nameof(model.TotalFinishers), "Enter the total number of finishers.");
            else if (entries.Any(e => e.Position > model.TotalFinishers))
                ModelState.AddModelError(nameof(model.TotalFinishers),
                    "Total finishers cannot be lower than a finishing position.");
        }
        else if (ModelState.IsValid)
        {
            ModelState.AddModelError(string.Empty, "Enter at least one result before saving.");
        }

        if (!ModelState.IsValid) return View(model);

        var failures = 0;
        foreach (var e in entries)
        {
            var save = e.ResultId == null
                ? await Api.CreateResultAsync(e.EnrolmentId, e.Time, e.Position, model.TotalFinishers!.Value)
                : await Api.UpdateResultAsync(e.ResultId.Value, e.Time, e.Position, model.TotalFinishers!.Value);

            if (!save.Success) failures++;
        }

        if (failures == 0)
            TempData["Success"] = $"{entries.Count} result(s) saved and published.";
        else
            TempData["Error"] = $"{failures} of {entries.Count} result(s) could not be saved. Please try again.";

        return RedirectToAction(nameof(Results), new { eventId });
    }
}
