using ArabFootball.Api.Features.Posts.Dtos;
using ArabFootball.Api.Features.Posts.Services;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly IPostsService _postsService;

    public PostsController(IPostsService postsService)
    {
        _postsService = postsService;
    }

    
    [HttpPost("create/{fanId}")]
    public async Task<IActionResult> Create(int fanId, [FromForm] CreatePostDto dto)
    {
        var result = await _postsService.CreatePostAsync(fanId, dto);
        return result ? Ok(new { message = "تم النشر بنجاح" }) : BadRequest("خطأ في النشر");
    }

    [HttpGet("feed/{fanId}")]
    public async Task<IActionResult> GetFeed(int fanId)
    {
        var posts = await _postsService.GetHomeFeedAsync(fanId);
        return Ok(posts);
    }


    [HttpDelete("{postId}/{fanId}")]
    public async Task<IActionResult> Delete(int postId, int fanId)
    {
        var result = await _postsService.DeletePostAsync(postId, fanId);
        return result ? Ok(new { message = "تم الحذف" }) : NotFound();
    }
}