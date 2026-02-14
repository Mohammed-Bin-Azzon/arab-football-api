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

        /*public bool IsPrivate { get; set; } = false;*/

        public int FollowersCount { get; set; } = 0;
        public int FollowingCount { get; set; } = 0;

        public double Points { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        // public ICollection<Post> Posts { get; set; }
        // public ICollection<Prediction> Predictions { get; set; }
    }
}