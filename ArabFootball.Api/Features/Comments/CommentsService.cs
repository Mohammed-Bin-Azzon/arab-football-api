using ArabFootball.Api.Features.Comments.CommentsDto;
using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
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

        public async Task<CommentDto> AddCommentAsync(int fanId, CreateCommentDto dto)
        {
            var content = dto.Content?.Trim();
            if (string.IsNullOrWhiteSpace(content))
                throw new InvalidOperationException("محتوى التعليق مطلوب.");

            var fan = await _context.Fans
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == fanId);

            if (fan == null)
                throw new InvalidOperationException("المستخدم غير موجود.");

            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == dto.PostId);

            if (post == null)
                throw new KeyNotFoundException("المنشور غير موجود.");

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

                return new CommentDto
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    CreatedAt = comment.CreatedAt,
                    FanId = fan.Id,
                    FanName = string.IsNullOrWhiteSpace(fan.DisplayName) ? fan.Username : fan.DisplayName,
                    FanProfilePic = fan.ProfilePicUrl
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<CommentDto>> GetPostCommentsAsync(int postId)
        {
            return await _context.Comments
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
        }
    }
}