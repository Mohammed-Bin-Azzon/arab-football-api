using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Shared.Helpers;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace ArabFootball.Api.Features.ChatMembers
{
    public class ChatMemberService: IChatMemberService
    {
        private readonly AppDBContext _context;
        public ChatMemberService(AppDBContext contetx) 
        {
            _context = contetx;
        }

        public async Task<ApiResponse<PaginatedResult<ChatMember>>> GetAllChatMembers(int pageNumber = 1, int pageSize = 10, string? search = null)
        {
            var chatmembers = await _context.ChatMembers.
            OrderByDescending(m => m.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            if (chatmembers == null)
                return ApiResponse<PaginatedResult<ChatMember>>.Error(HttpStatusCode.NotFound, "There are no Chats");

            var totalCount = chatmembers.Count();

            var paginated = PaginatedResult<ChatMember>.Success(chatmembers, totalCount, pageNumber, pageSize);

            return ApiResponse<PaginatedResult<ChatMember>>.Success(paginated, "Get all chats");

        }


        public async Task<ApiResponse<bool>> MuteMember(int chatId, int fanId)
        {
            var member = await _context.ChatMembers.FindAsync(chatId, fanId);

            if (member == null) 
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound , "Member not found");

            if (member.IsMuted == true)
                return ApiResponse<bool>.Error(HttpStatusCode.BadRequest, "Member muted already");

            member.IsMuted = true;

            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(true,"Memer Muted");
        }

        public async Task<ApiResponse<bool>> UnmuteMember(int chatId, int fanId)
        {
            var member = await _context.ChatMembers.FindAsync(chatId,fanId);
            if (member == null)
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound, "Member not found");

            if (member.IsMuted == false)
                return ApiResponse<bool>.Error(HttpStatusCode.BadRequest, "Member unmuted already");

            member.IsMuted = false;

            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "Member Unmuted");
        }

        public async Task<ApiResponse<bool>> MakeModerator(int chatId, int fanId)
        {
            var member = await _context.ChatMembers.FindAsync(chatId, fanId);
            if (member == null)
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound, "Member not found");

            if (member.IsModerator == true)
                return ApiResponse<bool>.Error(HttpStatusCode.BadRequest, "Member is Moderator already");

            member.IsModerator = true;

            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "Member is Moderator now"); 
        }

        public async Task<ApiResponse<bool>> RevokeModerator(int chatId, int fanId)
        {
            var member = await _context.ChatMembers.FindAsync(chatId, fanId);
            if (member == null)
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound, "Member not found");

            if (member.IsModerator == false)
                return ApiResponse<bool>.Error(HttpStatusCode.BadRequest, "Member is Unmoderator already");

            // منع إزالة آخر Moderator
            var moderatorsCount = await _context.ChatMembers
                .CountAsync(x => x.ChatId == chatId && x.IsModerator);

            if (moderatorsCount == 1)
                return ApiResponse<bool>.Error(HttpStatusCode.BadRequest, "Cannot remove last moderator");

            member.IsModerator= false;

            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "Member is Unmoderator now");
        }


        public async Task<ApiResponse<bool>> JoinChat(int chatId, int fanId)
        {
            var chatExists = await _context.Chats.AnyAsync(c => c.ChatId == chatId);
            if (!chatExists)
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound, "Chat not found");


            var exists = await _context.ChatMembers
                .AnyAsync(x => x.ChatId == chatId && x.FanId == fanId);

            if (exists)
                return ApiResponse<bool>.Error(HttpStatusCode.BadRequest, "User already in chat");

            var member = new ChatMember
            {
                ChatId = chatId,
                FanId = fanId,
                JoinedAt = DateTime.UtcNow
            };

            _context.ChatMembers.Add(member);
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "Member added");
        }

        public async Task<ApiResponse<bool>> LeaveChat(int chatId, int fanId)
        {
            var member = await _context.ChatMembers
                .FirstOrDefaultAsync(x => x.ChatId == chatId && x.FanId == fanId);

            if (member == null)
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound, "Member not found");

            // منع حذف آخر Moderator
            if (member.IsModerator)
            {
                var moderatorsCount = await _context.ChatMembers
                    .CountAsync(x => x.ChatId == chatId && x.IsModerator);

                if (moderatorsCount == 1)
                    return ApiResponse<bool>.Error(HttpStatusCode.BadRequest, "Cannot remove last moderator");
            }

            _context.ChatMembers.Remove(member);
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "Member removed");
        }

    }
}
