using ArabFootball.Api.Features.Enums;

namespace ArabFootball.Api.Features.Chats.ChatDto
{
    public class ChatResponseDto
    {
        public int Id { get; set; }
        public string? Title { get; set; } = null!;
        public ChatType ChatType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
