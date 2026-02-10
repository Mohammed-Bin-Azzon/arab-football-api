using ArabFootball.Api.Features.Fans.Dto;
using ArabFootball.Api.Features.Fans.FansDto;
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
            var fan = await _context.Fans.FirstOrDefaultAsync(f => f.Id == fanId);
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
                Points = fan.Points
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
            fan.IsPrivate = dto.IsPrivate;

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
    }
}