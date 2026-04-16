using System.Security.Claims;
using ArabFootball.Api.Features.Fans;
using ArabFootball.Api.Features.Fans.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArabFootball.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
                return NotFound(new { message = "المشجع غير موجود." });

            return Ok(profile);
        }

        
        [HttpPatch("me")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateFanProfileDto dto)
        {
            try
            {
                var fanId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _fansService.UpdateProfileAsync(fanId, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            var results = await _fansService.SearchFansAsync(query);
            return Ok(results);
        }

        
        [HttpPost("{targetId}/follow")]
        public async Task<IActionResult> Follow(int targetId)
        {
            try
            {
                var followerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await _fansService.FollowFanAsync(followerId, targetId);
                return Ok(new { message = "تمت المتابعة بنجاح." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [HttpDelete("{targetId}/unfollow")]
        public async Task<IActionResult> Unfollow(int targetId)
        {
            try
            {
                var followerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await _fansService.UnfollowFanAsync(followerId, targetId);
                return Ok(new { message = "تم إلغاء المتابعة بنجاح." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [HttpGet("{targetId}/is-following")]
        public async Task<IActionResult> CheckIsFollowing(int targetId)
        {
            var followerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var isFollowing = await _fansService.IsFollowingAsync(followerId, targetId);
            return Ok(new { isFollowing });
        }
    }
}