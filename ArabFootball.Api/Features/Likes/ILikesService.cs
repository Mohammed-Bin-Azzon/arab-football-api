using ArabFootball.Api.Features.Likes.LikesDto;
using ArabFootball.Shared.Helpers;

namespace ArabFootball.Api.Features.Likes
{
    public interface ILikesService
    {
        Task<ApiResponse<LikeResultDto>> ToggleLikeAsync(int postId, int fanId);
    }
}