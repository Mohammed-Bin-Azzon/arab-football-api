using System.Text.Json.Serialization;

namespace ArabFootball.Api.Features.Auth.AuthDto
{
    public class AuthResponseDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        //public string Token { get; set; }
        //[JsonIgnore]
        //public DateTime ExpireOn { get; set; }
    }
}
