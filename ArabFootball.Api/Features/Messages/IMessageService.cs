using ArabFootball.Api.Features.Messages.MessageDto;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Shared.Helpers;

namespace ArabFootball.Api.Features.Messages
{
    public interface IMessageService
    {
        Task<ApiResponse<Message>> SendMessageAsync(SendMessageDto dto, int senderId);
        Task<ApiResponse<List<MessageResponseDto>>> GetAllMessagesAsync(int chatId);
        Task<ApiResponse<bool>> DeleteMessageAsync(int messageId, int requesterId);
        Task<ApiResponse<bool>> MarkAsReadAsync(int messageId, int userId);
        Task<Message?> CreateSystemMessageAsync(int chatId, string content);
        Task<ApiResponse<string>> UploadAttachmentAsync(UploadMessageAttachmentDto dto);
    }
}
