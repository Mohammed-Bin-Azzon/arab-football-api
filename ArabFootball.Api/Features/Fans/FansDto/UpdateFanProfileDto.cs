using Microsoft.AspNetCore.Http; // Requird To Recives Files

namespace ArabFootball.Api.Features.Fans.Dtos
{
    public class UpdateFanProfileDto
    {
        public string? DisplayName { get; set; }
        public string? Bio { get; set; }
        /*public bool IsPrivate { get; set; }*/

        public IFormFile? ProfileImage { get; set; }
    }
}