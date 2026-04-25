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
    public class CommentsController : ControllerBase
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
                return this.ValidationProblemResponse("بيانات التعليق غير صالحة.");

            var fanId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _commentsService.AddCommentAsync(fanId, dto);
            return this.ToActionResult(response);
        }

        [HttpGet("post/{postId:int}")]
        public async Task<IActionResult> GetPostComments(int postId)
        {
            var response = await _commentsService.GetPostCommentsAsync(postId);
            return this.ToActionResult(response);
        }
    }
}