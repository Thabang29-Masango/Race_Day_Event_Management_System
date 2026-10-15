
using System.ComponentModel.DataAnnotations;

namespace RaceDayManagementSystem.DTOs;

public class RegisterRequest
{
    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "Participant";

    [Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string? OrganisationName { get; set; }

    public DateTime? DateOfBirth { get; set; }
}
