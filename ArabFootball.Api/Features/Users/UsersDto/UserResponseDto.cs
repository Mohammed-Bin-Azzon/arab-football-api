using System.Text.Json.Serialization;

namespace ArabFootball.Api.Features.Users.UsersDto
{
    public class UserResponseDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        //public string Token { get; set; }
        //[JsonIgnore]
        //public DateTime ExpireOn { get; set; }
    }
}
