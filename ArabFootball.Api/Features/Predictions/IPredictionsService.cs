using ArabFootball.Api.Features.Predictions.PredictionsDto;

namespace ArabFootball.Api.Features.Predictions
{
    public interface IPredictionsService
    {
        Task<PredictionDto> SubmitPredictionAsync(int fanId, SubmitPredictionDto dto);
        Task<List<PredictionDto>> GetMyPredictionsAsync(int fanId);
    }
}