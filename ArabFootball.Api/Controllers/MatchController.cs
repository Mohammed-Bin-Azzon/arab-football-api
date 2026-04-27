using api_training.Controllers;
using ArabFootball.Api.Features.Enums;
using ArabFootball.Api.Features.Matchs;
using ArabFootball.Api.Features.Matchs.MatchDto;
using ArabFootball.Api.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArabFootball.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MatchController : AppControllerBase
    {
        private readonly IMatchService _service;

        public MatchController(IMatchService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, string? search = null)
        {
            return Response(await _service.GetAllMatchesAsync(pageNumber, pageSize, search));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Response(await _service.GetMatchByIdAsync(id));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMatchDto dto)
        {
            //if (!ModelState.IsValid)
            //    return this.ValidationProblemResponse("بيانات المباراة غير صالحة.");

            int adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Response(await _service.CreateMatchAsync(dto, adminId));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMatchDto dto)
        {
            //if (!ModelState.IsValid)
            //    return this.ValidationProblemResponse("بيانات تحديث المباراة غير صالحة.");

            return Response(await _service.UpdateMatchAsync(id, dto));
            
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Response(await _service.DeleteMatchAsync(id));
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] MatchStatus status)
        {
            return Response(await _service.ChangeStatusAsync(id, status));
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/predictions/open")]
        public async Task<IActionResult> OpenPredictions(int id)
        {
            return Response(await _service.OpenPredictionsAsync(id));
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/predictions/close")]
        public async Task<IActionResult> ClosePredictions(int id)
        {
            return Response(await _service.ClosePredictionsAsync(id));
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/chat/link")]
        public async Task<IActionResult> LinkChat(int id, [FromBody] string chatUrl)
        {
            return Response(await _service.LinkChatAsync(id, chatUrl));
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/chat/unlink")]
        public async Task<IActionResult> UnlinkChat(int id)
        {
            return Response(await _service.UnlinkChatAsync(id));
        }
    }
}