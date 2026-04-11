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
            return await _context.Fans
                .AsNoTracking()
                .Where(f => f.Id == fanId)
                .Select(f => new FanProfileDto
                {
                    Id = f.Id,
                    Username = f.Username,
                    DisplayName = f.DisplayName,
                    Bio = f.Bio,
                    ProfilePicUrl = f.ProfilePicUrl,
                    FollowersCount = f.FollowersCount,
                    FollowingCount = f.FollowingCount,
                    Points = f.Points,
                    Posts = f.Posts
                        .OrderByDescending(p => p.CreatedAt)
                        .Select(p => new PostDto
                        {
                            Id = p.Id,
                            Caption = p.Caption,
                            MediaUrl = p.MediaUrl,
                            MediaType = p.MediaType.ToString(),
                            CreatedAt = p.CreatedAt,
                            LikeCount = p.LikeCount,
                            CommentCount = p.CommentCount,
                            BookmarkCount = p.BookmarkCount,
                            FanId = p.FanId,
                            FanDisplayName = f.DisplayName,
                            FanProfilePicUrl = f.ProfilePicUrl
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<FanProfileDto> UpdateProfileAsync(int fanId, UpdateFanProfileDto dto)
        {
            var fan = await _context.Fans.FirstOrDefaultAsync(f => f.Id == fanId);
            if (fan == null)
                throw new KeyNotFoundException("المشجع غير موجود.");

            var oldImagePath = fan.ProfilePicUrl;
            string? newImagePath = null;

            try
            {
                if (dto.ProfileImage != null)
                {
                    newImagePath = await _fileService.SaveFileAsync(dto.ProfileImage, "fans");
                    fan.ProfilePicUrl = newImagePath;
                }

                if (!string.IsNullOrWhiteSpace(dto.DisplayName))
                    fan.DisplayName = dto.DisplayName.Trim();

                fan.Bio = string.IsNullOrWhiteSpace(dto.Bio) ? null : dto.Bio.Trim();

                await _context.SaveChangesAsync();

                if (!string.IsNullOrWhiteSpace(oldImagePath) &&
                    !string.Equals(oldImagePath, fan.ProfilePicUrl, StringComparison.OrdinalIgnoreCase))
                {
                    _fileService.DeleteFile(oldImagePath);
                }

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
                    Posts = new List<PostDto>()
                };
            }
            catch
            {
                if (!string.IsNullOrWhiteSpace(newImagePath))
                {
                    _fileService.DeleteFile(newImagePath);
                }

                throw;
            }
        }

        public async Task<List<FanProfileDto>> SearchFansAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<FanProfileDto>();

            query = query.Trim();

            return await _context.Fans
                .AsNoTracking()
                .Where(f =>
                    EF.Functions.Like(f.DisplayName, $"%{query}%") ||
                    EF.Functions.Like(f.Username, $"%{query}%"))
                .OrderBy(f => f.DisplayName)
                .Take(20)
                .Select(f => new FanProfileDto
                {
                    Id = f.Id,
                    Username = f.Username,
                    DisplayName = f.DisplayName,
                    Bio = f.Bio,
                    ProfilePicUrl = f.ProfilePicUrl,
                    FollowersCount = f.FollowersCount,
                    FollowingCount = f.FollowingCount,
                    Points = f.Points,
                    Posts = new List<PostDto>()
                })
                .ToListAsync();
        }

        public async Task FollowFanAsync(int followerId, int followedFanId)
        {
            if (followerId == followedFanId)
                throw new InvalidOperationException("لا يمكن للمستخدم متابعة نفسه.");

            var follower = await _context.Fans.FirstOrDefaultAsync(f => f.Id == followerId);
            var followedFan = await _context.Fans.FirstOrDefaultAsync(f => f.Id == followedFanId);

            if (follower == null || followedFan == null)
                throw new KeyNotFoundException("أحد المستخدمين غير موجود.");

            var exists = await _context.Follows.AnyAsync(f =>
                f.FollowerId == followerId && f.FollowedFanId == followedFanId);

            if (exists)
                throw new InvalidOperationException("أنت تتابع هذا المستخدم بالفعل.");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var follow = new Follow
            {
                FollowerId = followerId,
                FollowedFanId = followedFanId,
                FollowDate = DateTime.UtcNow
            };

            await _context.Follows.AddAsync(follow);
            await _context.SaveChangesAsync();

            follower.FollowingCount = await _context.Follows.CountAsync(f => f.FollowerId == followerId);
            followedFan.FollowersCount = await _context.Follows.CountAsync(f => f.FollowedFanId == followedFanId);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        public async Task UnfollowFanAsync(int followerId, int followedFanId)
        {
            var follower = await _context.Fans.FirstOrDefaultAsync(f => f.Id == followerId);
            var followedFan = await _context.Fans.FirstOrDefaultAsync(f => f.Id == followedFanId);

            if (follower == null || followedFan == null)
                throw new KeyNotFoundException("أحد المستخدمين غير موجود.");

            var follow = await _context.Follows.FirstOrDefaultAsync(f =>
                f.FollowerId == followerId && f.FollowedFanId == followedFanId);

            if (follow == null)
                throw new InvalidOperationException("أنت لا تتابع هذا المستخدم.");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();

            follower.FollowingCount = await _context.Follows.CountAsync(f => f.FollowerId == followerId);
            followedFan.FollowersCount = await _context.Follows.CountAsync(f => f.FollowedFanId == followedFanId);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        public async Task<bool> IsFollowingAsync(int followerId, int followedFanId)
        {
            return await _context.Follows.AnyAsync(f =>
                f.FollowerId == followerId && f.FollowedFanId == followedFanId);
        }
    }
}