using System.Security.Claims;
using ArabFootball.Api.Features.Predictions;
using ArabFootball.Api.Features.Predictions.PredictionsDto;
using ArabFootball.Api.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArabFootball.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PredictionsController : AppControllerBase
    {
        private readonly IPredictionsService _predictionsService;

        public PredictionsController(IPredictionsService predictionsService)
        {
            _predictionsService = predictionsService;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitPrediction([FromBody] SubmitPredictionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var fanId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Response(await _predictionsService.SubmitPredictionAsync(fanId, dto));
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyPredictions()
        {
            var fanId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Response(await _predictionsService.GetMyPredictionsAsync(fanId));
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessPredictions([FromBody] ProcessPredictionsDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Response(await _predictionsService.ProcessPredictionsAsync(dto));
        }
    }
}