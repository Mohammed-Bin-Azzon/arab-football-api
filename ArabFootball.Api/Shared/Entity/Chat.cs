using ArabFootball.Api.Features.Enums;

namespace ArabFootball.Api.Shared.Entity
{
    public class Chat
    {
        public int ChatId { get; set; }

        public int? MatchId { get; set; }
        public Match Match { get; set; }

        public ChatType ChatType { get; set; }

        public string? Title { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<ChatMember> Members { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
