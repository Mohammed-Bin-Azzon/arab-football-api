using System.ComponentModel.DataAnnotations;

namespace ArabFootball.Api.Shared.Entity
{
    public class ChatMember
    {
        [Key]
        public int Id { get; set; }
        public int ChatId { get; set; }
        public Chat Chat { get; set; }
        public int FanId { get; set; }
        public Fan Fan { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public bool IsModerator { get; set; } = false;

        public bool IsMuted { get; set; } = false;
    }
}
