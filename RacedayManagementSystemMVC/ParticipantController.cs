using Microsoft.AspNetCore.Mvc;
using RaceDayManagementSystemMVC.Filters;
using RaceDayManagementSystemMVC.Helpers;
using RaceDayManagementSystemMVC.Models;
using RaceDayManagementSystemMVC.Services;
using RaceDayManagementSystemMVC.Controllers;


namespace RaceDayManagementSystemMVC.Controllers;

[RequireRole("Participant")]
public class ParticipantController : BaseController
{
    public ParticipantController(RaceDayApiService api) : base(api) { }

    public async Task<IActionResult> MyEnrolments()
    {
        var result = await Api.GetParticipantEnrolmentsAsync(CurrentUserId);

        var expired = RedirectIfSessionExpired(result);
        if (expired != null) return expired;

        if (!result.Success)
        {
            ViewBag.LoadError = result.Error;
            return View(new List<EnrolmentDetailDto>());
        }

        return View(result.Data!.OrderBy(e => e.EventDate).ToList());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelEnrolment(int id)
    {
        // Only allow cancelling the participant's own enrolment.
        var mine = await Api.GetParticipantEnrolmentsAsync(CurrentUserId);
        if (!mine.Success || mine.Data == null || mine.Data.All(e => e.EnrolmentId != id))
        {
            TempData["Error"] = "That enrolment could not be found.";
            return RedirectToAction(nameof(MyEnrolments));
        }

        var result = await Api.CancelEnrolmentAsync(id);
        TempData[result.Success ? "Success" : "Error"] =
            result.Success ? "Your enrolment has been cancelled." : result.Error;

        return RedirectToAction(nameof(MyEnrolments));
    }

    public async Task<IActionResult> MyResults()
    {
        var result = await Api.GetParticipantEnrolmentsAsync(CurrentUserId);

        var expired = RedirectIfSessionExpired(result);
        if (expired != null) return expired;

        if (!result.Success)
        {
            ViewBag.LoadError = result.Error;
            return View(new List<EnrolmentDetailDto>());
        }

        var completed = result.Data!
            .Where(e => e.FinishTime != null && e.FinishPosition != null &&
                        !e.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(e => e.EventDate)
            .ToList();

        return View(completed);
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var result = await Api.GetParticipantAsync(CurrentUserId);

        var expired = RedirectIfSessionExpired(result);
        if (expired != null) return expired;

        if (!result.Success || result.Data == null)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction("Index", "Home");
        }

        var p = result.Data;
        return View(new ProfileViewModel
        {
            UserId = p.UserId,
            FirstName = p.FirstName,
            LastName = p.LastName,
            DateOfBirth = p.DateOfBirth,
            ProfilePictureUrl = p.ProfilePictureUrl
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        if (model.DateOfBirth > DateTime.Today || model.DateOfBirth < DateTime.Today.AddYears(-120))
            ModelState.AddModelError(nameof(model.DateOfBirth), "Enter a valid date of birth.");

        if (model.Picture != null && model.Picture.Length > 0 &&
            !IsAllowedImage(model.Picture, out var imageError))
            ModelState.AddModelError(nameof(model.Picture), imageError);

        if (!ModelState.IsValid) return View(model);

        // PUT first (keeps the current picture URL), then upload the new picture.
        var update = await Api.UpdateParticipantAsync(CurrentUserId, new ParticipantDto
        {
            UserId = CurrentUserId,
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            DateOfBirth = model.DateOfBirth,
            ProfilePictureUrl = model.ProfilePictureUrl
        });

        var expired = RedirectIfSessionExpired(update);
        if (expired != null) return expired;

        if (!update.Success)
        {
            ModelState.AddModelError(string.Empty, update.Error);
            return View(model);
        }

        HttpContext.Session.SetString(SessionKeys.FullName, $"{model.FirstName.Trim()} {model.LastName.Trim()}");

        if (model.Picture != null && model.Picture.Length > 0)
        {
            var upload = await Api.UploadProfilePictureAsync(CurrentUserId, model.Picture);
            if (!upload.Success)
            {
                TempData["Error"] = $"Your details were saved, but the picture could not be uploaded. {upload.Error}";
                return RedirectToAction(nameof(Profile));
            }
        }

        TempData["Success"] = "Your profile has been updated.";
        return RedirectToAction(nameof(Profile));
    }
}