using ArabFootball.Api.Features.Likes;
using ArabFootball.Api.Features.Likes.LikesDto;
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

        [HttpPost("toggle/{postId}/{fanId}")]
        public async Task<IActionResult> ToggleLike(int postId, int fanId)
        {
            var result = await _likesService.ToggleLikeAsync(postId, fanId);

            if (result == null)
            {
                return NotFound(new { message = "المنشور غير موجود" });
            }

            return Ok(result);
        }
    }
}
