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

        private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp"];
        private static readonly string[] AllowedVideoExtensions = [".mp4", ".mov"];
        private static readonly HashSet<string> AllowedExtensions =
            AllowedImageExtensions.Concat(AllowedVideoExtensions).ToHashSet();

        public PostsService(AppDBContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<PostDto> CreatePostAsync(int fanId, CreatePostDto dto)
        {
            var fanExists = await _context.Fans.AnyAsync(f => f.Id == fanId);
            if (!fanExists)
                throw new InvalidOperationException("المستخدم غير موجود.");

            if (dto.MediaFile == null || dto.MediaFile.Length == 0)
                throw new InvalidOperationException("ملف الميديا مطلوب.");

            var extension = Path.GetExtension(dto.MediaFile.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new InvalidOperationException("نوع الملف غير مدعوم.");

            var mediaType = AllowedVideoExtensions.Contains(extension)
                ? MediaType.Video
                : MediaType.Image;

            string? mediaPath = null;

            try
            {
                mediaPath = await _fileService.SaveFileAsync(dto.MediaFile, "posts");

                var post = new Post
                {
                    Caption = string.IsNullOrWhiteSpace(dto.Caption) ? null : dto.Caption.Trim(),
                    MediaUrl = mediaPath,
                    MediaType = mediaType,
                    FanId = fanId,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Posts.AddAsync(post);
                await _context.SaveChangesAsync();

                var fan = await _context.Fans
                    .AsNoTracking()
                    .Where(f => f.Id == fanId)
                    .Select(f => new { f.DisplayName, f.ProfilePicUrl })
                    .FirstAsync();

                return new PostDto
                {
                    Id = post.Id,
                    Caption = post.Caption,
                    MediaUrl = post.MediaUrl,
                    MediaType = post.MediaType.ToString(),
                    LikeCount = post.LikeCount,
                    CommentCount = post.CommentCount,
                    BookmarkCount = post.BookmarkCount,
                    CreatedAt = post.CreatedAt,
                    FanId = post.FanId,
                    FanDisplayName = fan.DisplayName,
                    FanProfilePicUrl = fan.ProfilePicUrl
                };
            }
            catch
            {
                if (!string.IsNullOrWhiteSpace(mediaPath))
                {
                    _fileService.DeleteFile(mediaPath);
                }

                throw;
            }
        }

        public async Task<List<PostDto>> GetHomeFeedAsync()
        {
            return await _context.Posts
                .AsNoTracking()
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
                    BookmarkCount = p.BookmarkCount,
                    FanId = p.FanId,
                    FanDisplayName = p.Fan.DisplayName,
                    FanProfilePicUrl = p.Fan.ProfilePicUrl
                })
                .ToListAsync();
        }

        public async Task<bool> DeletePostAsync(int postId, int fanId)
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == postId && p.FanId == fanId);

            if (post == null)
                return false;

            var mediaPath = post.MediaUrl;

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            _fileService.DeleteFile(mediaPath);

            return true;
        }
    }
}