namespace RaceDayManagementSystemMVC.Models
{
    public class LoginResponse
    {
        public string Token { get; set; } = "";
        public int UserId { get; set; } 
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
    }

    public class ParticipantDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public DateTime DateOfBirth { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }

    public class OrganiserDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string? PhoneNumber { get; set; }
        public string? OrganisationName { get; set; }
    }

    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = "";
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public decimal DistanceKm { get; set; }
    }

    public class EnrolmentDetailDto
    {
        public int EnrolmentId { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; } = "";
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public string ParticipantName { get; set; } = "";
        public DateTime EnrolmentDate { get; set; }
        public string Status { get; set; } = "";
        public TimeSpan? FinishTime { get; set; }
        public int? FinishPosition { get; set; }
        public int? TotalFinishers { get; set; }
        public int? ResultId { get; set; }
    }

    public class EventDto
    {
        public int EventId { get; set; }
        public int OrganiserId { get; set; } 
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = "";
        public decimal DistanceKm { get; set; }
        public string EventType { get; set; } = "";
        public string? BannerImageUrl { get; set; }
    }
}
