using Microsoft.AspNetCore.Mvc;
using ArabFootball.Api.Features.Fans.Dtos;

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


        // POST: api/fans/{targetId}/follow?observerId=1
        [HttpPost("{targetId}/follow")]
        public async Task<IActionResult> Follow(int targetId, [FromQuery] int observerId)
        {

            var result = await _fansService.FollowFanAsync(observerId, targetId);

            if (!result)
                return BadRequest("لا يمكن إتمام المتابعة (تأكد من المعرفات أو أنك تتابعه بالفعل)");

            return Ok(new { message = "تمت المتابعة بنجاح" });
        }


        [HttpDelete("{targetId}/unfollow")]
        public async Task<IActionResult> Unfollow(int targetId, [FromQuery] int observerId)
        {
            var result = await _fansService.UnfollowFanAsync(observerId, targetId);

            if (!result)
                return BadRequest("أنت لا تتابع هذا المستخدم");

            return Ok(new { message = "تم إلغاء المتابعة" });
        }


        [HttpGet("{targetId}/is-following")]
        public async Task<IActionResult> CheckIsFollowing(int targetId, [FromQuery] int observerId)
        {
            var isFollowing = await _fansService.IsFollowingAsync(observerId, targetId);
            return Ok(new { isFollowing });
        }
    }
}