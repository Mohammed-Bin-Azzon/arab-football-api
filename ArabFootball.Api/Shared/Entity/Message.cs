using ArabFootball.Api.Features.Enums;
using System.ComponentModel.DataAnnotations;

namespace ArabFootball.Api.Shared.Entity
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        public int ChatId { get; set; }
        public Chat Chat { get; set; }

        public int? SenderId { get; set; }
        public Fan? Sender { get; set; }

        // Message Type (Text, Image, System...)
        public MessageType MessageType { get; set; }

        public string? Content { get; set; }

        public string? AttachmentUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;
    }
}
