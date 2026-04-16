using ArabFootball.Api.Features.Enums;
using ArabFootball.Api.Features.Matchs;
using ArabFootball.Api.Features.Matchs.MatchDto;
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
            var result = await _service.GetAllMatchesAsync(pageNumber, pageSize, search);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var match = await _service.GetMatchByIdAsync(id);
            if (match == null)
                return NotFound(new { message = "المباراة غير موجودة." });

            return Ok(match);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMatchDto dto)
        {
            try
            {
                int adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _service.CreateMatchAsync(dto, adminId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMatchDto dto)
        {
            try
            {
                var result = await _service.UpdateMatchAsync(id, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteMatchAsync(id);
            if (!result)
                return NotFound(new { message = "المباراة غير موجودة." });

            return Ok(new { message = "تم حذف المباراة." });
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] MatchStatus status)
        {
            var result = await _service.ChangeStatusAsync(id, status);
            if (!result)
                return NotFound(new { message = "المباراة غير موجودة." });

            return Ok(new { message = "تم تحديث حالة المباراة." });
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/predictions/open")]
        public async Task<IActionResult> OpenPredictions(int id)
        {
            var result = await _service.OpenPredictionsAsync(id);
            if (!result)
                return NotFound(new { message = "المباراة غير موجودة." });

            return Ok(new { message = "تم فتح التوقعات." });
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/predictions/close")]
        public async Task<IActionResult> ClosePredictions(int id)
        {
            var result = await _service.ClosePredictionsAsync(id);
            if (!result)
                return NotFound(new { message = "المباراة غير موجودة." });

            return Ok(new { message = "تم إغلاق التوقعات." });
        }


        [HttpPatch("{id}/chat/link")]
        public async Task<IActionResult> LinkChat(int id, [FromBody] string chatUrl)
        {
            var result = await _service.LinkChatAsync(id, chatUrl);
            if (!result)
                return NotFound(new { message = "المباراة غير موجودة." });

            return Ok(new { message = "تم ربط المحادثة." });
        }


        [HttpPatch("{id}/chat/unlink")]
        public async Task<IActionResult> UnlinkChat(int id)
        {
            var result = await _service.UnlinkChatAsync(id);
            if (!result)
                return NotFound(new { message = "المباراة غير موجودة." });

            return Ok(new { message = "تم فك ربط المحادثة." });
        }
    }
}