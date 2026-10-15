
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RaceDayManagementSystem.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(255)]
        [Column(TypeName = "varchar(255)")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [Column(TypeName = "varchar(255)")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string Role { get; set; } = string.Empty;

        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }
    }
}
