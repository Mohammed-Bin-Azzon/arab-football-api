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

        public async Task<CommentDto?> AddCommentAsync(CreateCommentDto dto)
        {
            var post = await _context.Posts.FindAsync(dto.PostId);
            if (post == null) return null;

            var comment = new Comment
            {
                Content = dto.Content,
                PostId = dto.PostId,
                FanId = dto.FanId,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Comments.AddAsync(comment);

            
            post.CommentCount++;

            await _context.SaveChangesAsync();

            
            var fan = await _context.Fans.FindAsync(dto.FanId);

            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                FanId = fan.Id,
                FanName = fan.DisplayName ?? fan.Username,
                FanProfilePic = fan.ProfilePicUrl
            };
        }

        public async Task<List<CommentDto>> GetPostCommentsAsync(int postId)
        {
            return await _context.Comments
                .Include(c => c.Fan)
                .Where(c => c.PostId == postId)
                .OrderByDescending(c => c.CreatedAt) 
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    FanId = c.FanId,
                    FanName = c.Fan.DisplayName ?? c.Fan.Username,
                    FanProfilePic = c.Fan.ProfilePicUrl
                }).ToListAsync();
        }
    }
}
