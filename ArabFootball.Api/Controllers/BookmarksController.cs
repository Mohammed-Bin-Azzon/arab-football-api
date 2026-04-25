using System.Security.Claims;
using ArabFootball.Api.Features.Bookmarks;
using ArabFootball.Api.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArabFootball.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookmarksController : ControllerBase
    {
        private readonly IBookmarksService _bookmarksService;

        public BookmarksController(IBookmarksService bookmarksService)
        {
            _bookmarksService = bookmarksService;
        }

        [HttpPost("toggle/{postId:int}")]
        public async Task<IActionResult> ToggleBookmark(int postId)
        {
            var fanId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _bookmarksService.ToggleBookmarkAsync(postId, fanId);
            return this.ToActionResult(response);
        }
    }
}