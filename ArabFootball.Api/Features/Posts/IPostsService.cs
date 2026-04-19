using ArabFootball.Api.Features.Posts.Dtos;
using ArabFootball.Shared.Helpers;

namespace ArabFootball.Api.Features.Posts.Services
{
    public interface IPostsService
    {
        Task<ApiResponse<PostDto>> CreatePostAsync(int fanId, CreatePostDto dto);
        Task<ApiResponse<List<PostDto>>> GetHomeFeedAsync();
        Task<ApiResponse<object>> DeletePostAsync(int postId, int fanId);
    }
}