using ArabFootball.Api.Features.Likes.LikesDto;
using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
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

        public async Task<LikeResultDto?> ToggleLikeAsync(int postId, int fanId)
        {

            var post = await _context.Posts.FindAsync(postId);
            if (post == null) return null; 


            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.PostId == postId && l.FanId == fanId);

            bool isLiked;

            if (existingLike != null)
            {

                _context.Likes.Remove(existingLike);
                post.LikeCount--; 
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
                post.LikeCount++; 
                isLiked = true;
            }


            await _context.SaveChangesAsync();

            return new LikeResultDto
            {
                IsLiked = isLiked,
                NewLikeCount = post.LikeCount
            };
        }
    }
}
