
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayManagementSystem.Models
{
    [Table("Participants")]
    public class Participant
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }

        [MaxLength(500)]
        [Column(TypeName = "varchar(500)")]
        public string? ProfilePictureUrl { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
    }
}
