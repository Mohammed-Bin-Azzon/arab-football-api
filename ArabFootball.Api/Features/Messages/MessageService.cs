using ArabFootball.Api.Features.Enums;
using ArabFootball.Api.Features.Messages.MessageDto;
using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Api.Shared.Helpers;
using ArabFootball.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ArabFootball.Api.Features.Messages
{
    public class MessageService : IMessageService
    {
        private readonly AppDBContext _context;
        private readonly IFileService _fileService;

        private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp"];
        private static readonly string[] AllowedVideoExtensions = [".mp4", ".mov"];
        private static readonly HashSet<string> AllowedExtensions =
            AllowedImageExtensions.Concat(AllowedVideoExtensions).ToHashSet();

        public MessageService(AppDBContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<ApiResponse<Message>> SendMessageAsync(SendMessageDto dto, int senderId)
        {
            var chat = await _context.Chats
                        .Include(c => c.Match)
                        .FirstOrDefaultAsync(c => c.ChatId == dto.ChatId);
            if (chat == null)
                return ApiResponse<Message>.Error(
                    HttpStatusCode.NotFound,
                    "المحادثة غير موجودة"
                );

            var availabilityError = ValidateMatchChatAvailability(chat);

            if (availabilityError != null)
            {
                return ApiResponse<Message>.Error(
                    availabilityError.StatusCode,
                    availabilityError.Message
                );
            }

            if (chat.ChatType != ChatType.Match)
            {
                var member = await _context.ChatMembers
                    .FirstOrDefaultAsync(x =>
                        x.ChatId == dto.ChatId &&
                        x.FanId == senderId);

                if (member == null)
                    return ApiResponse<Message>.Error(
                        HttpStatusCode.Forbidden,
                        "انت لست عضوا في هذه المحادثة"
                    );

                if (member.IsMuted)
                    return ApiResponse<Message>.Error(
                        HttpStatusCode.Forbidden,
                        "انت في وضع كتم الصوت"
                    );
            }

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


        public async Task<ApiResponse<List<MessageResponseDto>>> GetAllMessagesAsync(int chatId)
        {
            var exists = await _context.Chats
                .AnyAsync(c => c.ChatId == chatId);

            if (!exists)
                return ApiResponse<List<MessageResponseDto>>.Error(
                    HttpStatusCode.NotFound,
                    "المحادثة غير موجودة"
                );

            var messages = await _context.Messages
                .AsNoTracking()
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.CreatedAt)
                .Select(m => new MessageResponseDto
                {
                    MessageId = m.MessageId,
                    ChatId = m.ChatId,
                    SenderId = m.SenderId,
                    SenderName = m.Sender != null
                        ? m.Sender.DisplayName
                        : null,
                    Content = m.Content,
                    AttachmentUrl = m.AttachmentUrl,
                    MessageType = m.MessageType,
                    CreatedAt = m.CreatedAt,
                    IsRead = m.IsRead
                })
                .ToListAsync();

            return ApiResponse<List<MessageResponseDto>>.Success(
                messages,
                "تم استلام الرسائل"
            );
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

        public async Task<ApiResponse<string>> UploadAttachmentAsync(UploadMessageAttachmentDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                return ApiResponse<string>.Error(
                    HttpStatusCode.BadRequest,
                    "ملف المرفق مطلوب"
                );

            var extension = Path.GetExtension(dto.File.FileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
                return ApiResponse<string>.Error(
                    HttpStatusCode.BadRequest,
                    "نوع الملف غير مدعوم"
                );

            var path = await _fileService.SaveFileAsync(dto.File, "messages");

            return ApiResponse<string>.Success(path, "تم رفع المرفق بنجاح");
        }
        private ApiResponse<bool>? ValidateMatchChatAvailability(Chat chat)
        {
            if (chat.ChatType != ChatType.Match)
                return null;

            if (chat.Match == null)
                return ApiResponse<bool>.Error(
                    HttpStatusCode.BadRequest,
                    "بيانات المباراة المرتبطة بالمحادثة غير متوفرة"
                );

            var now = SaudiTime.Now();

            var opensAt = chat.Match.StartTime.AddMinutes(-30);
            var closesAt = chat.Match.StartTime.AddHours(3);

            if (now < opensAt)
                return ApiResponse<bool>.Error(
                    HttpStatusCode.Forbidden,
                    "محادثة المباراة لم تفتح بعد"
                );

            if (now > closesAt)
                return ApiResponse<bool>.Error(
                    HttpStatusCode.Forbidden,
                    "تم إغلاق محادثة المباراة"
                );

            return null;
        }
    }
}

