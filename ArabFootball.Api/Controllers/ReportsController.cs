using System.Security.Claims;
using ArabFootball.Api.Features.Reports;
using ArabFootball.Api.Features.Reports.ReportDto;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Api.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArabFootball.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController : AppControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReportDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reporterId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            return Response(
                await _reportService.CreateReportAsync(dto, reporterId)
            );
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<IActionResult> GetForAdmin(
            [FromQuery] ReportStatus? status = null)
        {
            return Response(
                await _reportService.GetReportsForAdminAsync(status)
            );
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/review")]
        public async Task<IActionResult> MarkAsReviewed([FromRoute] int id)
        {
            var adminId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            return Response(
                await _reportService.MarkAsReviewedAsync(id, adminId)
            );
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/reject")]
        public async Task<IActionResult> Reject([FromRoute] int id)
        {
            var adminId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            return Response(
                await _reportService.RejectReportAsync(id, adminId)
            );
        }
    }
}