using ArabFootball.Api.Features.Enums;

namespace ArabFootball.Api.Features.Messages.MessageDto
{
    public class MessageResponseDto
    {
        public int MessageId { get; set; }
        public int ChatId { get; set; }
        public int? SenderId { get; set; }
        public string? SenderName { get; set; }

        public string? Content { get; set; }
        public string? AttachmentUrl { get; set; }

        public MessageType MessageType { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
