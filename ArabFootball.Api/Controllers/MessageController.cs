using ArabFootball.Api.Features.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArabFootball.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : AppControllerBase
    {
        private readonly IMessageService _service;

        public MessageController(IMessageService service)
        {
            _service = service;
        }

        
        [HttpGet("{chatId:int}")]
        public async Task<IActionResult> GetAllByChat([FromRoute] int chatId)
        {
            return Response(await _service.GetAllMessagesAsync(chatId));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Response(await _service.DeleteMessageAsync(id, userId));
        }

        
        [HttpPatch("{id:int}/mark-as-read")]
        public async Task<IActionResult> MarkAsRead([FromRoute] int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Response(await _service.MarkAsReadAsync(id, userId));
        }
    }
}
