using ArabFootball.Api.Features.Fans.Dtos;
using ArabFootball.Api.Features.Posts.Dtos;
using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Api.Shared.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ArabFootball.Api.Features.Fans
{
    public class FansService : IFansService
    {
        private readonly AppDBContext _context;
        private readonly IFileService _fileService;

        public FansService(AppDBContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }


        public async Task<FanProfileDto?> GetProfileAsync(int fanId)
        {
            
            var fan = await _context.Fans
                .Include(f => f.Posts)
                .FirstOrDefaultAsync(f => f.Id == fanId);

            if (fan == null) return null;

            return new FanProfileDto
            {
                Id = fan.Id,
                Username = fan.Username,
                DisplayName = fan.DisplayName,
                Bio = fan.Bio,
                ProfilePicUrl = fan.ProfilePicUrl,
                FollowersCount = fan.FollowersCount,
                FollowingCount = fan.FollowingCount,
                Points = fan.Points,

                Posts = fan.Posts.OrderByDescending(p => p.CreatedAt).Select(p => new PostDto
                {
                    Id = p.Id,
                    Caption = p.Caption,
                    MediaUrl = p.MediaUrl,
                    MediaType = p.MediaType.ToString(),
                    CreatedAt = p.CreatedAt,
                    LikeCount = p.LikeCount,
                    CommentCount = p.CommentCount,
                }).ToList()
            };
        }

        public async Task<bool> UpdateProfileAsync(int fanId, UpdateFanProfileDto dto)
        {
            var fan = await _context.Fans.FindAsync(fanId);
            if (fan == null) return false;

            
            if (dto.ProfileImage != null)
            {
                if (!string.IsNullOrEmpty(fan.ProfilePicUrl))
                {
                    _fileService.DeleteFile(fan.ProfilePicUrl, "fans");
                }


                string newFileName = await _fileService.SaveFileAsync(dto.ProfileImage, "fans");
                fan.ProfilePicUrl = newFileName;
            }

            if (!string.IsNullOrWhiteSpace(dto.DisplayName))
                fan.DisplayName = dto.DisplayName;

            fan.Bio = dto.Bio;

            _context.Fans.Update(fan);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<FanProfileDto>> SearchFansAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return new List<FanProfileDto>();

            var fans = await _context.Fans
                .Where(f => f.DisplayName.Contains(query) || f.Username.Contains(query))
                .AsNoTracking()
                .ToListAsync();


            return fans.Select(f => new FanProfileDto
            {
                Id = f.Id,
                Username = f.Username,
                DisplayName = f.DisplayName,
                Bio = f.Bio,
                ProfilePicUrl = f.ProfilePicUrl,
                FollowersCount = f.FollowersCount,
                FollowingCount = f.FollowingCount,
                Points = f.Points
            }).ToList();
        }

        public async Task<bool> FollowFanAsync(int observerId, int targetId)
        {
            
            if (observerId == targetId) return false;

            var existingFollow = await _context.Follows.FindAsync(observerId, targetId);
            if (existingFollow != null) return false; 

            
            var follow = new Follow
            {
                ObserverId = observerId,
                TargetId = targetId
            };

            _context.Follows.Add(follow);


            var observer = await _context.Fans.FindAsync(observerId);
            var target = await _context.Fans.FindAsync(targetId);

            if (observer != null) observer.FollowingCount++; 
            if (target != null) target.FollowersCount++;     

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UnfollowFanAsync(int observerId, int targetId)
        {
            
            var follow = await _context.Follows.FindAsync(observerId, targetId);
            if (follow == null) return false; 

            
            _context.Follows.Remove(follow);

            
            var observer = await _context.Fans.FindAsync(observerId);
            var target = await _context.Fans.FindAsync(targetId);

            if (observer != null) observer.FollowingCount--;
            if (target != null) target.FollowersCount--;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> IsFollowingAsync(int observerId, int targetId)
        {
            return await _context.Follows.AnyAsync(f => f.ObserverId == observerId && f.TargetId == targetId);
        }
    }
}