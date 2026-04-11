using System.Security.Claims;
using ArabFootball.Api.Features.Bookmarks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArabFootball.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookmarksController : ControllerBase
    {
        private readonly IBookmarksService _bookmarksService;

        public BookmarksController(IBookmarksService bookmarksService)
        {
            _bookmarksService = bookmarksService;
        }

        [Authorize]
        [HttpPost("toggle/{postId}")]
        public async Task<IActionResult> ToggleBookmark(int postId)
        {
            try
            {
                var fanId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _bookmarksService.ToggleBookmarkAsync(postId, fanId);
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
    }
}