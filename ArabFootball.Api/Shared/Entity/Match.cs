using ArabFootball.Api.Features.Enums;

namespace ArabFootball.Api.Shared.Entity
{
   public class Match
    {
        public int Id { get; set; }

        public int AdminId { get; set; }
        public Admin Admin { get; set; } = null!;

        public string HomeTeam { get; set; } = null!;
        public string? HomeTeamLogoUrl { get; set; }
        public string AwayTeam { get; set; } = null!;
        public string? AwayTeamLogoUrl { get; set; }
        public string League { get; set; } = null!;

        public DateTime StartTime { get; set; }

        public MatchStatus Status { get; set; }
        public PredictionState PredictionState { get; set; }

        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }

        public string? StatsJson { get; set; }
        public string? ChatUrl { get; set; }

        public ICollection<Prediction> Predictions { get; set; } = new List<Prediction>();
    }
}
