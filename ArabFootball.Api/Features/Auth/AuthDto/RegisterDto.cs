using System.ComponentModel.DataAnnotations;

namespace ArabFootball.Api.Features.Auth.AuthDto
{
    public class RegisterDto
    {
        [Required, MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; } = null!;

        [Required, MinLength(6), MaxLength(100)]
        public string Password { get; set; } = null!;
    }
}