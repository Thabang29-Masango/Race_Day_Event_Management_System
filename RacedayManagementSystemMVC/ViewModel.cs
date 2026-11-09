using Microsoft.AspNetCore.Mvc.Rendering;
using RaceDayManagementSystemMVC.Models;
using System.ComponentModel.DataAnnotations;

namespace RaceDayManagementSystemMVC.Models;

// ------------------------------------------------------------------ account
public class LoginViewModel
{
    [Required(ErrorMessage = "Enter your email address."), EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enter your password."), DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}

public class RegisterViewModel : IValidatableObject
{
    [Required(ErrorMessage = "Enter your email address."), EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Choose a password."), MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm your password."), DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "The passwords do not match.")]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Choose a role.")]
    [Display(Name = "I am registering as")]
    public string Role { get; set; } = "Participant";

    [Required(ErrorMessage = "Enter your first name."), StringLength(100)]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enter your last name."), StringLength(100)]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "Phone number"), StringLength(20)]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Organisation name"), StringLength(150)]
    public string? OrganisationName { get; set; }

    [DataType(DataType.Date), Display(Name = "Date of birth")]
    public DateTime? DateOfBirth { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (Role != "Organiser" && Role != "Participant")
            yield return new ValidationResult("Choose Organiser or Participant.", new[] { nameof(Role) });

        if (Role == "Organiser")
        {
            if (string.IsNullOrWhiteSpace(PhoneNumber))
                yield return new ValidationResult("Enter a phone number.", new[] { nameof(PhoneNumber) });
            if (string.IsNullOrWhiteSpace(OrganisationName))
                yield return new ValidationResult("Enter your organisation name.", new[] { nameof(OrganisationName) });
        }

        if (Role == "Participant")
        {
            if (DateOfBirth == null)
                yield return new ValidationResult("Enter your date of birth.", new[] { nameof(DateOfBirth) });
            else if (DateOfBirth > DateTime.Today || DateOfBirth < DateTime.Today.AddYears(-120))
                yield return new ValidationResult("Enter a valid date of birth.", new[] { nameof(DateOfBirth) });
        }
    }
}

// --------------------------------------------------------------------- home
public class HomeViewModel
{
    public List<EventDto> Events { get; set; } = new();
    public List<string> EventTypes { get; set; } = new();

    public string? Search { get; set; }
    public string? EventType { get; set; }
    public string? Location { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public decimal? MaxDistance { get; set; }

    public bool HasFilters =>
        !string.IsNullOrWhiteSpace(Search) || !string.IsNullOrWhiteSpace(EventType) ||
        !string.IsNullOrWhiteSpace(Location) || From != null || To != null || MaxDistance != null;
}

public class EventDetailsViewModel
{
    public EventDto Event { get; set; } = new();
    public List<CategoryDto> Categories { get; set; } = new();
    public bool AlreadyEnrolled { get; set; }
    public bool IsPast => Event.EventDate.Date < DateTime.Today;
}

// ------------------------------------------------------------- participant
public class ProfileViewModel
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "Enter your first name."), StringLength(100)]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enter your last name."), StringLength(100)]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enter your date of birth."), DataType(DataType.Date)]
    [Display(Name = "Date of birth")]
    public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-20);

    public string? ProfilePictureUrl { get; set; }

    [Display(Name = "Profile picture")]
    public IFormFile? Picture { get; set; }
}

// --------------------------------------------------------------- organiser
public class EventSummary
{
    public EventDto Event { get; set; } = new();
    public int Enrolments { get; set; }
}

public class DashboardViewModel
{
    public List<EventSummary> Events { get; set; } = new();
    public int TotalEnrolments => Events.Sum(e => e.Enrolments);
    public int UpcomingCount => Events.Count(e => e.Event.EventDate.Date >= DateTime.Today);
    public EventSummary? NextEvent => Events
        .Where(e => e.Event.EventDate.Date >= DateTime.Today)
        .OrderBy(e => e.Event.EventDate).FirstOrDefault();
}

public class EventFormViewModel
{
    public int EventId { get; set; }

    [Required(ErrorMessage = "Enter the event name."), StringLength(150)]
    [Display(Name = "Event name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description can be at most 1000 characters.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Choose the event date."), DataType(DataType.Date)]
    [Display(Name = "Event date")]
    public DateTime? EventDate { get; set; }

    [Required(ErrorMessage = "Enter the location."), StringLength(255)]
    public string Location { get; set; } = string.Empty;

    [Range(0.1, 1000, ErrorMessage = "Distance must be between 0.1 and 1000 km.")]
    [Display(Name = "Distance (km)")]
    public decimal DistanceKm { get; set; }

    [Required(ErrorMessage = "Choose an event type."), StringLength(100)]
    [Display(Name = "Event type")]
    public string EventType { get; set; } = string.Empty;

    public string? BannerImageUrl { get; set; }

    [Display(Name = "Banner image")]
    public IFormFile? Banner { get; set; }

    public static readonly string[] Types =
        { "Road Run", "Trail Run", "Cycling", "Triathlon", "Swimming", "Walk", "Obstacle Race", "Other" };

    public IEnumerable<SelectListItem> TypeOptions =>
        Types.Select(t => new SelectListItem(t, t, t == EventType));
}

public class CategoriesViewModel
{
    public EventDto Event { get; set; } = new();
    public List<CategoryDto> Categories { get; set; } = new();
    public CategoryDto NewCategory { get; set; } = new();
}

public class EventEnrolmentsViewModel
{
    public EventDto Event { get; set; } = new();
    public List<EnrolmentDetailDto> Enrolments { get; set; } = new();
}

public class ResultRow
{
    public int EnrolmentId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int? ResultId { get; set; }

    [Display(Name = "Finish time (hh:mm:ss)")]
    public string? FinishTime { get; set; }

    [Display(Name = "Position")]
    public int? FinishPosition { get; set; }
}

public class ResultsEntryViewModel
{
    public EventDto Event { get; set; } = new();
    public bool EventHasEnded => Event.EventDate.Date <= DateTime.Today;

    [Display(Name = "Total finishers")]
    public int? TotalFinishers { get; set; }

    public List<ResultRow> Rows { get; set; } = new();
}