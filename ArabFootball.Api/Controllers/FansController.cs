using Microsoft.AspNetCore.Mvc;
using ArabFootball.Api.Features.Fans.FansDto;

namespace ArabFootball.Api.Features.Fans
{
    [ApiController]
    [Route("api/[controller]")] 
    public class FansController : ControllerBase
    {
        private readonly IFansService _fansService;

        public FansController(IFansService fansService)
        {
            _fansService = fansService;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfile(int id)
        {
            var profile = await _fansService.GetProfileAsync(id);

            if (profile == null)
            {
                return NotFound(new { message = "المشجع غير موجود" });
            }

            return Ok(profile);
        }


        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromForm] UpdateFanProfileDto dto)
        {

            var result = await _fansService.UpdateProfileAsync(id, dto);

            if (!result)
            {
                return BadRequest(new { message = "فشل تحديث البيانات، تأكد من صحة المعرف" });
            }

            return Ok(new { message = "تم تحديث الملف الشخصي بنجاح" });
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            var results = await _fansService.SearchFansAsync(query);
            return Ok(results);
        }
    }
}