using api_training.Controllers;
using ArabFootball.Api.Features.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static ArabFootball.Shared.Helpers.Routing;

namespace ArabFootball.Api.Controllers
{
    [Route(Messages.Prefix)]
    public class MessageController : AppControllerBase
    {
        private readonly IMessageService _service;

        public MessageController(IMessageService service)
        {
            _service = service;
        }

        
        [HttpGet(Messages.GetByChat)]
        public async Task<IActionResult> GetByChat([FromRoute] int chatId)
        {
            return Response(await _service.GetAllMessages(chatId));
        }

        [HttpDelete(Messages.Delete)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            int userId = int.Parse(User.FindFirst("UserId").Value);

            return Response(await _service.DeleteMessage(id, userId));
        }

        
        [HttpPatch(Messages.MarkAsRead)]
        public async Task<IActionResult> MarkAsRead([FromRoute] int id)
        {
            int userId = int.Parse(User.FindFirst("UserId").Value);

            return Response(await _service.MarkAsRead(id, userId));
        }
    }
}
