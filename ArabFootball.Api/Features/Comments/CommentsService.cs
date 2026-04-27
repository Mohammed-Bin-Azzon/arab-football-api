using System.Net;
using ArabFootball.Api.Features.Comments.CommentsDto;
using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Shared.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ArabFootball.Api.Features.Comments
{
    public class CommentsService : ICommentsService
    {
        private readonly AppDBContext _context;

        public CommentsService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<CommentDto>> AddCommentAsync(int fanId, CreateCommentDto dto)
        {
            try
            {
                var content = dto.Content?.Trim();
                if (string.IsNullOrWhiteSpace(content))
                {
                    return ApiResponse<CommentDto>.Error(
                        HttpStatusCode.BadRequest,
                        "محتوى التعليق مطلوب.");
                }

                var fan = await _context.Fans
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Id == fanId);

                if (fan == null)
                {
                    return ApiResponse<CommentDto>.Error(
                        HttpStatusCode.NotFound,
                        "المستخدم غير موجود.");
                }

                var post = await _context.Posts
                    .FirstOrDefaultAsync(p => p.Id == dto.PostId);

                if (post == null)
                {
                    return ApiResponse<CommentDto>.Error(
                        HttpStatusCode.NotFound,
                        "المنشور غير موجود.");
                }

                await using var transaction = await _context.Database.BeginTransactionAsync();

                var comment = new Comment
                {
                    Content = content,
                    PostId = dto.PostId,
                    FanId = fanId,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Comments.AddAsync(comment);

                try
                {
                    await _context.SaveChangesAsync();

                    var actualCommentCount = await _context.Comments.CountAsync(c => c.PostId == dto.PostId);
                    post.CommentCount = actualCommentCount;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var result = new CommentDto
                    {
                        Id = comment.Id,
                        Content = comment.Content,
                        CreatedAt = comment.CreatedAt,
                        FanId = fan.Id,
                        FanName = string.IsNullOrWhiteSpace(fan.DisplayName) ? fan.Username : fan.DisplayName,
                        FanProfilePic = fan.ProfilePicUrl
                    };

                    return ApiResponse<CommentDto>.Success(result, "تمت إضافة التعليق بنجاح.");
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return ApiResponse<CommentDto>.Error(
                        HttpStatusCode.InternalServerError,
                        "حدث خطأ أثناء إضافة التعليق.");
                }
            }
            catch (Exception)
            {
                return ApiResponse<CommentDto>.Error(
                    HttpStatusCode.InternalServerError,
                    "حدث خطأ غير متوقع أثناء إضافة التعليق.");
            }
        }

        public async Task<ApiResponse<List<CommentDto>>> GetPostCommentsAsync(int postId)
        {
            try
            {
                var postExists = await _context.Posts
                    .AsNoTracking()
                    .AnyAsync(p => p.Id == postId);

                if (!postExists)
                {
                    return ApiResponse<List<CommentDto>>.Error(
                        HttpStatusCode.NotFound,
                        "المنشور غير موجود.");
                }

                var comments = await _context.Comments
                    .AsNoTracking()
                    .Where(c => c.PostId == postId)
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new CommentDto
                    {
                        Id = c.Id,
                        Content = c.Content,
                        CreatedAt = c.CreatedAt,
                        FanId = c.FanId,
                        FanName = string.IsNullOrWhiteSpace(c.Fan.DisplayName) ? c.Fan.Username : c.Fan.DisplayName,
                        FanProfilePic = c.Fan.ProfilePicUrl
                    })
                    .ToListAsync();

                return ApiResponse<List<CommentDto>>.Success(comments, "تم جلب التعليقات بنجاح.");
            }
            catch (Exception)
            {
                return ApiResponse<List<CommentDto>>.Error(
                    HttpStatusCode.InternalServerError,
                    "حدث خطأ أثناء جلب التعليقات.");
            }
        }
    }
}