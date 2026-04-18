using ArabFootball.Api.Features.Enums;
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


        public async Task<ApiResponse<PaginatedResult<Chat>>> GetAllChats(int pageNumber = 1, int pageSize = 10, string? search = null)
        {
            var chats = await _context.Chats.
            OrderByDescending(m => m.Members)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            var totalCount = chats.Count();

            var paginated = PaginatedResult<Chat>.Success(chats, totalCount, pageNumber, pageSize);

            if (chats == null)
                return ApiResponse<PaginatedResult<Chat>>.Error(HttpStatusCode.NotFound, "There are no Chats");

            return ApiResponse<PaginatedResult<Chat>>.Success(paginated, "Get all chats");

        }

        public async Task<ApiResponse<Chat>> GetChatById(int chatId)
        {
            var chat = await _context.Chats
                .Include(c => c.Members)
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.ChatId == chatId);

            if (chat == null)
                return ApiResponse<Chat>.Error(HttpStatusCode.NotFound, "Chat not found");

            return ApiResponse<Chat>.Success(chat, "Get chat");
        }

        public async Task<ApiResponse<Chat>> CreatePrivateChat(int fan1Id, int fan2Id)
        {
            if (fan1Id == fan2Id)
                return ApiResponse<Chat>.Error(HttpStatusCode.BadRequest, "Cannot create chat with yourself");

            var existingChat = await _context.Chats
                .Where(c => c.ChatType == ChatType.Private)
                .Where(c => c.Members.Count == 2)
                .FirstOrDefaultAsync(c =>
                    c.Members.Any(m => m.FanId == fan1Id) &&
                    c.Members.Any(m => m.FanId == fan2Id)
                );

            if (existingChat != null)
                return ApiResponse<Chat>.Error(HttpStatusCode.BadRequest, "Chat is already existing");

            var chat = new Chat
            {
                ChatType = ChatType.Private,
                CreatedAt = DateTime.UtcNow,
                Members = new List<ChatMember>()
            };
            



            chat.Members.Add(new ChatMember { FanId = fan1Id });
            chat.Members.Add(new ChatMember { FanId = fan2Id });

            _context.Chats.Add(chat);
                        
            await _context.SaveChangesAsync();

            return ApiResponse<Chat>.Success(chat,"Chat Created");
        }


        public async Task<ApiResponse<Chat>> CreateGroupChat(string title, List<int> memberIds)
        {
            if (string.IsNullOrEmpty(title))
                return ApiResponse<Chat>.Error(HttpStatusCode.BadRequest, "Title is required");


            if (memberIds == null || memberIds.Count < 2)
                return ApiResponse<Chat>.Error(HttpStatusCode.BadRequest, "Group chat must have at least 2 members");


            var chat = new Chat
            {
                ChatType = ChatType.Group,
                Title = title,
                CreatedAt = DateTime.UtcNow,
                Members = new List<ChatMember>()
            };

            foreach (var id in memberIds.Distinct())
            {
                chat.Members.Add(new ChatMember
                {
                    FanId = id,
                    JoinedAt = DateTime.UtcNow
                });
            }

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            return ApiResponse<Chat>.Success(chat, "Chat Created");
        }

        public async Task<ApiResponse<Chat>> CreateMatchChat(int matchId)
        {
            var matchExists = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);

            if (matchExists != null)
                return ApiResponse<Chat>.Error(HttpStatusCode.BadRequest, "Match not found");

            var existingChat = await _context.Chats
                .FirstOrDefaultAsync(c => c.MatchId == matchId);

            if (existingChat != null)
                return ApiResponse<Chat>.Error(HttpStatusCode.BadRequest, "Chat is already existing");
            
            var chat = new Chat
            {
                ChatType = ChatType.Match,
                MatchId = matchId,
                Title = matchExists.HomeTeam + "VS" + matchExists.AwayTeam,
                CreatedAt = DateTime.UtcNow
            };

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            return ApiResponse<Chat>.Success(chat, "Chat Created");
        }

    }
}
