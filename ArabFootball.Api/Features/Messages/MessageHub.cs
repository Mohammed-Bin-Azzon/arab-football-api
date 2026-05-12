using ArabFootball.Api.Features.Messages.MessageDto;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ArabFootball.Api.Features.Messages
{
    public class MessageHub : Hub
    {
        private readonly IMessageService _messageService;

        public MessageHub(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task JoinChat(int chatId)
        {
            string groupName = chatId.ToString();

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveChat(int chatId)
        {
            string groupName = chatId.ToString();

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }


        public async Task SendMessage(SendMessageDto dto)
        {
            int userId = int.Parse(Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _messageService.SendMessageAsync(dto, userId);

            if (!result.IsSuccess)
            {
                await Clients.Caller.SendAsync("Error", result.Message);
                return;
            }

            var message = result.Data;

            await Clients.Group(dto.ChatId.ToString())
                .SendAsync("ReceiveMessage", new
                {
                    message.MessageId,
                    message.ChatId,
                    message.SenderId,
                    message.Content,
                    message.AttachmentUrl,
                    message.MessageType,
                    message.CreatedAt
                });
        }
    }

}

