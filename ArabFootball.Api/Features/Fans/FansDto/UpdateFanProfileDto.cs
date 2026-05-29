using System.ComponentModel.DataAnnotations;

namespace ArabFootball.Api.Features.Fans.Dtos
{
    public class UpdateFanProfileDto
    {
        [MaxLength(100)]
        public string? DisplayName { get; set; }

        [MaxLength(500)]
        public string? Bio { get; set; }

        public IFormFile? ProfileImage { get; set; }

        [MaxLength(50)]
        public string? FavoriteTeamCode { get; set; }

        [MaxLength(50)]
        public string? FavoritePlayerCode { get; set; }
    }
}