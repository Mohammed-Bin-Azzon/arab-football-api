using ArabFootball.Api.Features.Posts.Dtos;
using ArabFootball.Shared.Helpers;

namespace ArabFootball.Api.Features.Posts.Services
{
    public interface IPostsService
    {
        Task<ApiResponse<PostDto>> CreatePostAsync(int fanId, CreatePostDto dto);
        Task<ApiResponse<PostDto>> GetPostByIdAsync(int postId, int? viewerFanId);
        Task<ApiResponse<List<PostDto>>> GetHomeFeedAsync(int? viewerFanId = null);
        Task<ApiResponse<object>> DeletePostAsync(int postId, int fanId);
        Task<ApiResponse<PostDto>> UpdatePostAsync(int postId, int fanId, UpdatePostDto dto);
    }
}