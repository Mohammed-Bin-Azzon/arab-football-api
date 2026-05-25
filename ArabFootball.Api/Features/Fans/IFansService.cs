using ArabFootball.Api.Features.Fans.Dtos;
using ArabFootball.Shared.Helpers;

namespace ArabFootball.Api.Features.Fans
{
    public interface IFansService
    {
        Task<ApiResponse<FanProfileDto>> GetProfileAsync(int fanId);
        Task<ApiResponse<FanBasicProfileDto>> UpdateProfileAsync(int fanId, UpdateFanProfileDto dto);
        Task<ApiResponse<List<FanBasicProfileDto>>> SearchFansAsync(string query);

        Task<ApiResponse<List<FanBasicProfileDto>>> GetFollowersAsync(int fanId);
        Task<ApiResponse<List<FanBasicProfileDto>>> GetFollowingAsync(int fanId);

        Task<ApiResponse<object>> FollowFanAsync(int followerId, int followedFanId);
        Task<ApiResponse<object>> UnfollowFanAsync(int followerId, int followedFanId);
        Task<ApiResponse<bool>> IsFollowingAsync(int followerId, int followedFanId);
    }
}