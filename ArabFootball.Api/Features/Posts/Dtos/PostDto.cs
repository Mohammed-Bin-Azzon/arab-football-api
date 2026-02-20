using System;

namespace ArabFootball.Api.Features.Posts.Dtos
{
    public class PostDto
    {
        public int Id { get; set; }
        public string? Caption { get; set; }

        public string MediaUrl { get; set; } 
        public string MediaType { get; set; } 

        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        //public int BookmarkCount { get; set; }

        public DateTime CreatedAt { get; set; }

        
        public int FanId { get; set; }
        public string FanDisplayName { get; set; }
        public string? FanProfilePicUrl { get; set; }
    }
}