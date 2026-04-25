using ArabFootball.Api.Features.Comments.CommentsDto;
using ArabFootball.Shared.Helpers;

namespace ArabFootball.Api.Features.Comments
{
    public interface ICommentsService
    {
        Task<ApiResponse<CommentDto>> AddCommentAsync(int fanId, CreateCommentDto dto);
        Task<ApiResponse<List<CommentDto>>> GetPostCommentsAsync(int postId);
    }
}