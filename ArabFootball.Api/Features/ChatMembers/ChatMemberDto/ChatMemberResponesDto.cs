using ArabFootball.Api.Shared.Entity;

namespace ArabFootball.Api.Features.ChatMembers.ChatMemberDto
{
    public class ChatMemberResponesDto
    {
        public int ChatMemberId { get; set; }
       
        public int FanId { get; set; }
        public string? FanName { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public bool IsModerator { get; set; } = false;

        public bool IsMuted { get; set; } = false;
    }
}
