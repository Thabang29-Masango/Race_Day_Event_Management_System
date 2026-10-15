
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayManagementSystem.Models
{
    [Table("Organisers")]
    public class Organiser
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
        [MaxLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        [Column(TypeName = "varchar(150)")]
        public string OrganisationName { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
    }
}
