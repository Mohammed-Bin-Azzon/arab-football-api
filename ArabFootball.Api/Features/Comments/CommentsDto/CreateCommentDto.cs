namespace ArabFootball.Api.Features.Comments.CommentsDto
{
    public class CreateCommentDto
    {
        public string Content { get; set; }
        public int PostId { get; set; }
        public int FanId { get; set; } // مؤقتاً حتى نستخدم الـ Auth
    }
}
