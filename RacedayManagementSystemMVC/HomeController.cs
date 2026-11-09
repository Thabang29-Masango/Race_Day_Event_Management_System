using Microsoft.AspNetCore.Mvc;
using RaceDayManagementSystemMVC.Models;
using RaceDayManagementSystemMVC.Services;
using RaceDayManagementSystemMVC.Controllers;


namespace RaceDayManagementSystemMVC.Controllers;

public class HomeController : BaseController
{
    public HomeController(RaceDayApiService api) : base(api) { }

    public async Task<IActionResult> Index(
        string? search, string? eventType, string? location,
        DateTime? fromDate, DateTime? toDate, decimal? maxDistance)
    {
        var model = new HomeViewModel
        {
            Search = search,
            EventType = eventType,
            Location = location,
            From = fromDate,
            To = toDate,
            MaxDistance = maxDistance
        };

        var result = await Api.GetEventsAsync();
        if (!result.Success || result.Data == null)
        {
            ViewBag.LoadError = result.Error;
            return View(model);
        }

        var upcoming = result.Data
            .Where(e => e.EventDate.Date >= DateTime.Today)
            .ToList();

        model.EventTypes = upcoming
            .Select(e => e.EventType)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(t => t)
            .ToList();

        IEnumerable<EventDto> filtered = upcoming;

        if (!string.IsNullOrWhiteSpace(search))
            filtered = filtered.Where(e =>
                e.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (e.Description ?? string.Empty).Contains(search, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(eventType))
            filtered = filtered.Where(e =>
                string.Equals(e.EventType, eventType, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(location))
            filtered = filtered.Where(e =>
                e.Location.Contains(location, StringComparison.OrdinalIgnoreCase));

        if (fromDate != null)
            filtered = filtered.Where(e => e.EventDate.Date >= fromDate.Value.Date);

        if (toDate != null)
            filtered = filtered.Where(e => e.EventDate.Date <= toDate.Value.Date);

        if (maxDistance != null)
            filtered = filtered.Where(e => e.DistanceKm <= maxDistance.Value);

        model.Events = filtered.OrderBy(e => e.EventDate).ToList();
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}