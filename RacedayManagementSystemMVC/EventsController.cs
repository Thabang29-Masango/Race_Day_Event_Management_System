using Microsoft.AspNetCore.Mvc;
using RaceDayManagementSystemMVC.Filters;
using RaceDayManagementSystemMVC.Helpers;
using RaceDayManagementSystemMVC.Models;
using RaceDayManagementSystemMVC.Services;
using RaceDayManagementSystemMVC.Controllers;


namespace RaceDayManagementSystemMVC.Controllers;

public class EventsController : BaseController
{
    public EventsController(RaceDayApiService api) : base(api) { }

    // Public: anyone can read event information. Enrolling needs a Participant login.
    public async Task<IActionResult> Details(int id)
    {
        var eventResult = await Api.GetEventAsync(id);
        if (!eventResult.Success || eventResult.Data == null)
        {
            TempData["Error"] = eventResult.StatusCode == 404
                ? "That event could not be found."
                : eventResult.Error;
            return RedirectToAction("Index", "Home");
        }

        var model = new EventDetailsViewModel { Event = eventResult.Data };

        var categories = await Api.GetCategoriesAsync(id);
        if (categories.Success && categories.Data != null)
            model.Categories = categories.Data.OrderBy(c => c.DistanceKm).ThenBy(c => c.MinAge).ToList();

        if (HttpContext.Session.IsLoggedIn() && HttpContext.Session.GetRole() == "Participant")
        {
            var mine = await Api.GetParticipantEnrolmentsAsync(CurrentUserId);
            if (mine.Success && mine.Data != null)
                model.AlreadyEnrolled = mine.Data.Any(e =>
                    e.EventId == id && !e.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase));
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequireRole("Participant")]
    public async Task<IActionResult> Enrol(int eventId, int categoryId)
    {
        if (categoryId <= 0)
        {
            TempData["Error"] = "Please choose a category before enrolling.";
            return RedirectToAction(nameof(Details), new { id = eventId });
        }

        var ev = await Api.GetEventAsync(eventId);
        if (!ev.Success || ev.Data == null)
        {
            TempData["Error"] = "That event could not be found.";
            return RedirectToAction("Index", "Home");
        }

        if (ev.Data.EventDate.Date < DateTime.Today)
        {
            TempData["Error"] = "This event has already taken place.";
            return RedirectToAction(nameof(Details), new { id = eventId });
        }

        var result = await Api.EnrolAsync(CurrentUserId, eventId, categoryId);

        var expired = RedirectIfSessionExpired(result);
        if (expired != null) return expired;

        if (!result.Success)
        {
            TempData["Error"] = result.StatusCode == 409
                ? "You are already enrolled for this event."
                : result.Error;
            return RedirectToAction(nameof(Details), new { id = eventId });
        }

        TempData["Success"] = $"You are enrolled for {ev.Data.Name}. Good luck!";
        return RedirectToAction("MyEnrolments", "Participant");
    }
}