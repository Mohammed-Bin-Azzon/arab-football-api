using System.Net;
using ArabFootball.Api.Features.Reports.ReportDto;
using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Shared.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ArabFootball.Api.Features.Reports
{
    public class ReportService : IReportService
    {
        private readonly AppDBContext _context;

        public ReportService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<AdminReportDto>> CreateReportAsync(
            CreateReportDto dto,
            int reporterId)
        {
            var reporterExists = await _context.Fans
                .AnyAsync(f => f.Id == reporterId);

            if (!reporterExists)
            {
                return ApiResponse<AdminReportDto>.Error(
                    HttpStatusCode.NotFound,
                    "المستخدم المبلّغ غير موجود.");
            }

            if (!Enum.IsDefined(typeof(TargetType), dto.TargetType))
            {
                return ApiResponse<AdminReportDto>.Error(
                    HttpStatusCode.BadRequest,
                    "نوع الهدف غير صحيح.");
            }

            if (!Enum.IsDefined(typeof(ReasonType), dto.Reason))
            {
                return ApiResponse<AdminReportDto>.Error(
                    HttpStatusCode.BadRequest,
                    "سبب البلاغ غير صحيح.");
            }

            var targetExists = await TargetExistsAsync(dto.TargetType, dto.TargetId);

            if (!targetExists)
            {
                return ApiResponse<AdminReportDto>.Error(
                    HttpStatusCode.NotFound,
                    "العنصر المراد الإبلاغ عنه غير موجود.");
            }

            var duplicateExists = await _context.Reports.AnyAsync(r =>
                r.ReporterId == reporterId &&
                r.TargetType == dto.TargetType &&
                r.TargetId == dto.TargetId &&
                r.Status == ReportStatus.Pending);

            if (duplicateExists)
            {
                return ApiResponse<AdminReportDto>.Error(
                    HttpStatusCode.BadRequest,
                    "تم إرسال بلاغ على هذا العنصر مسبقاً.");
            }

            var report = new Report
            {
                ReporterId = reporterId,
                TargetType = dto.TargetType,
                TargetId = dto.TargetId,
                Reason = dto.Reason,
                Status = ReportStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Reports.AddAsync(report);
            await _context.SaveChangesAsync();

            var result = await BuildAdminReportDtoAsync(report.Id);

            return ApiResponse<AdminReportDto>.Success(
                result!,
                "تم إرسال البلاغ بنجاح.");
        }

        public async Task<ApiResponse<List<AdminReportDto>>> GetReportsForAdminAsync(
            ReportStatus? status = null)
        {
            var query = _context.Reports
                .AsNoTracking()
                .Include(r => r.Reporter)
                .Include(r => r.Admin)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status.Value);
            }

            var reports = await query
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new AdminReportDto
                {
                    Id = r.Id,

                    ReporterId = r.ReporterId,
                    ReporterUsername = r.Reporter.Username,

                    AdminId = r.AdminId,
                    AdminUsername = r.Admin == null ? null : r.Admin.Username,

                    TargetType = r.TargetType,
                    TargetTypeName = r.TargetType.ToString(),

                    TargetId = r.TargetId,

                    Reason = r.Reason,
                    ReasonName = r.Reason.ToString(),

                    Status = r.Status,
                    StatusName = r.Status.ToString(),

                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return ApiResponse<List<AdminReportDto>>.Success(
                reports,
                reports.Any() ? "تم جلب البلاغات بنجاح." : "لا توجد بلاغات.");
        }

        public async Task<ApiResponse<bool>> MarkAsReviewedAsync(
            int reportId,
            int adminId)
        {
            var report = await _context.Reports
                .FirstOrDefaultAsync(r => r.Id == reportId);

            if (report == null)
            {
                return ApiResponse<bool>.Error(
                    HttpStatusCode.NotFound,
                    "البلاغ غير موجود.");
            }

            var adminExists = await _context.Admins.AnyAsync(a => a.Id == adminId);

            if (!adminExists)
            {
                return ApiResponse<bool>.Error(
                    HttpStatusCode.BadRequest,
                    "المشرف غير موجود.");
            }

            report.Status = ReportStatus.Reviewed;
            report.AdminId = adminId;

            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(
                true,
                "تمت مراجعة البلاغ بنجاح.");
        }

        public async Task<ApiResponse<bool>> RejectReportAsync(
            int reportId,
            int adminId)
        {
            var report = await _context.Reports
                .FirstOrDefaultAsync(r => r.Id == reportId);

            if (report == null)
            {
                return ApiResponse<bool>.Error(
                    HttpStatusCode.NotFound,
                    "البلاغ غير موجود.");
            }

            var adminExists = await _context.Admins.AnyAsync(a => a.Id == adminId);

            if (!adminExists)
            {
                return ApiResponse<bool>.Error(
                    HttpStatusCode.BadRequest,
                    "المشرف غير موجود.");
            }

            report.Status = ReportStatus.Rejected;
            report.AdminId = adminId;

            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(
                true,
                "تم رفض البلاغ بنجاح.");
        }

        private async Task<bool> TargetExistsAsync(
            TargetType targetType,
            int targetId)
        {
            return targetType switch
            {
                TargetType.Fan => await _context.Fans.AnyAsync(f => f.Id == targetId),

                TargetType.Post => await _context.Posts.AnyAsync(p => p.Id == targetId),

                TargetType.Comment => await _context.Comments.AnyAsync(c => c.Id == targetId),

                TargetType.Message => await _context.Messages.AnyAsync(m => m.MessageId == targetId),

                _ => false
            };
        }

        private async Task<AdminReportDto?> BuildAdminReportDtoAsync(int reportId)
        {
            return await _context.Reports
                .AsNoTracking()
                .Include(r => r.Reporter)
                .Include(r => r.Admin)
                .Where(r => r.Id == reportId)
                .Select(r => new AdminReportDto
                {
                    Id = r.Id,

                    ReporterId = r.ReporterId,
                    ReporterUsername = r.Reporter.Username,

                    AdminId = r.AdminId,
                    AdminUsername = r.Admin == null ? null : r.Admin.Username,

                    TargetType = r.TargetType,
                    TargetTypeName = r.TargetType.ToString(),

                    TargetId = r.TargetId,

                    Reason = r.Reason,
                    ReasonName = r.Reason.ToString(),

                    Status = r.Status,
                    StatusName = r.Status.ToString(),

                    CreatedAt = r.CreatedAt
                })
                .FirstOrDefaultAsync();
        }
    }
}