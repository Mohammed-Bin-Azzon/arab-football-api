namespace ArabFootball.Api.Features.Predictions.PredictionsDto
{
    public class ProcessPredictionsResultDto
    {
        public int MatchId { get; set; }
        public int ActualHomeScore { get; set; }
        public int ActualAwayScore { get; set; }
        public int TotalPredictions { get; set; }
        public int CorrectPredictions { get; set; }
        public int ProcessedPredictions { get; set; }
        public int PointsPerCorrectPrediction { get; set; }
    }
}