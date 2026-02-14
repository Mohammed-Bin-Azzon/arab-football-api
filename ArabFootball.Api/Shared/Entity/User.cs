using ArabFootball.Api.Features.Enums;
using System.ComponentModel.DataAnnotations;

namespace ArabFootball.Api.Shared.Entity
{
    public  class User
    {
        
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public UserRole Role { get; set; }
    }


    
}
