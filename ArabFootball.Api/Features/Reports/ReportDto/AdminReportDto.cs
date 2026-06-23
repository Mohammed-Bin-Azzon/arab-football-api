using ArabFootball.Api.Shared.Entity;

namespace ArabFootball.Api.Features.Reports.ReportDto
{
    public class AdminReportDto
    {
        public int Id { get; set; }

        public int ReporterId { get; set; }
        public string ReporterUsername { get; set; } = string.Empty;

        public int? AdminId { get; set; }
        public string? AdminUsername { get; set; }

        public TargetType TargetType { get; set; }
        public string TargetTypeName { get; set; } = string.Empty;

        public int TargetId { get; set; }

        public ReasonType Reason { get; set; }
        public string ReasonName { get; set; } = string.Empty;

        public ReportStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}