using ArabFootball.Api.Features.Bookmarks.BookmarksDto;
using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
using Microsoft.EntityFrameworkCore;

namespace ArabFootball.Api.Features.Bookmarks
{
    public class BookmarksService : IBookmarksService
    {
        private readonly AppDBContext _context;

        public BookmarksService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<BookmarkResultDto?> ToggleBookmarkAsync(int postId, int fanId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null) return null;

            var existingBookmark = await _context.Bookmarks
                .FirstOrDefaultAsync(b => b.PostId == postId && b.FanId == fanId);

            bool isBookmarked;

            if (existingBookmark != null)
            {
                _context.Bookmarks.Remove(existingBookmark);
                post.BookmarkCount--; 
                isBookmarked = false;
            }
            else
            {
                var newBookmark = new Bookmark
                {
                    PostId = postId,
                    FanId = fanId,
                    CreatedAt = DateTime.UtcNow
                };
                await _context.Bookmarks.AddAsync(newBookmark);
                post.BookmarkCount++; 
                isBookmarked = true;
            }

            await _context.SaveChangesAsync();

            return new BookmarkResultDto
            {
                IsBookmarked = isBookmarked,
                NewBookmarkCount = post.BookmarkCount
            };
        }
    }
}
