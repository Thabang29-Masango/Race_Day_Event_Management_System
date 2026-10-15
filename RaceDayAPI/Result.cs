
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayManagementSystem.Models;

[Table("Results")]
public class Result
{
    [Key]
    public int ResultId { get; set; }

    [Required]
    public int EnrolmentId { get; set; }

    [Column(TypeName = "time")]
    public TimeSpan? FinishTime { get; set; }

    public int? FinishPosition { get; set; }

    public int? TotalFinishers { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime CapturedDate { get; set; }

    [ForeignKey(nameof(EnrolmentId))]
    public Enrolment? Enrolment { get; set; }
}
