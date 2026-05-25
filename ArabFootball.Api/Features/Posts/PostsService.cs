using System.Net;
using ArabFootball.Api.Features.Posts.Dtos;
using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Api.Shared.Helpers;
using ArabFootball.Shared.Helpers;
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

        public async Task<ApiResponse<PostDto>> CreatePostAsync(int fanId, CreatePostDto dto)
        {
            string? mediaPath = null;

            try
            {
                var fanExists = await _context.Fans.AnyAsync(f => f.Id == fanId);
                if (!fanExists)
                {
                    return ApiResponse<PostDto>.Error(
                        HttpStatusCode.NotFound,
                        "المستخدم غير موجود.");
                }

                if (dto.MediaFile == null || dto.MediaFile.Length == 0)
                {
                    return ApiResponse<PostDto>.Error(
                        HttpStatusCode.BadRequest,
                        "ملف الميديا مطلوب.");
                }

                var extension = Path.GetExtension(dto.MediaFile.FileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(extension))
                {
                    return ApiResponse<PostDto>.Error(
                        HttpStatusCode.BadRequest,
                        "نوع الملف غير مدعوم.");
                }

                var mediaType = AllowedVideoExtensions.Contains(extension)
                    ? MediaType.Video
                    : MediaType.Image;

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

                var result = new PostDto
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

                return ApiResponse<PostDto>.Success(result, "تم إنشاء المنشور بنجاح.");
            }
            catch (Exception)
            {
                if (!string.IsNullOrWhiteSpace(mediaPath))
                {
                    _fileService.DeleteFile(mediaPath);
                }

                return ApiResponse<PostDto>.Error(
                    HttpStatusCode.InternalServerError,
                    "حدث خطأ أثناء إنشاء المنشور.");
            }
        }

        public async Task<ApiResponse<PostDto>> GetPostByIdAsync(int postId, int viewerFanId)
        {
            var post = await _context.Posts
                .AsNoTracking()
                .Where(p => p.Id == postId)
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
                    IsLiked = p.Likes.Any(l => l.FanId == viewerFanId),
                    IsBookmarked = p.Bookmarks.Any(b => b.FanId == viewerFanId),
                    FanId = p.FanId,
                    FanDisplayName = p.Fan.DisplayName,
                    FanProfilePicUrl = p.Fan.ProfilePicUrl
                })
                .FirstOrDefaultAsync();

            if (post == null)
            {
                return ApiResponse<PostDto>.Error(
                    HttpStatusCode.NotFound,
                    "المنشور غير موجود.");
            }

            return ApiResponse<PostDto>.Success(post, "تم جلب تفاصيل المنشور بنجاح.");
        }

        public async Task<ApiResponse<List<PostDto>>> GetHomeFeedAsync()
        {
            var posts = await _context.Posts
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

            return ApiResponse<List<PostDto>>.Success(posts, "تم جلب المنشورات بنجاح.");
        }

        public async Task<ApiResponse<object>> DeletePostAsync(int postId, int fanId)
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == postId && p.FanId == fanId);

            if (post == null)
            {
                return ApiResponse<object>.Error(
                    HttpStatusCode.NotFound,
                    "المنشور غير موجود أو لا تملك صلاحية حذفه.");
            }

            var mediaPath = post.MediaUrl;

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            _fileService.DeleteFile(mediaPath);

            return ApiResponse<object>.Success(null, "تم حذف المنشور بنجاح.");
        }

        public async Task<ApiResponse<PostDto>> UpdatePostAsync(int postId, int fanId, UpdatePostDto dto)
        {
            string? newMediaPath = null;
            string? oldMediaPath = null;

            try
            {
                var post = await _context.Posts
                    .Include(p => p.Fan)
                    .FirstOrDefaultAsync(p => p.Id == postId);

                if (post == null)
                {
                    return ApiResponse<PostDto>.Error(
                        HttpStatusCode.NotFound,
                        "المنشور غير موجود.");
                }

                if (post.FanId != fanId)
                {
                    return ApiResponse<PostDto>.Error(
                        HttpStatusCode.Forbidden,
                        "لا يمكنك تعديل منشور لا تملكه.");
                }

                if (dto.Caption != null)
                {
                    post.Caption = string.IsNullOrWhiteSpace(dto.Caption)
                        ? null
                        : dto.Caption.Trim();
                }

                if (dto.Media != null)
                {
                    oldMediaPath = post.MediaUrl;

                    newMediaPath = await _fileService.SaveFileAsync(dto.Media, "posts");
                    post.MediaUrl = newMediaPath;

                    var contentType = dto.Media.ContentType.ToLower();

                    if (contentType.StartsWith("image/"))
                    {
                        post.MediaType = MediaType.Image;
                    }
                    else if (contentType.StartsWith("video/"))
                    {
                        post.MediaType = MediaType.Video;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(newMediaPath))
                            _fileService.DeleteFile(newMediaPath);

                        return ApiResponse<PostDto>.Error(
                            HttpStatusCode.BadRequest,
                            "نوع الملف غير مدعوم. الرجاء رفع صورة أو فيديو فقط.");
                    }
                }

                await _context.SaveChangesAsync();

                if (!string.IsNullOrWhiteSpace(oldMediaPath) &&
                    !string.Equals(oldMediaPath, post.MediaUrl, StringComparison.OrdinalIgnoreCase))
                {
                    _fileService.DeleteFile(oldMediaPath);
                }

                var isLiked = await _context.Likes.AnyAsync(l => l.PostId == post.Id && l.FanId == fanId);
                var isBookmarked = await _context.Bookmarks.AnyAsync(b => b.PostId == post.Id && b.FanId == fanId);

                var result = new PostDto
                {
                    Id = post.Id,
                    Caption = post.Caption,
                    MediaUrl = post.MediaUrl,
                    MediaType = post.MediaType.ToString(),
                    CreatedAt = post.CreatedAt,
                    LikeCount = post.LikeCount,
                    CommentCount = post.CommentCount,
                    BookmarkCount = post.BookmarkCount,
                    IsLiked = isLiked,
                    IsBookmarked = isBookmarked,
                    FanId = post.FanId,
                    FanDisplayName = post.Fan.DisplayName,
                    FanProfilePicUrl = post.Fan.ProfilePicUrl
                };

                return ApiResponse<PostDto>.Success(result, "تم تعديل المنشور بنجاح.");
            }
            catch (Exception)
            {
                if (!string.IsNullOrWhiteSpace(newMediaPath))
                    _fileService.DeleteFile(newMediaPath);

                return ApiResponse<PostDto>.Error(
                    HttpStatusCode.InternalServerError,
                    "حدث خطأ أثناء تعديل المنشور.");
            }
        }
    }


}