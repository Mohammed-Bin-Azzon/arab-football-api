using System.Security.Claims;
using ArabFootball.Api.Features.Comments;
using ArabFootball.Api.Features.Comments.CommentsDto;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] CreateCommentDto dto)
        {
            try
            {
                var fanId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _commentsService.AddCommentAsync(fanId, dto);
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

        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetPostComments(int postId)
        {
            var comments = await _commentsService.GetPostCommentsAsync(postId);
            return Ok(comments);
        }
    }
}