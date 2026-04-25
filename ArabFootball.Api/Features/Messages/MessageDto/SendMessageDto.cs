using ArabFootball.Api.Features.Enums;

namespace ArabFootball.Api.Features.Messages.MessageDto
{
    public class SendMessageDto
    {
        public int ChatId { get; set; }
        public string? Content { get; set; }
        public string? AttachmentUrl { get; set; }
        public MessageType MessageType { get; set; }
    }
}
