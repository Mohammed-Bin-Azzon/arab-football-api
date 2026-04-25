using ArabFootball.Api.Features.Enums;
using System.ComponentModel.DataAnnotations;

namespace ArabFootball.Api.Shared.Entity
{
    public abstract class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        [Required]
        public UserRole Role { get; set; }
    }


    
}
