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
            //if (!ModelState.IsValid)
            //    return this.ValidationProblemResponse("بيانات المنشور غير صالحة.");

            var fanId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Response(await _postsService.CreatePostAsync(fanId, dto));
        }

        [HttpGet("feed")]
        public async Task<IActionResult> GetFeed()
        {
            return Response(await _postsService.GetHomeFeedAsync());
        }

        [HttpDelete("{postId:int}")]
        public async Task<IActionResult> Delete(int postId)
        {
            var fanId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Response(await _postsService.DeletePostAsync(postId, fanId));
        }
    }
}