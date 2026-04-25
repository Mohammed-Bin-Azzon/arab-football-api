using ArabFootball.Api.Features.Messages.MessageDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ArabFootball.Api.Features.Messages
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;

        public ChatHub(IMessageService messageService)
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
            // 1. الحصول على UserId من JWT
            var userId = int.Parse(Context.User.FindFirst("UserId").Value);

            // 2. إرسال إلى Service
            var result = await _messageService.SendMessage(dto, userId);

            // 3. في حال الخطأ
            if (!result.IsSuccess)
            {
                await Clients.Caller.SendAsync("Error", result.Message);
                return;
            }

            var message = result.Data;

            // 4. Broadcast
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

