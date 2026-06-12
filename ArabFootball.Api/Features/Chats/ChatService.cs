using ArabFootball.Api.Features.Chats.ChatDto;
using ArabFootball.Api.Features.Enums;
using ArabFootball.Api.Features.Fans.Dtos;
using ArabFootball.Api.Features.Matchs.MatchDto;
using ArabFootball.Api.Features.Posts.Dtos;
using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;

namespace ArabFootball.Api.Features.Chats
{
    public class ChatService : IChatService
    {
        private readonly AppDBContext _context;

        public ChatService(AppDBContext context)
        {
            _context = context;
        }


        public async Task<ApiResponse<PaginatedResult<Chat>>> GetAllChatsAsync(int pageNumber = 1, int pageSize = 10, string? search = null)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var query = _context.Chats.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c =>
                    EF.Functions.Like(c.Title, $"%{search}%") ||
                    EF.Functions.Like(c.Match.HomeTeam, $"%{search}%") ||
                    EF.Functions.Like(c.Match.AwayTeam, $"%{search}%") ||
                    EF.Functions.Like(c.Match.League, $"%{search}%"));
            }

            var totalCount = await query.CountAsync();

            var chats = await query
                    .OrderByDescending(c => c.ChatId)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();



            var paginated = PaginatedResult<Chat>.Success(chats, totalCount, pageNumber, pageSize);

            string message = chats.Any() ? "جميع المحادثات" : "لا توجد محادثات";

            return ApiResponse<PaginatedResult<Chat>>.Success(paginated, message);

        }

        public async Task<ApiResponse<Chat>> GetChatByIdAsync(int chatId)
        {
            var chat = await _context.Chats
                
                .FirstOrDefaultAsync(c => c.ChatId == chatId);

            if (chat == null)
                return ApiResponse<Chat>.Error(HttpStatusCode.NotFound, "المحادثة غير موجودة");

            return ApiResponse<Chat>.Success(chat, "المحادثة موجودة");
        }

        public async Task<ApiResponse<List<ChatResponseDto>>> GetMyChatsAsync(int userId)
        {
            var chats = await _context.Chats
                .AsNoTracking()
                .Where(c => c.Members.Any(m => m.FanId == userId))
                .OrderByDescending(c =>
                    c.Messages
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(m => (DateTime?)m.CreatedAt)
                        .FirstOrDefault() ?? c.CreatedAt)
                .Select(c => new ChatResponseDto
                {
                    Id = c.ChatId,
                    ChatType = c.ChatType,
                    CreatedAt = c.CreatedAt,

                    Title = c.ChatType == ChatType.Private
                        ? c.Members
                            .Where(m => m.FanId != userId)
                            .Select(m => m.Fan.Username)
                            .FirstOrDefault()
                        : c.Title,

                    LastMessage = c.Messages
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(m =>
                            m.MessageType == MessageType.Text
                                ? m.Content
                                : m.MessageType == MessageType.Image
                                    ? "صورة"
                                    : m.MessageType == MessageType.Video
                                        ? "فيديو"
                                        : "رسالة نظام")
                        .FirstOrDefault(),

                    LastMessageAt = c.Messages
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(m => (DateTime?)m.CreatedAt)
                        .FirstOrDefault(),

                    LastMessageType = c.Messages
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(m => (MessageType?)m.MessageType)
                        .FirstOrDefault()
                })
                .ToListAsync();

            var message = chats.Any()
                ? "تم جلب محادثاتك"
                : "لا توجد محادثات";

            return ApiResponse<List<ChatResponseDto>>.Success(chats, message);
        }
        public async Task<ApiResponse<ChatResponseDto>> GetMatchChatAsync(int matchId)
        {
            var chat = await _context.Chats
                .AsNoTracking()
                .FirstOrDefaultAsync(c =>
                    c.MatchId == matchId &&
                    c.ChatType == ChatType.Match);

            if (chat == null)
                return ApiResponse<ChatResponseDto>.Error(HttpStatusCode.NotFound,"محادثة المباراة غير موجودة");

            var response = new ChatResponseDto
            {
                Id = chat.ChatId,
                Title = chat.Title,
                ChatType = chat.ChatType,
                CreatedAt = chat.CreatedAt
            };

            return ApiResponse<ChatResponseDto>.Success(response,"تم جلب محادثة المباراة");
        }
        public async Task<ApiResponse<ChatResponseDto>> CreatePrivateChatAsync(CreatePrivateChatDto dto)
        {
            if (dto.Fan1Id == dto.Fan2Id)
                return ApiResponse<ChatResponseDto>.Error(HttpStatusCode.BadRequest, "لا يمكنك انشاء محادثة مع نفسك");

            var f1 = await _context.Fans.FirstOrDefaultAsync(f => f.Id == dto.Fan1Id);

            if (f1 == null)
                return ApiResponse<ChatResponseDto>.Error(HttpStatusCode.BadRequest, $"المشجع {dto.Fan1Id} غير موجود");

            var f2 = await _context.Fans.FirstOrDefaultAsync(f => f.Id == dto.Fan2Id);

            if (f2 == null)
                return ApiResponse<ChatResponseDto>.Error(HttpStatusCode.BadRequest, $"المشجع {dto.Fan2Id} غير موجود");

            var existingChat = await _context.Chats
                .Where(c => c.ChatType == ChatType.Private)
                .Where(c => c.Members.Count == 2)
                .FirstOrDefaultAsync(c =>
                    c.Members.Any(m => m.FanId == dto.Fan1Id) &&
                    c.Members.Any(m => m.FanId == dto.Fan2Id)
                );

            if (existingChat != null)
            {
                var existingResponse = new ChatResponseDto
                {
                    Id = existingChat.ChatId,
                    ChatType = existingChat.ChatType,
                    CreatedAt = existingChat.CreatedAt
                };

                return ApiResponse<ChatResponseDto>.Success(
                    existingResponse,
                    "المحادثة موجودة بالفعل"
                );
            }

            var chat = new Chat
            {
                ChatType = ChatType.Private,
                CreatedAt = DateTime.UtcNow,
                Members = new List<ChatMember>()
            };
            

            chat.Members.Add(new ChatMember { FanId = dto.Fan1Id });
            chat.Members.Add(new ChatMember { FanId = dto.Fan2Id });

            _context.Chats.Add(chat);
                        
            await _context.SaveChangesAsync();

            var response = new ChatResponseDto
            {
                Id = chat.ChatId,
                ChatType = chat.ChatType,
                CreatedAt = chat.CreatedAt
            };
            return ApiResponse<ChatResponseDto>.Success(response,"تم انشاء المحادثة");
        }


        public async Task<ApiResponse<ChatResponseDto>> CreateGroupChatAsync(CreateGroupChatDto dto)
        {
            if (string.IsNullOrEmpty(dto.Title))
                return ApiResponse<ChatResponseDto>.Error(HttpStatusCode.BadRequest, "عنوان المحادثة مطلوب");


            if (dto.MemberIds == null || dto.MemberIds.Count < 2)
                return ApiResponse<ChatResponseDto>.Error(HttpStatusCode.BadRequest, "المحادثة يجب ان تحتوي على عضوين على الاقل ");

            foreach (var id in dto.MemberIds.Distinct())
            {
                var member = await _context.Fans.FirstOrDefaultAsync(f => f.Id == id);
                if (member == null)
                    return ApiResponse<ChatResponseDto>.Error(HttpStatusCode.NotFound, $"المشجع رقم {id} غير موجود");
            }

            var chat = new Chat
            {
                ChatType = ChatType.Group,
                Title = dto.Title,
                CreatedAt = DateTime.UtcNow,
                Members = new List<ChatMember>()
            };

            foreach (var id in dto.MemberIds.Distinct())
            {
                chat.Members.Add(new ChatMember
                {
                    FanId = id,
                    JoinedAt = DateTime.UtcNow
                });
            }

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            var response = new ChatResponseDto
            {
                Id = chat.ChatId,
                Title = chat.Title,
                ChatType = chat.ChatType,
                CreatedAt = chat.CreatedAt
            };

            return ApiResponse<ChatResponseDto>.Success(response, "تم انشاء المحادثة");
        }

        public async Task<ApiResponse<ChatResponseDto>> CreateMatchChatAsync(int matchId)
        {
            var matchExists = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);

            if (matchExists == null)
                return ApiResponse<ChatResponseDto>.Error(HttpStatusCode.BadRequest, "المباراة غير موجودة");

            var existingChat = await _context.Chats
                .FirstOrDefaultAsync(c => c.MatchId == matchId);

            if (existingChat != null)
                return ApiResponse<ChatResponseDto>.Error(HttpStatusCode.BadRequest, "المحادثة بالفعل موجودة");

            var chat = new Chat
            {
                ChatType = ChatType.Match,
                MatchId = matchId,
                Title = matchExists.HomeTeam + " VS " + matchExists.AwayTeam,
                CreatedAt = DateTime.UtcNow
            };

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            var response = new ChatResponseDto
            {
                Id = chat.ChatId,
                Title = chat.Title,
                ChatType = chat.ChatType,
                CreatedAt = chat.CreatedAt
            };

            return ApiResponse<ChatResponseDto>.Success(response, "تم انشاء المحادثة");
           
           
        }

    }
}
