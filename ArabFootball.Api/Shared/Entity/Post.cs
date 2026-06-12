using System;
using System.ComponentModel.DataAnnotations;

namespace ArabFootball.Api.Shared.Entity
{

    public enum MediaType
    {
        Image,
        Video
    }

    public class Post
    {
        public int Id { get; set; }

        [MaxLength(1000)]
        public string? Caption { get; set; }

        [Required]
        public string MediaUrl { get; set; } = null!;

        public MediaType MediaType { get; set; }

        public int LikeCount { get; set; } = 0;
        public int CommentCount { get; set; } = 0;
        public int BookmarkCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int FanId { get; set; }
        public Fan Fan { get; set; } = null!;

        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
    }
}