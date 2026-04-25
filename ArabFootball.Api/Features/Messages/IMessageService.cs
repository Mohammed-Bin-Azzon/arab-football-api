using ArabFootball.Api.Features.Messages.MessageDto;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Shared.Helpers;

namespace ArabFootball.Api.Features.Messages
{
    public interface IMessageService
    {
        // Send Message
        Task<ApiResponse<Message>> SendMessage(SendMessageDto dto, int senderId);

        // Get Messages
        Task<ApiResponse<List<Message>>> GetAllMessages(int chatId);

        // Delete Message
        Task<ApiResponse<bool>> DeleteMessage(int messageId, int requesterId);

        // Mark as Read
        Task<ApiResponse<bool>> MarkAsRead(int messageId, int userId);

        Task<Message?> CreateSystemMessage(int chatId, string content);
    }
}
