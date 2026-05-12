using ArabFootball.Api.Features.Chats;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ArabFootball.Api.Features.Chats.ChatDto;
using Microsoft.AspNetCore.Authorization;

namespace ArabFootball.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : AppControllerBase
    {
        private readonly IChatService _service;

        public ChatController(IChatService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, string? search = null)
        {
            return Response(await _service.GetAllChatsAsync(pageNumber, pageSize, search));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return Response(await _service.GetChatByIdAsync(id));
        }

        
        [HttpPost("create-private")]
        public async Task<IActionResult> CreatePrivate([FromBody] CreatePrivateChatDto dto)
        {
            return Response(await _service.CreatePrivateChatAsync(dto));
        }

        [HttpPost("create-group")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupChatDto dto)
        {
            return Response(await _service.CreateGroupChatAsync(dto));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create-match/{matchId:int}")]
        public async Task<IActionResult> CreateMatchChat([FromRoute] int matchId)
        {
            return Response(await _service.CreateMatchChatAsync(matchId));
        }
               
    }
}

