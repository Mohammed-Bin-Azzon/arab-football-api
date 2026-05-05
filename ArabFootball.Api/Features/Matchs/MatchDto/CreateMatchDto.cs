using ArabFootball.Api.Features.Enums;
using System.ComponentModel.DataAnnotations;

namespace ArabFootball.Api.Features.Matchs.MatchDto
{
    public class CreateMatchDto
    {
        [Required, MaxLength(100)]
        public string HomeTeam { get; set; } = null!;

        [Required, MaxLength(100)]
        public string AwayTeam { get; set; } = null!;

        [Required, MaxLength(100)]
        public string League { get; set; } = null!;

        public DateTime StartTime { get; set; }

        
    }
}