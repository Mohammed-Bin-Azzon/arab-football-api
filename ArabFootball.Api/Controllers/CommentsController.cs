using ArabFootball.Api.Features.Comments;
using ArabFootball.Api.Features.Comments.CommentsDto;
using Microsoft.AspNetCore.Mvc;

namespace ArabFootball.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentsService _commentsService;

        public CommentsController(ICommentsService commentsService)
        {
            _commentsService = commentsService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddComment([FromBody] CreateCommentDto dto)
        {
            var result = await _commentsService.AddCommentAsync(dto);
            if (result == null) return NotFound(new { message = "المنشور غير موجود" });
            return Ok(result);
        }

        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetPostComments(int postId)
        {
            var comments = await _commentsService.GetPostCommentsAsync(postId);
            return Ok(comments);
        }
    }
}

