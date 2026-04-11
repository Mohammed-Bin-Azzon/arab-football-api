using ArabFootball.Api.Features.Likes.LikesDto;

namespace ArabFootball.Api.Features.Likes
{

    public interface ILikesService
    {
        Task<LikeResultDto> ToggleLikeAsync(int postId, int fanId);
    }
}
