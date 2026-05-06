using ArabFootball.Api.Features.Enums;
using ArabFootball.Api.Features.Messages.MessageDto;
using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ArabFootball.Api.Features.Messages
{
    public class MessageService : IMessageService
    {
        private readonly AppDBContext _context;

        public MessageService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<Message>> SendMessageAsync(SendMessageDto dto, int senderId)
        {
            // 1. تحقق من الشات
            var chat = await _context.Chats
                .FirstOrDefaultAsync(c => c.ChatId == dto.ChatId);

            if (chat == null)
                return ApiResponse<Message>.Error(HttpStatusCode.NotFound, "Chat not found");

            // 2. تحقق من العضوية
            var member = await _context.ChatMembers
                .FirstOrDefaultAsync(x => x.ChatId == dto.ChatId && x.FanId == senderId);

            if (member == null)
                return ApiResponse<Message>.Error(HttpStatusCode.Forbidden, "User is not a member of this chat");

            // 3. تحقق من mute
            if (member.IsMuted)
                return ApiResponse<Message>.Error(HttpStatusCode.Forbidden, "User is muted");

            // 4. Validation حسب نوع الرسالة 🔥
            switch (dto.MessageType)
            {
                case MessageType.Text:
                    if (string.IsNullOrWhiteSpace(dto.Content))
                        return ApiResponse<Message>.Error(HttpStatusCode.BadRequest, "Text content is required");
                    break;

                case MessageType.Image:
                    if (string.IsNullOrWhiteSpace(dto.AttachmentUrl))
                        return ApiResponse<Message>.Error(HttpStatusCode.BadRequest, "Image URL is required");
                    break;

                case MessageType.Video:
                    if (string.IsNullOrWhiteSpace(dto.AttachmentUrl))
                        return ApiResponse<Message>.Error(HttpStatusCode.BadRequest, "Video URL is required");
                    break;

                case MessageType.System:
                    return ApiResponse<Message>.Error(HttpStatusCode.BadRequest, "Cannot send system message manually");
            }

            // 5. إنشاء الرسالة
            var message = new Message
            {
                ChatId = dto.ChatId,
                SenderId = senderId,
                Content = dto.Content,
                AttachmentUrl = dto.AttachmentUrl,
                MessageType = dto.MessageType,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            // 6. حفظ
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return ApiResponse<Message>.Success(message, "Message sent");
        }


        public async Task<ApiResponse<List<Message>>> GetAllMessagesAsync(int chatId)
        {
            var exists = await _context.Chats
                .AnyAsync(c => c.ChatId == chatId);

            if (!exists)
                return ApiResponse<List<Message>>.Error(HttpStatusCode.NotFound, "Chat not found");

            var messages = await _context.Messages
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            return ApiResponse<List<Message>>.Success(messages, "Messages retrieved");
        }


        public async Task<ApiResponse<bool>> DeleteMessageAsync(int messageId, int requesterId)
        {
            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.MessageId == messageId);

            if (message == null)
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound, "Message not found");

            // فقط المرسل يقدر يحذف (مؤقتًا)
            if (message.SenderId != requesterId)
                return ApiResponse<bool>.Error(HttpStatusCode.Forbidden, "Not allowed");

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "Message deleted");
        }


        public async Task<ApiResponse<bool>> MarkAsReadAsync(int messageId, int userId)
        {
            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.MessageId == messageId);

            if (message == null)
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound, "Message not found");

            message.IsRead = true;

            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "Message marked as read");
        }

        public async Task<Message?> CreateSystemMessageAsync(int chatId, string content)
        {
            var chatExists = await _context.Chats
                .AnyAsync(c => c.ChatId == chatId);

            if (!chatExists)
                return null;

            var message = new Message
            {
                ChatId = chatId,
                SenderId = null,
                Content = content,
                MessageType = MessageType.System,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return message;
        }
    }
}

