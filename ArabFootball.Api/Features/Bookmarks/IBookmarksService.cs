using ArabFootball.Api.Features.Bookmarks.BookmarksDto;
using ArabFootball.Shared.Helpers;

namespace ArabFootball.Api.Features.Bookmarks
{
    public interface IBookmarksService
    {
        Task<ApiResponse<BookmarkResultDto>> ToggleBookmarkAsync(int postId, int fanId);
    }
}