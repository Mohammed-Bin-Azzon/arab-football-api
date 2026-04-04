using ArabFootball.Api.Features.Predictions.PredictionsDto;

namespace ArabFootball.Api.Features.Predictions
{
    public interface IPredictionsService
    {
        Task<PredictionDto> SubmitPredictionAsync(SubmitPredictionDto dto);
        Task<List<PredictionDto>> GetFanPredictionsAsync(int fanId);
    }
}
