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
    public class MatchController : ControllerBase
    {
        private readonly IMatchService _service;

        public MatchController(IMatchService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, string? search = null)
        {
            var response = await _service.GetAllMatchesAsync(pageNumber, pageSize, search);
            return this.ToActionResult(response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _service.GetMatchByIdAsync(id);
            return this.ToActionResult(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMatchDto dto)
        {
            if (!ModelState.IsValid)
                return this.ValidationProblemResponse("بيانات المباراة غير صالحة.");

            int adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _service.CreateMatchAsync(dto, adminId);
            return this.ToActionResult(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMatchDto dto)
        {
            if (!ModelState.IsValid)
                return this.ValidationProblemResponse("بيانات تحديث المباراة غير صالحة.");

            var response = await _service.UpdateMatchAsync(id, dto);
            return this.ToActionResult(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _service.DeleteMatchAsync(id);
            return this.ToActionResult(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] MatchStatus status)
        {
            var response = await _service.ChangeStatusAsync(id, status);
            return this.ToActionResult(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/predictions/open")]
        public async Task<IActionResult> OpenPredictions(int id)
        {
            var response = await _service.OpenPredictionsAsync(id);
            return this.ToActionResult(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/predictions/close")]
        public async Task<IActionResult> ClosePredictions(int id)
        {
            var response = await _service.ClosePredictionsAsync(id);
            return this.ToActionResult(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/chat/link")]
        public async Task<IActionResult> LinkChat(int id, [FromBody] string chatUrl)
        {
            var response = await _service.LinkChatAsync(id, chatUrl);
            return this.ToActionResult(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/chat/unlink")]
        public async Task<IActionResult> UnlinkChat(int id)
        {
            var response = await _service.UnlinkChatAsync(id);
            return this.ToActionResult(response);
        }
    }
}