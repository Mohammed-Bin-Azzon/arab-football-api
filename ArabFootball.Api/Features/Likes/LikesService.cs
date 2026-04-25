using System.Net;
using ArabFootball.Api.Features.Likes.LikesDto;
using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Shared.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ArabFootball.Api.Features.Likes
{
    public class LikesService : ILikesService
    {
        private readonly AppDBContext _context;

        public LikesService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<LikeResultDto>> ToggleLikeAsync(int postId, int fanId)
        {
            try
            {
                var fanExists = await _context.Fans.AnyAsync(f => f.Id == fanId);
                if (!fanExists)
                {
                    return ApiResponse<LikeResultDto>.Fail(
                        HttpStatusCode.NotFound,
                        "المستخدم غير موجود.");
                }

                var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
                if (post == null)
                {
                    return ApiResponse<LikeResultDto>.Fail(
                        HttpStatusCode.NotFound,
                        "المنشور غير موجود.");
                }

                await using var transaction = await _context.Database.BeginTransactionAsync();

                var existingLike = await _context.Likes
                    .FirstOrDefaultAsync(l => l.PostId == postId && l.FanId == fanId);

                bool isLiked;

                if (existingLike != null)
                {
                    _context.Likes.Remove(existingLike);
                    isLiked = false;
                }
                else
                {
                    var newLike = new Like
                    {
                        PostId = postId,
                        FanId = fanId,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _context.Likes.AddAsync(newLike);
                    isLiked = true;
                }

                try
                {
                    await _context.SaveChangesAsync();

                    var actualLikeCount = await _context.Likes.CountAsync(l => l.PostId == postId);
                    post.LikeCount = actualLikeCount;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var result = new LikeResultDto
                    {
                        IsLiked = isLiked,
                        NewLikeCount = actualLikeCount
                    };

                    return ApiResponse<LikeResultDto>.Success(
                        result,
                        isLiked ? "تم تسجيل الإعجاب بنجاح." : "تم إزالة الإعجاب بنجاح.");
                }
                catch (DbUpdateException)
                {
                    await transaction.RollbackAsync();

                    return ApiResponse<LikeResultDto>.Fail(
                        HttpStatusCode.BadRequest,
                        "تعذر تنفيذ العملية بسبب تعارض في البيانات. أعد المحاولة.");
                }
            }
            catch (Exception)
            {
                return ApiResponse<LikeResultDto>.Fail(
                    HttpStatusCode.InternalServerError,
                    "حدث خطأ أثناء تنفيذ العملية.");
            }
        }
    }
}