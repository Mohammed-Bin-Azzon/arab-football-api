using ArabFootball.Api.Features.Dashboard.DashboardDto;
using ArabFootball.Shared.Helpers;

namespace ArabFootball.Api.Features.Dashboard
{
    public interface IDashboardService
    {
        Task<ApiResponse<DashboardStatsDto>> GetStatsAsync();
    }
}