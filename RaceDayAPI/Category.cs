
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayManagementSystem.Models;

[Table("Categories")]
public class Category
{
    [Key]
    public int CategoryId { get; set; }

    [Required]
    public int EventId { get; set; }

    [Required, MaxLength(100)]
    [Column(TypeName = "varchar(100)")]
    public string Name { get; set; } = string.Empty;

    public int MinAge { get; set; }

    public int MaxAge { get; set; }

    [Column(TypeName = "decimal(6,2)")]
    public decimal DistanceKm { get; set; }

    [ForeignKey(nameof(EventId))]
    public Event? Event { get; set; }
}
