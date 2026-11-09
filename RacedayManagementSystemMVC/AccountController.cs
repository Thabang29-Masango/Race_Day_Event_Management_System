using Microsoft.AspNetCore.Mvc;
using RaceDayManagementSystemMVC.Helpers;
using RaceDayManagementSystemMVC.Models;
using RaceDayManagementSystemMVC.Services;

namespace RaceDayManagementSystemMVC.Controllers;

public class AccountController : BaseController
{
    public AccountController(RaceDayApiService api) : base(api) { }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (HttpContext.Session.IsLoggedIn())
            return RedirectToRoleHome();

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await Api.LoginAsync(model.Email.Trim(), model.Password);
        if (!result.Success || result.Data == null)
        {
            ModelState.AddModelError(string.Empty,
                result.StatusCode == 401
                    ? "Incorrect email or password."
                    : result.Error);
            return View(model);
        }

        // Store the token first so the follow-up profile call is authenticated.
        HttpContext.Session.SignIn(result.Data);
        await LoadDisplayNameAsync(result.Data);

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return LocalRedirect(model.ReturnUrl);

        return RedirectToRoleHome();
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (HttpContext.Session.IsLoggedIn())
            return RedirectToRoleHome();

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await Api.RegisterAsync(model);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error);
            return View(model);
        }

        TempData["Success"] = "Your account is ready. Please log in.";
        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        TempData["Success"] = "You have been logged out.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult AccessDenied() => View();

    private async Task LoadDisplayNameAsync(LoginResponse login)
    {
        string? name = null;

        if (login.Role == "Organiser")
        {
            var org = await Api.GetOrganiserAsync(login.UserId);
            if (org.Success && org.Data != null)
                name = $"{org.Data.FirstName} {org.Data.LastName}";
        }
        else
        {
            var p = await Api.GetParticipantAsync(login.UserId);
            if (p.Success && p.Data != null)
                name = $"{p.Data.FirstName} {p.Data.LastName}";
        }

        if (!string.IsNullOrWhiteSpace(name))
            HttpContext.Session.SetString(SessionKeys.FullName, name);
    }

    private IActionResult RedirectToRoleHome() =>
        HttpContext.Session.GetRole() == "Organiser"
            ? RedirectToAction("Dashboard", "Organiser")
            : RedirectToAction("Index", "Home");
}