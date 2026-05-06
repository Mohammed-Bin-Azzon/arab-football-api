using ArabFootball.Api.Features.Messages.MessageDto;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Shared.Helpers;

namespace ArabFootball.Api.Features.Messages
{
    public interface IMessageService
    {
        // Send Message
        Task<ApiResponse<Message>> SendMessageAsync(SendMessageDto dto, int senderId);

        // Get Messages
        Task<ApiResponse<List<Message>>> GetAllMessagesAsync(int chatId);

        // Delete Message
        Task<ApiResponse<bool>> DeleteMessageAsync(int messageId, int requesterId);

        // Mark as Read
        Task<ApiResponse<bool>> MarkAsReadAsync(int messageId, int userId);

        Task<Message?> CreateSystemMessageAsync(int chatId, string content);
    }
}
