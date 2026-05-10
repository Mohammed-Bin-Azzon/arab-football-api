using api_training.Controllers;
using ArabFootball.Api.Features.ChatMembers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace ArabFootball.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatMembersController : AppControllerBase
    {
        private readonly IChatMemberService _chatMemberService;

        public ChatMembersController(IChatMemberService chatMemberService)
        {
            _chatMemberService = chatMemberService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, string? search = null)
        {
            return Response(await _chatMemberService.GetAllChatMembersAsync(pageNumber, pageSize, search));
        }

        [HttpGet("chats/{chatId:int}")]
        public async Task<IActionResult> GetByChatId(int chatId)
        {
            return Response(await _chatMemberService.GetChatMembersByChatIdAsync(chatId));
        }

        [HttpPatch("{memberId:int}/mute")]
        public async Task<IActionResult> MuteMember([FromRoute] int memberId)
        {
            return Response(await _chatMemberService.MuteMemberAsync(memberId));
        }

        [HttpPatch("{memberId:int}/unmute")]
        public async Task<IActionResult> UnmuteMember([FromRoute] int memberId)
        {
            return Response(await _chatMemberService.UnmuteMemberAsync(memberId));
        }

        [HttpPatch("{memberId:int}/make-moderator")]
        public async Task<IActionResult> MakeModerator([FromRoute] int memberId)
        {
            return Response(await _chatMemberService.MakeModeratorAsync(memberId));
        }

        [HttpPatch("{memberId:int}/revoke-moderator")]
        public async Task<IActionResult> RevokeModerator([FromRoute] int memberId)
        {
            return Response(await _chatMemberService.RevokeModeratorAsync(memberId));
        }

        [HttpPost("chats/{chatId:int}/members/{fanId:int}/join")]
        public async Task<IActionResult> JoinChat([FromRoute] int chatId, [FromRoute] int fanId)
        {
            return Response(await _chatMemberService.JoinChatAsync(chatId, fanId));
        }

        [HttpDelete("chats/{chatId:int}/members/{fanId:int}/leave")]
        public async Task<IActionResult> LeaveChat([FromRoute] int chatId, [FromRoute] int fanId)
        {
            return Response(await _chatMemberService.LeaveChatAsync(chatId, fanId));
        }

    }
}
