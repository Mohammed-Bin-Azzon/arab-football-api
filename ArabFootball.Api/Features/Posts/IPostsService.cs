using ArabFootball.Api.Features.Posts.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArabFootball.Api.Features.Posts.Services
{
    public interface IPostsService
    {
        Task<bool> CreatePostAsync(int fanId, CreatePostDto dto);

        Task<List<PostDto>> GetHomeFeedAsync(int fanId);

        Task<bool> DeletePostAsync(int postId, int fanId);
    }
}