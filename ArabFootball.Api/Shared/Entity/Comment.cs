using System.ComponentModel.DataAnnotations;

namespace ArabFootball.Api.Shared.Entity
{
    public class Comment
    {
        public int Id { get; set; }

        [Required, MaxLength(1000)]
        public string Content { get; set; } = null!;

        public int FanId { get; set; }
        public Fan Fan { get; set; } = null!;

        public int PostId { get; set; }
        public Post Post { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
