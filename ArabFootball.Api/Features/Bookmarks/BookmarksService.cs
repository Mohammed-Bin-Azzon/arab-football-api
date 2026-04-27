using System.Net;
using ArabFootball.Api.Features.Bookmarks.BookmarksDto;
using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Shared.Helpers;
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

        public async Task<ApiResponse<BookmarkResultDto>> ToggleBookmarkAsync(int postId, int fanId)
        {
            try
            {
                var fanExists = await _context.Fans.AnyAsync(f => f.Id == fanId);
                if (!fanExists)
                {
                    return ApiResponse<BookmarkResultDto>.Error(
                        HttpStatusCode.NotFound,
                        "المستخدم غير موجود.");
                }

                var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
                if (post == null)
                {
                    return ApiResponse<BookmarkResultDto>.Error(
                        HttpStatusCode.NotFound,
                        "المنشور غير موجود.");
                }

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

                    var result = new BookmarkResultDto
                    {
                        IsBookmarked = isBookmarked,
                        NewBookmarkCount = actualBookmarkCount
                    };

                    return ApiResponse<BookmarkResultDto>.Success(
                        result,
                        isBookmarked ? "تم حفظ المنشور بنجاح." : "تمت إزالة المنشور من المحفوظات بنجاح.");
                }
                catch (DbUpdateException)
                {
                    await transaction.RollbackAsync();

                    return ApiResponse<BookmarkResultDto>.Error(
                        HttpStatusCode.BadRequest,
                        "تعذر تنفيذ العملية بسبب تعارض في البيانات. أعد المحاولة.");
                }
            }
            catch (Exception)
            {
                return ApiResponse<BookmarkResultDto>.Error(
                    HttpStatusCode.InternalServerError,
                    "حدث خطأ أثناء تنفيذ العملية.");
            }
        }
    }
}