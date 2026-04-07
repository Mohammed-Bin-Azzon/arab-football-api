using ArabFootball.Api.Features.Enums;

namespace ArabFootball.Api.Shared.Entity
{
    public class Match
    {
        public int Id { get; set; }
        public int AdminId { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string League { get; set; }
        public DateTime StartTime { get; set; }
        public MatchStatus Status { get; set; }
        public string Stats { get; set; }
        public PredictionState PredictionState { get; set; }
        public string? ChatUrl { get; set; }

        public Admin Admin { get; set; }
    }
}
