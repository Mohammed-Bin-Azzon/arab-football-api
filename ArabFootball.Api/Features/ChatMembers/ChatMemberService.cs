using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Shared.Helpers;
using System.Net;
using Microsoft.EntityFrameworkCore;
using ArabFootball.Api.Features.ChatMembers.ChatMemberDto;

namespace ArabFootball.Api.Features.ChatMembers
{
    public class ChatMemberService: IChatMemberService
    {
        private readonly AppDBContext _context;
        public ChatMemberService(AppDBContext contetx) 
        {
            _context = contetx;
        }

        public async Task<ApiResponse<PaginatedResult<ChatMember>>> GetAllChatMembersAsync(int pageNumber = 1, int pageSize = 10, string? search = null)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var query = _context.ChatMembers.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c =>
                    EF.Functions.Like(c.Id.ToString(), $"%{search}%"));
            }

            var totalCount = await query.CountAsync();

            var chatmembers = await query
                    .OrderByDescending(c => c.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

            var paginated = PaginatedResult<ChatMember>.Success(chatmembers, totalCount, pageNumber, pageSize);

            string message = chatmembers.Any() ? " جميع اعضاء المحادثة" : "لا يوجد هناك اعضاء في المحادثة";

            return ApiResponse<PaginatedResult<ChatMember>>.Success(paginated, message);
                        
        }

        public async Task<ApiResponse<List<ChatMemberResponesDto>>> GetChatMembersByChatIdAsync(int chatId)
        {
            var chat = await _context.Chats.FirstOrDefaultAsync(c => c.ChatId == chatId);
            if (chat == null)
                return ApiResponse<List<ChatMemberResponesDto>>.Error(HttpStatusCode.NotFound, "المحادثة غير موجودة");

            var chatMembers = await _context.ChatMembers
                                         .Where(c => c.ChatId == chatId)
                                         .OrderByDescending(c => c.JoinedAt)
                                         .Select(c => new ChatMemberResponesDto
                                         {
                                             ChatMemberId = c.Id,
                                             FanId = c.FanId,
                                             JoinedAt = c.JoinedAt,
                                             IsModerator = c.IsModerator,
                                             IsMuted = c.IsMuted
                                         })
                                         .ToListAsync();

            var message = chatMembers.Count() > 0 ? "حمبع اعضاء المحادثة" : "لا يوجد هناك اعضاء في المحادثة";

            return ApiResponse<List<ChatMemberResponesDto>>.Success(chatMembers, message);

        }


        public async Task<ApiResponse<bool>> MuteMemberAsync(int memberId)
        {
            var member = await _context.ChatMembers.FirstOrDefaultAsync(c=> c.Id == memberId);

            if (member == null) 
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound , "العضو غير موحود");

            if (member.IsMuted == true)
                return ApiResponse<bool>.Error(HttpStatusCode.BadRequest, "العضو على الصامت بالفعل");

            member.IsMuted = true;

            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(true,"تم كتم صوت العضو");
        }

        public async Task<ApiResponse<bool>> UnmuteMemberAsync(int memberId)
        {
            var member = await _context.ChatMembers.FirstOrDefaultAsync(c => c.Id == memberId);
            if (member == null)
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound, "العضو غير موحود");

            if (member.IsMuted == false)
                return ApiResponse<bool>.Error(HttpStatusCode.BadRequest, "العضو ليس على الصامت بالفعل");

            member.IsMuted = false;

            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "تم الغاء كتم صوت العضو");
        }

        public async Task<ApiResponse<bool>> MakeModeratorAsync(int memberId)
        {
            var member = await _context.ChatMembers.FirstOrDefaultAsync(c => c.Id == memberId);
            if (member == null)
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound, "العضو غير موحود");

            if (member.IsModerator == true)
                return ApiResponse<bool>.Error(HttpStatusCode.BadRequest, "العضو مشرف بالفعل");

            member.IsModerator = true;

            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "العضو مشرف الان"); 
        }

        public async Task<ApiResponse<bool>> RevokeModeratorAsync(int memberId)
        {
            var member = await _context.ChatMembers.FirstOrDefaultAsync(c => c.Id == memberId);
            if (member == null)
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound, "العضو غير موحود");

            if (member.IsModerator == false)
                return ApiResponse<bool>.Error(HttpStatusCode.BadRequest, "العضو ليس مشرف بالفعل");

            // منع إزالة آخر Moderator
            var moderatorsCount = await _context.ChatMembers
                .CountAsync(x => x.ChatId == member.ChatId && x.IsModerator);

            if (moderatorsCount == 1)
                return ApiResponse<bool>.Error(HttpStatusCode.BadRequest, "لا يمكن  ازالة اخر مشرف");

            member.IsModerator= false;
            
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "العضو ليس مشرف الان");
        }


        public async Task<ApiResponse<bool>> JoinChatAsync(int chatId, int fanId)
        {
            var chatExists = await _context.Chats.AnyAsync(c => c.ChatId == chatId);
            if (!chatExists)
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound, "المحادثة غير موجودة");

            var exists = await _context.ChatMembers.AnyAsync(x => x.ChatId == chatId && x.FanId == fanId);
            if (exists)
                return ApiResponse<bool>.Error(HttpStatusCode.BadRequest,"انت مضاف في المحادثة بالفعل");

            var fan = await _context.Fans.AnyAsync(x => x.Id == fanId);
            if (!fan)
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound,"المشجع غير موجود");

            var member = new ChatMember
            {
                ChatId = chatId,
                FanId = fanId,
                JoinedAt = DateTime.UtcNow
            };

            _context.ChatMembers.Add(member);
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "تم اضافة المشجع الى المحادثة");
        }

        public async Task<ApiResponse<bool>> LeaveChatAsync(int chatId, int fanId)
        {
            var chatExists = await _context.Chats.AnyAsync(c => c.ChatId == chatId);
            if (!chatExists)
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound, "المحادثة غير موجودة");

            var member = await _context.ChatMembers.FirstOrDefaultAsync(x => x.ChatId == chatId && x.FanId == fanId);
            if (member == null)
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound, "العضو غير موحود");

            // منع حذف آخر Moderator
            if (member.IsModerator)
            {
                var moderatorsCount = await _context.ChatMembers
                    .CountAsync(x => x.ChatId == chatId && x.IsModerator);

                if (moderatorsCount == 1)
                    return ApiResponse<bool>.Error(HttpStatusCode.BadRequest, "لايمكن  حذف اخر مشرف");
            }

            _context.ChatMembers.Remove(member);
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "تم حذف العضو من المحادثة");
        }

    }
}
