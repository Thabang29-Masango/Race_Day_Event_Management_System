using Microsoft.AspNetCore.Mvc;
using RaceDayManagementSystemMVC.Helpers;
using RaceDayManagementSystemMVC.Services;

namespace RaceDayManagementSystemMVC.Controllers;

public abstract class BaseController : Controller
{
    protected readonly RaceDayApiService Api;

    protected BaseController(RaceDayApiService api) => Api = api;

    protected int CurrentUserId => HttpContext.Session.GetUserId();

    /// <summary>
    /// If the API says our token has expired (401), end the session and send the
    /// user back to the login page. Returns null when the result is fine.
    /// </summary>
    protected IActionResult? RedirectIfSessionExpired(ApiResult result)
    {
        if (!result.Unauthorized) return null;

        HttpContext.Session.Clear();
        TempData["Error"] = "Your session has expired. Please log in again.";
        return RedirectToAction("Login", "Account");
    }

    protected static bool IsAllowedImage(IFormFile file, out string error)
    {
        var allowed = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
        error = string.Empty;

        if (file.Length > 5 * 1024 * 1024)
            error = "Image must be 5 MB or smaller.";
        else if (!allowed.Contains(file.ContentType.ToLowerInvariant()))
            error = "Only JPG, PNG, WEBP or GIF images are allowed.";

        return error.Length == 0;
    }
}