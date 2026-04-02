using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArabFootball.Api.Shared.Entity
{
    public class Fan : User
    {
        [MaxLength(100)]
        public string DisplayName { get; set; } = string.Empty; 

        public string? Bio { get; set; }

        public string? ProfilePicUrl { get; set; }


        public int FollowersCount { get; set; } = 0;
        public int FollowingCount { get; set; } = 0;

        public double Points { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Like> PostLikes { get; set; } = new List<Like>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();


        // public ICollection<Prediction> Predictions { get; set; }
    }
}