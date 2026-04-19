using api_training.Controllers;
using ArabFootball.Api.Features.ChatMembers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static ArabFootball.Shared.Helpers.Routing;

namespace ArabFootball.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatMembersController : AppControllerBase
    {
        private readonly IChatMemberService _chatMemberService;

        public ChatMembersController(IChatMemberService chatMemberService)
        { 
            _chatMemberService = chatMemberService;
        }

        [HttpGet(ChatMembers.GetAll)]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, string? search = null)
        {
            return Response(await _chatMemberService.GetAllChatMembers(pageNumber, pageSize, search));
        }

        [HttpPut(ChatMembers.MuteMember)]
        public async Task<IActionResult> MuteMember([FromRoute] int chatid, [FromRoute] int fanid)
        {
            return Response(await _chatMemberService.MuteMember(chatid, fanid));
        }

        [HttpPut(ChatMembers.UnmuteMember)]
        public async Task<IActionResult> UnmuteMember([FromRoute] int chatId, [FromRoute] int fanId)
        {
            return Response(await _chatMemberService.UnmuteMember(chatId, fanId));
        }

        [HttpPut(ChatMembers.MakeModerator)]
        public async Task<IActionResult> MakeModerator([FromRoute] int chatId, [FromRoute] int fanId)
        {
            return Response(await _chatMemberService.MakeModerator(chatId, fanId));
        }
        [HttpPut(ChatMembers.RevokeModerator)]
        public async Task<IActionResult> RevokeModerator([FromRoute] int chatId, [FromRoute] int fanId)
        {
            return Response(await _chatMemberService.RevokeModerator(chatId, fanId));
        }

        [HttpPut(ChatMembers.JoinChat)]
        public async Task<IActionResult> JoinChat([FromRoute] int chatId, [FromRoute] int fanId)
        {
            return Response(await _chatMemberService.JoinChat(chatId, fanId));
        }

        [HttpPut(ChatMembers.LeaveChat)]
        public async Task<IActionResult> LeaveChat([FromRoute] int chatId, [FromRoute] int fanId)
        {
            return Response(await _chatMemberService.LeaveChat(chatId, fanId));
        }

    }
}
