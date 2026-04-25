using System.ComponentModel.DataAnnotations;

namespace ArabFootball.Api.Features.Auth.AuthDto
{
    public class LoginDto
    {
        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}