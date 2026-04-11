using ArabFootball.Api.Features.Comments.CommentsDto;

namespace ArabFootball.Api.Features.Comments
{
    public interface ICommentsService
    {
        Task<CommentDto> AddCommentAsync(int fanId, CreateCommentDto dto);
        Task<List<CommentDto>> GetPostCommentsAsync(int postId);
    }
}
