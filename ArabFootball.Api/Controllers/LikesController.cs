using System.Security.Claims;
using ArabFootball.Api.Features.Likes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArabFootball.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly ILikesService _likesService;

        public LikesController(ILikesService likesService)
        {
            _likesService = likesService;
        }

        [Authorize]
        [HttpPost("toggle/{postId}")]
        public async Task<IActionResult> ToggleLike(int postId)
        {
            try
            {
                var fanId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _likesService.ToggleLikeAsync(postId, fanId);
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
    }
}