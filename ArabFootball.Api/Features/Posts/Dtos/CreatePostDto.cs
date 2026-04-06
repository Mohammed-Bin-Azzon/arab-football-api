using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ArabFootball.Api.Features.Posts.Dtos
{
    public class CreatePostDto
    {
        [MaxLength(1000, ErrorMessage = "الوصف طويل جداً")]
        public string? Caption { get; set; } 

        [Required(ErrorMessage = "يجب إرفاق ملف ميديا (صورة أو فيديو)")]
        public IFormFile MediaFile { get; set; } 
    }
}