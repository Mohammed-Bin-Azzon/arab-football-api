using System.Security.Claims;
using ArabFootball.Api.Features.Comments;
using ArabFootball.Api.Features.Comments.CommentsDto;
using ArabFootball.Api.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArabFootball.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentsController : AppControllerBase
    {
        private readonly ICommentsService _commentsService;

        public CommentsController(ICommentsService commentsService)
        {
            _commentsService = commentsService;
        }

        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] CreateCommentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var fanId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Response(await _commentsService.AddCommentAsync(fanId, dto));
        }

        [AllowAnonymous]
        [HttpGet("post/{postId:int}")]
        public async Task<IActionResult> GetPostComments(int postId)
        {
            return Response(await _commentsService.GetPostCommentsAsync(postId));
        }

        [HttpDelete("{commentId:int}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var fanId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Response(await _commentsService.DeleteCommentAsync(commentId, fanId));
        }
    }
}