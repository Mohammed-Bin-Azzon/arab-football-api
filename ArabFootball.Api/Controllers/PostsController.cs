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
    public class PostsController : ControllerBase
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
                return this.ValidationProblemResponse("بيانات المنشور غير صالحة.");

            var fanId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _postsService.CreatePostAsync(fanId, dto);
            return this.ToActionResult(response);
        }

        [HttpGet("feed")]
        public async Task<IActionResult> GetFeed()
        {
            var response = await _postsService.GetHomeFeedAsync();
            return this.ToActionResult(response);
        }

        [HttpDelete("{postId:int}")]
        public async Task<IActionResult> Delete(int postId)
        {
            var fanId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _postsService.DeletePostAsync(postId, fanId);
            return this.ToActionResult(response);
        }
    }
}