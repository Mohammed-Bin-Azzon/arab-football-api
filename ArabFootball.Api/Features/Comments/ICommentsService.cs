using ArabFootball.Api.Features.Comments.CommentsDto;

namespace ArabFootball.Api.Features.Comments
{
    public interface ICommentsService
    {
        Task<CommentDto?> AddCommentAsync(CreateCommentDto dto);
        Task<List<CommentDto>> GetPostCommentsAsync(int postId);
    }
}
