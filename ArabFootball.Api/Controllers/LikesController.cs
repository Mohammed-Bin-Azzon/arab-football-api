using System.Security.Claims;
using ArabFootball.Api.Features.Likes;
using ArabFootball.Api.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArabFootball.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LikesController : ControllerBase
    {
        private readonly ILikesService _likesService;

        public LikesController(ILikesService likesService)
        {
            _likesService = likesService;
        }

        [HttpPost("toggle/{postId:int}")]
        public async Task<IActionResult> ToggleLike(int postId)
        {
            var fanId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _likesService.ToggleLikeAsync(postId, fanId);
            return this.ToActionResult(response);
        }
    }
}