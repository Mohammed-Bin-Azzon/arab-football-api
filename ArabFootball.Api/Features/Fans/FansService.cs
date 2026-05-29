using System.Net;
using ArabFootball.Api.Features.Fans.Dtos;
using ArabFootball.Api.Features.Posts.Dtos;
using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Api.Shared.Helpers;
using ArabFootball.Shared.Helpers;
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

        public async Task<ApiResponse<FanProfileDto>> GetProfileAsync(int fanId)
        {
            var profile = await _context.Fans
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
                    FavoriteTeamCode = f.FavoriteTeamCode,
                    FavoritePlayerCode = f.FavoritePlayerCode,
                    //Points = f.Points,
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

            if (profile == null)
            {
                return ApiResponse<FanProfileDto>.Error(
                    HttpStatusCode.NotFound,
                    "المشجع غير موجود.");
            }

            return ApiResponse<FanProfileDto>.Success(profile, "تم جلب الملف الشخصي بنجاح.");
        }

        public async Task<ApiResponse<FanBasicProfileDto>> UpdateProfileAsync(int fanId, UpdateFanProfileDto dto)
        {
            var oldImagePath = string.Empty;
            string? newImagePath = null;

            try
            {
                var fan = await _context.Fans.FirstOrDefaultAsync(f => f.Id == fanId);
                if (fan == null)
                {
                    return ApiResponse<FanBasicProfileDto>.Error(
                        HttpStatusCode.NotFound,
                        "المشجع غير موجود.");
                }

                oldImagePath = fan.ProfilePicUrl ?? string.Empty;

                if (dto.ProfileImage != null)
                {
                    newImagePath = await _fileService.SaveFileAsync(dto.ProfileImage, "fans");
                    fan.ProfilePicUrl = newImagePath;
                }

                if (!string.IsNullOrWhiteSpace(dto.DisplayName))
                {
                    fan.DisplayName = dto.DisplayName.Trim();
                }

                fan.Bio = string.IsNullOrWhiteSpace(dto.Bio) ? null : dto.Bio.Trim();

                if (dto.FavoriteTeamCode != null)
                {
                    fan.FavoriteTeamCode = string.IsNullOrWhiteSpace(dto.FavoriteTeamCode)
                        ? null
                        : dto.FavoriteTeamCode.Trim().ToLowerInvariant();
                }

                if (dto.FavoritePlayerCode != null)
                {
                    fan.FavoritePlayerCode = string.IsNullOrWhiteSpace(dto.FavoritePlayerCode)
                        ? null
                        : dto.FavoritePlayerCode.Trim().ToLowerInvariant();
                }

                await _context.SaveChangesAsync();

                if (!string.IsNullOrWhiteSpace(oldImagePath) &&
                    !string.Equals(oldImagePath, fan.ProfilePicUrl, StringComparison.OrdinalIgnoreCase))
                {
                    _fileService.DeleteFile(oldImagePath);
                }

                var result = new FanBasicProfileDto
                {
                    Id = fan.Id,
                    Username = fan.Username,
                    DisplayName = fan.DisplayName,
                    Bio = fan.Bio,
                    ProfilePicUrl = fan.ProfilePicUrl,
                    FavoriteTeamCode = fan.FavoriteTeamCode,
                    FavoritePlayerCode = fan.FavoritePlayerCode,
                    FollowersCount = fan.FollowersCount,
                    FollowingCount = fan.FollowingCount
                };

                return ApiResponse<FanBasicProfileDto>.Success(result, "تم تحديث الملف الشخصي بنجاح.");
            }
            catch (Exception)
            {
                if (!string.IsNullOrWhiteSpace(newImagePath))
                {
                    _fileService.DeleteFile(newImagePath);
                }

                return ApiResponse<FanBasicProfileDto>.Error(
                    HttpStatusCode.InternalServerError,
                    "حدث خطأ أثناء تحديث الملف الشخصي.");
            }
        }

        public async Task<ApiResponse<List<FanBasicProfileDto>>> SearchFansAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return ApiResponse<List<FanBasicProfileDto>>.Success(
                    new List<FanBasicProfileDto>(),
                    "لا توجد نتائج لأن عبارة البحث فارغة.");
            }

            query = query.Trim();

            var results = await _context.Fans
                .AsNoTracking()
                .Where(f =>
                    EF.Functions.Like(f.DisplayName, $"%{query}%") ||
                    EF.Functions.Like(f.Username, $"%{query}%"))
                .OrderBy(f => f.DisplayName)
                .Take(20)
                .Select(f => new FanBasicProfileDto
                {
                    Id = f.Id,
                    Username = f.Username,
                    DisplayName = f.DisplayName,
                    Bio = f.Bio,
                    ProfilePicUrl = f.ProfilePicUrl,
                    FollowersCount = f.FollowersCount,
                    FollowingCount = f.FollowingCount,
                    //Points = f.Points
                })
                .ToListAsync();

            return ApiResponse<List<FanBasicProfileDto>>.Success(results, "تم جلب نتائج البحث بنجاح.");
        }

        public async Task<ApiResponse<List<FanBasicProfileDto>>> GetFollowersAsync(int fanId)
        {
            var fanExists = await _context.Fans
                .AsNoTracking()
                .AnyAsync(f => f.Id == fanId);

            if (!fanExists)
            {
                return ApiResponse<List<FanBasicProfileDto>>.Error(
                    HttpStatusCode.NotFound,
                    "المشجع غير موجود.");
            }

            var followers = await _context.Follows
                .AsNoTracking()
                .Where(f => f.FollowedFanId == fanId)
                .OrderByDescending(f => f.FollowDate)
                .Select(f => new FanBasicProfileDto
                {
                    Id = f.Follower.Id,
                    Username = f.Follower.Username,
                    DisplayName = f.Follower.DisplayName,
                    Bio = f.Follower.Bio,
                    ProfilePicUrl = f.Follower.ProfilePicUrl,
                    FollowersCount = f.Follower.FollowersCount,
                    FollowingCount = f.Follower.FollowingCount,
                    //Points = f.Follower.Points
                })
                .ToListAsync();

            return ApiResponse<List<FanBasicProfileDto>>.Success(
                followers,
                "تم جلب المتابعين بنجاح.");
        }

        public async Task<ApiResponse<List<FanBasicProfileDto>>> GetFollowingAsync(int fanId)
        {
            var fanExists = await _context.Fans
                .AsNoTracking()
                .AnyAsync(f => f.Id == fanId);

            if (!fanExists)
            {
                return ApiResponse<List<FanBasicProfileDto>>.Error(
                    HttpStatusCode.NotFound,
                    "المشجع غير موجود.");
            }

            var following = await _context.Follows
                .AsNoTracking()
                .Where(f => f.FollowerId == fanId)
                .OrderByDescending(f => f.FollowDate)
                .Select(f => new FanBasicProfileDto
                {
                    Id = f.FollowedFan.Id,
                    Username = f.FollowedFan.Username,
                    DisplayName = f.FollowedFan.DisplayName,
                    Bio = f.FollowedFan.Bio,
                    ProfilePicUrl = f.FollowedFan.ProfilePicUrl,
                    FollowersCount = f.FollowedFan.FollowersCount,
                    FollowingCount = f.FollowedFan.FollowingCount,
                    //Points = f.FollowedFan.Points
                })
                .ToListAsync();

            return ApiResponse<List<FanBasicProfileDto>>.Success(
                following,
                "تم جلب قائمة المتابعة بنجاح.");
        }

        public async Task<ApiResponse<object>> FollowFanAsync(int followerId, int followedFanId)
        {
            try
            {
                if (followerId == followedFanId)
                {
                    return ApiResponse<object>.Error(
                        HttpStatusCode.BadRequest,
                        "لا يمكن للمستخدم متابعة نفسه.");
                }

                var follower = await _context.Fans.FirstOrDefaultAsync(f => f.Id == followerId);
                var followedFan = await _context.Fans.FirstOrDefaultAsync(f => f.Id == followedFanId);

                if (follower == null || followedFan == null)
                {
                    return ApiResponse<object>.Error(
                        HttpStatusCode.NotFound,
                        "أحد المستخدمين غير موجود.");
                }

                var exists = await _context.Follows.AnyAsync(f =>
                    f.FollowerId == followerId && f.FollowedFanId == followedFanId);

                if (exists)
                {
                    return ApiResponse<object>.Error(
                        HttpStatusCode.BadRequest,
                        "أنت تتابع هذا المستخدم بالفعل.");
                }

                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
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

                    return ApiResponse<object>.Success(null, "تمت المتابعة بنجاح.");
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return ApiResponse<object>.Error(
                        HttpStatusCode.InternalServerError,
                        "حدث خطأ أثناء تنفيذ المتابعة.");
                }
            }
            catch (Exception)
            {
                return ApiResponse<object>.Error(
                    HttpStatusCode.InternalServerError,
                    "حدث خطأ أثناء تنفيذ المتابعة.");
            }
        }

        public async Task<ApiResponse<object>> UnfollowFanAsync(int followerId, int followedFanId)
        {
            try
            {
                var follower = await _context.Fans.FirstOrDefaultAsync(f => f.Id == followerId);
                var followedFan = await _context.Fans.FirstOrDefaultAsync(f => f.Id == followedFanId);

                if (follower == null || followedFan == null)
                {
                    return ApiResponse<object>.Error(
                        HttpStatusCode.NotFound,
                        "أحد المستخدمين غير موجود.");
                }

                var follow = await _context.Follows.FirstOrDefaultAsync(f =>
                    f.FollowerId == followerId && f.FollowedFanId == followedFanId);

                if (follow == null)
                {
                    return ApiResponse<object>.Error(
                        HttpStatusCode.BadRequest,
                        "أنت لا تتابع هذا المستخدم.");
                }

                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    _context.Follows.Remove(follow);
                    await _context.SaveChangesAsync();

                    follower.FollowingCount = await _context.Follows.CountAsync(f => f.FollowerId == followerId);
                    followedFan.FollowersCount = await _context.Follows.CountAsync(f => f.FollowedFanId == followedFanId);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return ApiResponse<object>.Success(null, "تم إلغاء المتابعة بنجاح.");
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return ApiResponse<object>.Error(
                        HttpStatusCode.InternalServerError,
                        "حدث خطأ أثناء إلغاء المتابعة.");
                }
            }
            catch (Exception)
            {
                return ApiResponse<object>.Error(
                    HttpStatusCode.InternalServerError,
                    "حدث خطأ أثناء إلغاء المتابعة.");
            }
        }

        public async Task<ApiResponse<bool>> IsFollowingAsync(int followerId, int followedFanId)
        {
            var isFollowing = await _context.Follows.AnyAsync(f =>
                f.FollowerId == followerId && f.FollowedFanId == followedFanId);

            return ApiResponse<bool>.Success(isFollowing, "تم التحقق من حالة المتابعة بنجاح.");
        }
    }
}