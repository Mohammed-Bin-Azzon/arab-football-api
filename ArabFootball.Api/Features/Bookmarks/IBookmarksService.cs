using ArabFootball.Api.Features.Bookmarks.BookmarksDto;

namespace ArabFootball.Api.Features.Bookmarks
{
    public interface IBookmarksService
    {
        Task<BookmarkResultDto?> ToggleBookmarkAsync(int postId, int fanId);
    }
}
