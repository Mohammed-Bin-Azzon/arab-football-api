using System.Security.Claims;
using api_training.Controllers;
using ArabFootball.Api.Features.Bookmarks;
using ArabFootball.Api.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArabFootball.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookmarksController : AppControllerBase    
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
            return Response(await _bookmarksService.ToggleBookmarkAsync(postId, fanId));
        }
    }
}