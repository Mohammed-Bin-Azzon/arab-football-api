namespace ArabFootball.Api.Shared.Entity
{
    public class Like
    {
        public int Id { get; set; }

        public int FanId { get; set; }
        public Fan Fan { get; set; } = null!;

        public int PostId { get; set; }
        public Post Post { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
