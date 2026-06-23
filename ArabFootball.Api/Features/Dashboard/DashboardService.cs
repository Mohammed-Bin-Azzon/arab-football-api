using ArabFootball.Api.Features.Dashboard.DashboardDto;
using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Shared.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ArabFootball.Api.Features.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDBContext _context;

        public DashboardService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<DashboardStatsDto>> GetStatsAsync()
        {
            var stats = new DashboardStatsDto
            {
                UsersCount = await _context.Fans.CountAsync(),

                MatchesCount = await _context.Matches.CountAsync(),

                PostsCount = await _context.Posts.CountAsync(),

                ReportsCount = await _context.Reports.CountAsync(),

                PendingReportsCount = await _context.Reports
                    .CountAsync(r => r.Status == ReportStatus.Pending)
            };

            return ApiResponse<DashboardStatsDto>.Success(
                stats,
                "تم جلب إحصائيات لوحة التحكم بنجاح."
            );
        }
    }
}