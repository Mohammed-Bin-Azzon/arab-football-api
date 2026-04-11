using ArabFootball.Api.Features.Posts.Dtos;

namespace ArabFootball.Api.Features.Posts.Services
{
    public interface IPostsService
    {
        Task<PostDto> CreatePostAsync(int fanId, CreatePostDto dto);
        Task<List<PostDto>> GetHomeFeedAsync();
        Task<bool> DeletePostAsync(int postId, int fanId);
    }
}