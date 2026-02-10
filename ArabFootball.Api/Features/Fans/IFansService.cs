using ArabFootball.Api.Features.Fans.Dto;
using ArabFootball.Api.Features.Fans.FansDto;

namespace ArabFootball.Api.Features.Fans
{
    public interface IFansService
    {
        
        Task<FanProfileDto?> GetProfileAsync(int fanId);

        Task<bool> UpdateProfileAsync(int fanId, UpdateFanProfileDto dto);

        Task<List<FanProfileDto>> SearchFansAsync(string query);
    }
}