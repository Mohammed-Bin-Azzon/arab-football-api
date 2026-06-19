using ArabFootball.Api.Shared.Entity;
using System.ComponentModel.DataAnnotations;

namespace ArabFootball.Api.Features.Reports.ReportDto
{
    public class CreateReportDto
    {
        [Required(ErrorMessage = "نوع الهدف مطلوب.")]
        public TargetType TargetType { get; set; }

        [Required(ErrorMessage = "رقم الهدف مطلوب.")]
        public int TargetId { get; set; }

        [Required(ErrorMessage = "سبب البلاغ مطلوب.")]
        public ReasonType Reason { get; set; }
    }
}
