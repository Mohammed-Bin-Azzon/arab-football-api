using ArabFootball.Api.Features.Bookmarks;
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

        [HttpPost("toggle/{postId}/{fanId}")]
        public async Task<IActionResult> ToggleBookmark(int postId, int fanId)
        {
            var result = await _bookmarksService.ToggleBookmarkAsync(postId, fanId);

            if (result == null)
            {
                return NotFound(new { message = "المنشور غير موجود" });
            }

            return Ok(result);
        }
    }
}
