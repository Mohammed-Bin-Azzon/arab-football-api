using ArabFootball.Api.Features.Predictions.PredictionsDto;
using ArabFootball.Shared.Helpers;

namespace ArabFootball.Api.Features.Predictions
{
    public interface IPredictionsService
    {
        Task<ApiResponse<PredictionDto>> SubmitPredictionAsync(int fanId, SubmitPredictionDto dto);
        Task<ApiResponse<List<PredictionDto>>> GetMyPredictionsAsync(int fanId);
        Task<ApiResponse<ProcessPredictionsResultDto>> ProcessPredictionsAsync(ProcessPredictionsDto dto);
    }
}