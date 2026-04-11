using System.Security.Claims;
using ArabFootball.Api.Features.Posts.Dtos;
using ArabFootball.Api.Features.Posts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArabFootball.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostsService _postsService;

        public PostsController(IPostsService postsService)
        {
            _postsService = postsService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreatePostDto dto)
        {
            try
            {
                var fanId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var post = await _postsService.CreatePostAsync(fanId, dto);
                return Ok(post);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("feed")]
        public async Task<IActionResult> GetFeed()
        {
            var posts = await _postsService.GetHomeFeedAsync();
            return Ok(posts);
        }

        [Authorize]
        [HttpDelete("{postId}")]
        public async Task<IActionResult> Delete(int postId)
        {
            var fanId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _postsService.DeletePostAsync(postId, fanId);
            if (!result)
                return NotFound(new { message = "المنشور غير موجود أو لا تملك صلاحية حذفه." });

            return Ok(new { message = "تم الحذف بنجاح." });
        }
    }
}