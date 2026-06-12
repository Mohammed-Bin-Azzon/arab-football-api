using System;

namespace ArabFootball.Api.Features.Posts.Dtos
{
    public class PostDto
    {
        public int Id { get; set; }
        public string? Caption { get; set; }

        public string MediaUrl { get; set; } = null!;
        public string MediaType { get; set; } = null!;

        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public int BookmarkCount { get; set; }

        public bool IsLiked { get; set; }
        public bool IsBookmarked { get; set; }

        public DateTime CreatedAt { get; set; }

        public int FanId { get; set; }
        public string FanDisplayName { get; set; } = null!;
        public string? FanProfilePicUrl { get; set; }
    }
}