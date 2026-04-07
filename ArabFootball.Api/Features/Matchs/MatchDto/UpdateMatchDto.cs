using System.ComponentModel.DataAnnotations;

namespace ArabFootball.Api.Features.Matchs.MatchDto
{
    public class UpdateMatchDto
    {
        [Required]
        [MaxLength(100)]
        public string HomeTeam { get; set; }

        [Required]
        [MaxLength(100)]
        public string AwayTeam { get; set; }

        [Required]
        [MaxLength(100)]
        public string League { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public string? Stats { get; set; }
    }
}
