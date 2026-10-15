
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayManagementSystem.Models;

[Table("Enrolments")]
public class Enrolment
{
    [Key]
    public int EnrolmentId { get; set; }

    [Required]
    public int ParticipantId { get; set; }

    [Required]
    public int EventId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime EnrolmentDate { get; set; }

    [Required, MaxLength(30)]
    [Column(TypeName = "varchar(30)")]
    public string Status { get; set; } = "Active";

    [ForeignKey(nameof(ParticipantId))]
    public Participant? Participant { get; set; }

    [ForeignKey(nameof(EventId))]
    public Event? Event { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public Category? Category { get; set; }

    public Result? Result { get; set; }
}
