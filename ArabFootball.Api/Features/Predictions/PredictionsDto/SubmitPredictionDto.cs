using System.ComponentModel.DataAnnotations;

namespace ArabFootball.Api.Features.Predictions.PredictionsDto
{
    public class SubmitPredictionDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "معرّف المباراة غير صالح")]
        public int MatchId { get; set; }

        [Range(0, 50, ErrorMessage = "عدد أهداف الفريق المستضيف غير صالح")]
        public int HomeScore { get; set; }

        [Range(0, 50, ErrorMessage = "عدد أهداف الفريق الضيف غير صالح")]
        public int AwayScore { get; set; }
    }
}