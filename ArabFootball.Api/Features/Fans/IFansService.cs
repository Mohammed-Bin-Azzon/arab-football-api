using ArabFootball.Api.Features.Fans.Dtos;

namespace ArabFootball.Api.Features.Fans
{
    public interface IFansService
    {
        
        Task<FanProfileDto?> GetProfileAsync(int fanId);
        Task<bool> UpdateProfileAsync(int fanId, UpdateFanProfileDto dto);
        Task<List<FanProfileDto>> SearchFansAsync(string query);

        
        Task<bool> FollowFanAsync(int observerId, int targetId);
        Task<bool> UnfollowFanAsync(int observerId, int targetId);
        Task<bool> IsFollowingAsync(int observerId, int targetId); 
    }
}