namespace ArabFootball.Api.Features.Predictions.PredictionsDto
{
    public class PredictionDto
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int PredictedHomeScore { get; set; }
        public int PredictedAwayScore { get; set; }
        public bool IsProcessed { get; set; }
        public int PointsEarned { get; set; }
    }
}
