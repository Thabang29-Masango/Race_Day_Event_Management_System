
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayManagementSystem.Models;

[Table("Events")]
public class Event
{
    [Key]
    public int EventId { get; set; }

    [Required]
    public int OrganiserId { get; set; }

    [Required, MaxLength(150)]
    [Column(TypeName = "varchar(150)")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    [Column(TypeName = "varchar(1000)")]
    public string? Description { get; set; }

    [Required]
    [Column(TypeName = "date")]
    public DateTime EventDate { get; set; }

    [Required, MaxLength(255)]
    [Column(TypeName = "varchar(255)")]
    public string Location { get; set; } = string.Empty;

    [Column(TypeName = "decimal(6,2)")]
    public decimal DistanceKm { get; set; }

    [Required, MaxLength(100)]
    [Column(TypeName = "varchar(100)")]
    public string EventType { get; set; } = string.Empty;

    [MaxLength(500)]
    [Column(TypeName = "varchar(500)")]
    public string? BannerImageUrl { get; set; }

    [ForeignKey(nameof(OrganiserId))]
    public Organiser? Organiser { get; set; }

    public ICollection<Category> Categories { get; set; }
        = new List<Category>();
}
