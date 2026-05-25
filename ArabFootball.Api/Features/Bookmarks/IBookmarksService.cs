using ArabFootball.Api.Features.Bookmarks.BookmarksDto;
using ArabFootball.Api.Features.Posts.Dtos;
using ArabFootball.Shared.Helpers;

namespace ArabFootball.Api.Features.Bookmarks
{
    public interface IBookmarksService
    {
        Task<ApiResponse<BookmarkResultDto>> ToggleBookmarkAsync(int postId, int fanId);
        Task<ApiResponse<List<PostDto>>> GetSavedPostsAsync(int fanId);
    }
}