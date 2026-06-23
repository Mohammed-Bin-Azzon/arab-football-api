using System.Security.Claims;
using ArabFootball.Api.Features.Posts.Dtos;
using ArabFootball.Api.Features.Posts.Services;
using ArabFootball.Api.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArabFootball.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostsController : AppControllerBase
    {
        private readonly IPostsService _postsService;

        public PostsController(IPostsService postsService)
        {
            _postsService = postsService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreatePostDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var fanId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Response(await _postsService.CreatePostAsync(fanId, dto));
        }

        [AllowAnonymous]
        [HttpGet("{postId:int}")]
        public async Task<IActionResult> GetById(int postId)
        {
            var fanId = GetCurrentFanIdOrNull();
            return Response(await _postsService.GetPostByIdAsync(postId, fanId));
        }

        [AllowAnonymous]
        [HttpGet("feed")]
        public async Task<IActionResult> GetFeed()
        {
            var fanId = GetCurrentFanIdOrNull();
            return Response(await _postsService.GetHomeFeedAsync(fanId));
        }

        [HttpPatch("{postId:int}")]
        public async Task<IActionResult> UpdatePost(int postId, [FromForm] UpdatePostDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var fanId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Response(await _postsService.UpdatePostAsync(postId, fanId, dto));
        }

        [HttpDelete("{postId:int}")]
        public async Task<IActionResult> Delete(int postId)
        {
            var fanId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Response(await _postsService.DeletePostAsync(postId, fanId));
        }

        private int? GetCurrentFanIdOrNull()
        {
            if (User.Identity?.IsAuthenticated != true)
                return null;

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userId, out var fanId) ? fanId : null;
        }
    }
}