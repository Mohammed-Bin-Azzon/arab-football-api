using System.Security.Claims;
using ArabFootball.Api.Features.Predictions;
using ArabFootball.Api.Features.Predictions.PredictionsDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArabFootball.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
            try
            {
                var fanId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _predictionsService.SubmitPredictionAsync(fanId, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyPredictions()
        {
            var fanId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var predictions = await _predictionsService.GetMyPredictionsAsync(fanId);
            return Ok(predictions);
        }
    }
}