using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ArabFootball.Api.Features.Fans.Dtos
{
    public class UpdateFanProfileDto
    {
        [MaxLength(100, ErrorMessage = "اسم العرض طويل جداً")]
        public string? DisplayName { get; set; }

        [MaxLength(500, ErrorMessage = "النبذة طويلة جداً")]
        public string? Bio { get; set; }

        public IFormFile? ProfileImage { get; set; }
    }
}