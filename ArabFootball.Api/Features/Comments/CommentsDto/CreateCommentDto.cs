using System.ComponentModel.DataAnnotations;

namespace ArabFootball.Api.Features.Comments.CommentsDto
{
    public class CreateCommentDto
    {
        [Required(ErrorMessage = "محتوى التعليق مطلوب")]
        [MaxLength(1000, ErrorMessage = "التعليق طويل جداً")]
        public string Content { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "معرّف المنشور غير صالح")]
        public int PostId { get; set; }
    }
}
