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
            var chat = await _context.Chats.AnyAsync(c => c.ChatId == dto.ChatId);

            if (!chat)
                return ApiResponse<Message>.Error(HttpStatusCode.NotFound, "المحادة غير موجودة");

            var member = await _context.ChatMembers.FirstOrDefaultAsync(x => x.ChatId == dto.ChatId && x.FanId == senderId);

            if (member == null)
                return ApiResponse<Message>.Error(HttpStatusCode.Forbidden, "انت ليست عضوا في هذة المحادثة");

            if (member.IsMuted)
                return ApiResponse<Message>.Error(HttpStatusCode.Forbidden, "انت في وضع كتم الصوت");

            switch (dto.MessageType)
            {
                case MessageType.Text:
                    if (string.IsNullOrWhiteSpace(dto.Content))
                        return ApiResponse<Message>.Error(HttpStatusCode.BadRequest, "المحتوى النصي مطلوب");
                    break;

                case MessageType.Image:
                    if (string.IsNullOrWhiteSpace(dto.AttachmentUrl))
                        return ApiResponse<Message>.Error(HttpStatusCode.BadRequest, "رابط الصورة مطلوب");
                    break;

                case MessageType.Video:
                    if (string.IsNullOrWhiteSpace(dto.AttachmentUrl))
                        return ApiResponse<Message>.Error(HttpStatusCode.BadRequest, "رابط الفيديو مطلوب");
                    break;

                case MessageType.System:
                    return ApiResponse<Message>.Error(HttpStatusCode.BadRequest, "لا يمكن ارسال رسالة النظام");
            }

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

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return ApiResponse<Message>.Success(message, "تم ارسال الرسالة");
        }


        public async Task<ApiResponse<List<Message>>> GetAllMessagesAsync(int chatId)
        {
            var exists = await _context.Chats
                .AnyAsync(c => c.ChatId == chatId);

            if (!exists)
                return ApiResponse<List<Message>>.Error(HttpStatusCode.NotFound, "المحادثة غير موجودة");

            var messages = await _context.Messages
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            return ApiResponse<List<Message>>.Success(messages, "تم استلام الرسالة");
        }


        public async Task<ApiResponse<bool>> DeleteMessageAsync(int messageId, int requesterId)
        {
            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.MessageId == messageId);

            if (message == null)
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound, "الرسالة غير موجودة");

            // فقط المرسل يقدر يحذف (مؤقتًا)
            if (message.SenderId != requesterId)
                return ApiResponse<bool>.Error(HttpStatusCode.Forbidden, "انت غير مسموح لك بحذف هذة الرسالة");

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "تم محذف الرسالة");
        }


        public async Task<ApiResponse<bool>> MarkAsReadAsync(int messageId, int userId)
        {
            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.MessageId == messageId);

            if (message == null)
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound, "الرسالة غير موجودة");

            message.IsRead = true;

            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "تم قراءة الرسالة");
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

