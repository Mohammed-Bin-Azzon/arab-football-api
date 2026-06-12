namespace ArabFootball.Api.Shared.Entity
{
    public class Prediction
    {
        public int Id { get; set; }

        public int FanId { get; set; }
        public Fan Fan { get; set; } = null!;

        public int MatchId { get; set; }
        public Match Match { get; set; } = null!;

        public int PredictedHomeScore { get; set; }
        public int PredictedAwayScore { get; set; }

        public bool IsProcessed { get; set; } = false;
        public int PointsEarned { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
