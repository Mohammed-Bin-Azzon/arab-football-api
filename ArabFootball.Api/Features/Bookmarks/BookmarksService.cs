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

        public async Task<BookmarkResultDto> ToggleBookmarkAsync(int postId, int fanId)
        {
            var fanExists = await _context.Fans.AnyAsync(f => f.Id == fanId);
            if (!fanExists)
                throw new InvalidOperationException("المستخدم غير موجود.");

            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null)
                throw new KeyNotFoundException("المنشور غير موجود.");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var existingBookmark = await _context.Bookmarks
                .FirstOrDefaultAsync(b => b.PostId == postId && b.FanId == fanId);

            bool isBookmarked;

            if (existingBookmark != null)
            {
                _context.Bookmarks.Remove(existingBookmark);
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
                isBookmarked = true;
            }

            try
            {
                await _context.SaveChangesAsync();

                var actualBookmarkCount = await _context.Bookmarks.CountAsync(b => b.PostId == postId);
                post.BookmarkCount = actualBookmarkCount;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new BookmarkResultDto
                {
                    IsBookmarked = isBookmarked,
                    NewBookmarkCount = actualBookmarkCount
                };
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("تعذر تنفيذ العملية بسبب تعارض في البيانات. أعد المحاولة.");
            }
        }
    }
}