namespace ArabFootball.Api.Features.Predictions.PredictionsDto
{
    public class SubmitPredictionDto
    {
        public int FanId { get; set; } // مؤقتاً حتى نستخدم الـ Auth
        public int MatchId { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
    }
}
