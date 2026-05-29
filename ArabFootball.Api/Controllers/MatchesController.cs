using ArabFootball.Api.Features.Enums;
using ArabFootball.Api.Features.Matchs;
using ArabFootball.Api.Features.Matchs.MatchDto;
using ArabFootball.Api.Shared.Helpers;
using ArabFootball.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace ArabFootball.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MatchesController : AppControllerBase
    {
        private readonly IMatchService _service;

        public MatchesController(IMatchService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        {
            return Response(await _service.GetAllMatchesAsync(pageNumber, pageSize, search));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            return Response(await _service.GetMatchByIdAsync(id));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMatchDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Response(await _service.CreateMatchAsync(dto, adminId));
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/update")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateMatchDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            return Response(await _service.UpdateMatchAsync(id, dto));
            
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            return Response(await _service.DeleteMatchAsync(id));
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> ChangeStatus([FromRoute] int id, [FromBody] MatchStatus status)
        {
            return Response(await _service.ChangeStatusAsync(id, status));
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/predictions-open")]
        public async Task<IActionResult> OpenPredictions([FromRoute] int id)
        {
            return Response(await _service.OpenPredictionsAsync(id));
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/predictions-close")]
        public async Task<IActionResult> ClosePredictions([FromRoute] int id)
        {
            return Response(await _service.ClosePredictionsAsync(id));
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/chat-link")]
        public async Task<IActionResult> LinkChat([FromRoute] int id, [FromBody] string chatUrl)
        {
            return Response(await _service.LinkChatAsync(id, chatUrl));
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/chat-unlink")]
        public async Task<IActionResult> UnlinkChat([FromRoute] int id)
        {
            return Response(await _service.UnlinkChatAsync(id));
        }
    }
}