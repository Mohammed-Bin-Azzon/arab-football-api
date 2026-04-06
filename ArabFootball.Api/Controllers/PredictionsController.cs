using ArabFootball.Api.Features.Predictions;
using ArabFootball.Api.Features.Predictions.PredictionsDto;
using Microsoft.AspNetCore.Mvc;

namespace ArabFootball.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredictionsController : ControllerBase
    {
        private readonly IPredictionsService _predictionsService;

        public PredictionsController(IPredictionsService predictionsService)
        {
            _predictionsService = predictionsService;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitPrediction([FromBody] SubmitPredictionDto dto)
        {
            var result = await _predictionsService.SubmitPredictionAsync(dto);
            return Ok(result);
        }

        [HttpGet("fan/{fanId}")]
        public async Task<IActionResult> GetFanPredictions(int fanId)
        {
            var predictions = await _predictionsService.GetFanPredictionsAsync(fanId);
            return Ok(predictions);
        }
    }
}

