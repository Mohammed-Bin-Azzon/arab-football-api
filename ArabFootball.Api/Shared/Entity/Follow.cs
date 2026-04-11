namespace ArabFootball.Api.Shared.Entity
{
    public class Follow
    {
        public int FollowerId { get; set; }
        public Fan Follower { get; set; } = null!;

        public int FollowedFanId { get; set; }
        public Fan FollowedFan { get; set; } = null!;

        public DateTime FollowDate { get; set; } = DateTime.UtcNow;
    }
}