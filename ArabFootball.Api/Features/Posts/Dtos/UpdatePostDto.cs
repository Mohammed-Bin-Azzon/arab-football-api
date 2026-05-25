using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ArabFootball.Api.Features.Posts.Dtos
{
    public class UpdatePostDto
    {
        [MaxLength(1000, ErrorMessage = "نص المنشور طويل جداً")]
        public string? Caption { get; set; }

        public IFormFile? Media { get; set; }
    }
}