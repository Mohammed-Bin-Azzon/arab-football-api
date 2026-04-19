using ArabFootball.Api.Features.Chats;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using api_training.Controllers;
using static ArabFootball.Shared.Helpers.Routing;
using ArabFootball.Api.Features.Chats.ChatDto;
using Microsoft.AspNetCore.Authorization;

namespace ArabFootball.Api.Controllers
{
    [Route(Chats.Prefix)]
    [Authorize(Roles = "Admin")]
    public class ChatController : AppControllerBase
    {
        private readonly IChatService _service;

        public ChatController(IChatService service)
        {
            _service = service;
        }

        [HttpGet(Chats.GetAll)]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, string? search = null)
        {
            return Response(await _service.GetAllChats(pageNumber, pageSize, search));
        }

        [HttpGet(Chats.GetById)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return Response(await _service.GetChatById(id));
        }

       
        [HttpPost(Chats.CreatePrivate)]
        public async Task<IActionResult> CreatePrivate([FromBody] CreatePrivateChatDto dto)
        {
            return Response(await _service.CreatePrivateChat(dto.Fan1Id, dto.Fan2Id));
        }

        [HttpPost(Chats.CreateGroup)]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupChatDto dto)
        {
            return Response(await _service.CreateGroupChat(dto.Title, dto.MemberIds));
        }

        
        [HttpPost(Chats.CreateMatch)]
        public async Task<IActionResult> CreateMatchChat([FromRoute] int matchId)
        {
            return Response(await _service.CreateMatchChat(matchId));
        }

        
        [HttpPost(Chats.AddMember)]
        public async Task<IActionResult> AddMember([FromRoute] int id, [FromBody] int fanId)
        {
            return Response(await _service.AddMember(id, fanId));
        }

        
        [HttpDelete(Chats.RemoveMember)]
        public async Task<IActionResult> RemoveMember([FromRoute] int id, [FromBody] int fanId)
        {
            return Response(await _service.RemoveMember(id, fanId));
        }
    }
}

