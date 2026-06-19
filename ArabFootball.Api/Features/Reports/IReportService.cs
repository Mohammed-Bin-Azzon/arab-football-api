using ArabFootball.Api.Features.Reports.ReportDto;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Shared.Helpers;

namespace ArabFootball.Api.Features.Reports
{
    public interface IReportService
    {
        Task<ApiResponse<AdminReportDto>> CreateReportAsync(
            CreateReportDto dto,
            int reporterId);

        Task<ApiResponse<List<AdminReportDto>>> GetReportsForAdminAsync(
            ReportStatus? status = null);

        Task<ApiResponse<bool>> MarkAsReviewedAsync(
            int reportId,
            int adminId);

        Task<ApiResponse<bool>> RejectReportAsync(
            int reportId,
            int adminId);
    }
}