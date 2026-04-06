namespace ArabFootball.Api.Features.Comments.CommentsDto
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int FanId { get; set; }
        public string FanName { get; set; } 
        public string FanProfilePic { get; set; } 
    }
}
