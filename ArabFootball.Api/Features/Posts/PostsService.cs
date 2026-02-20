using ArabFootball.Api.Features.Posts.Dtos;
using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Api.Shared.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ArabFootball.Api.Features.Posts.Services
{
    public class PostsService : IPostsService
    {
        private readonly AppDBContext _context;
        private readonly IFileService _fileService;

        public PostsService(AppDBContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<bool> CreatePostAsync(int fanId, CreatePostDto dto)
        {
            string fileName = await _fileService.SaveFileAsync(dto.MediaFile, "posts");

            var extension = Path.GetExtension(dto.MediaFile.FileName).ToLower();
            var mediaType = (extension == ".mp4" || extension == ".mov") ? MediaType.Video : MediaType.Image;

            var post = new Post
            {
                Caption = dto.Caption,
                MediaUrl = $"/uploads/posts/{fileName}",
                MediaType = mediaType,
                FanId = fanId,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Posts.AddAsync(post);
            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<List<PostDto>> GetHomeFeedAsync(int fanId)
        {
            return await _context.Posts
                .Include(p => p.Fan) 
                .OrderByDescending(p => p.CreatedAt) 
                .Select(p => new PostDto
                {
                    Id = p.Id,
                    Caption = p.Caption,
                    MediaUrl = p.MediaUrl,
                    MediaType = p.MediaType.ToString(),
                    CreatedAt = p.CreatedAt,
                    LikeCount = p.LikeCount,
                    CommentCount = p.CommentCount,
                    FanId = p.FanId,
                    FanDisplayName = p.Fan.DisplayName,
                    FanProfilePicUrl = p.Fan.ProfilePicUrl
                })
                .ToListAsync();
        }

        public async Task<bool> DeletePostAsync(int postId, int fanId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId && p.FanId == fanId);
            if (post == null) return false;

            
            var fileName = Path.GetFileName(post.MediaUrl);
            _fileService.DeleteFile(fileName, "posts");

            _context.Posts.Remove(post);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}