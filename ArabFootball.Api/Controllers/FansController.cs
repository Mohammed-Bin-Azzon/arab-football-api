using System.Security.Claims;
using ArabFootball.Api.Features.Fans;
using ArabFootball.Api.Features.Fans.Dtos;
using ArabFootball.Api.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArabFootball.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FansController : AppControllerBase
    {
        private readonly IFansService _fansService;

        public FansController(IFansService fansService)
        {
            _fansService = fansService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<IActionResult> GetFansForAdmin([FromQuery] string? search = null)
        {
            return Response(await _fansService.GetFansForAdminAsync(search));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProfile(int id)
        {
            return Response(await _fansService.GetProfileAsync(id));
        }

        [HttpPatch("me")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateFanProfileDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var fanId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Response(await _fansService.UpdateProfileAsync(fanId, dto));
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            return Response(await _fansService.SearchFansAsync(query));
        }

        [HttpGet("{id:int}/followers")]
        public async Task<IActionResult> GetFollowers(int id)
        {
            return Response(await _fansService.GetFollowersAsync(id));
        }

        [HttpGet("{id:int}/following")]
        public async Task<IActionResult> GetFollowing(int id)
        {
            return Response(await _fansService.GetFollowingAsync(id));
        }

        [HttpPost("{targetId:int}/follow")]
        public async Task<IActionResult> Follow(int targetId)
        {
            var followerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Response(await _fansService.FollowFanAsync(followerId, targetId));
        }

        [HttpDelete("{targetId:int}/unfollow")]
        public async Task<IActionResult> Unfollow(int targetId)
        {
            var followerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Response(await _fansService.UnfollowFanAsync(followerId, targetId));
        }

        [HttpGet("{targetId:int}/is-following")]
        public async Task<IActionResult> CheckIsFollowing(int targetId)
        {
            var followerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Response(await _fansService.IsFollowingAsync(followerId, targetId));
        }
    }
}