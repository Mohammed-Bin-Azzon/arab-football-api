using ArabFootball.Api.Features.Fans.Dtos;

namespace ArabFootball.Api.Features.Fans
{
    public interface IFansService
    {
        Task<FanProfileDto?> GetProfileAsync(int fanId);
        Task<FanProfileDto> UpdateProfileAsync(int fanId, UpdateFanProfileDto dto);
        Task<List<FanProfileDto>> SearchFansAsync(string query);

        Task FollowFanAsync(int followerId, int followedFanId);
        Task UnfollowFanAsync(int followerId, int followedFanId);
        Task<bool> IsFollowingAsync(int followerId, int followedFanId);
    }
}