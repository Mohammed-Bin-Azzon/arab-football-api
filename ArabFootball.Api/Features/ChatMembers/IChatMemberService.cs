using ArabFootball.Api.Features.ChatMembers.ChatMemberDto;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Shared.Helpers;

namespace ArabFootball.Api.Features.ChatMembers
{
    public interface IChatMemberService
    {
        Task<ApiResponse<PaginatedResult<ChatMember>>> GetAllChatMembersAsync(int pageNumber = 1, int pageSize = 10, string? search = null);
        Task<ApiResponse<List<ChatMemberResponesDto>>> GetChatMembersByChatIdAsync(int chatId);
        Task<ApiResponse<bool>> MuteMemberAsync(int memberId);
        Task<ApiResponse<bool>> UnmuteMemberAsync(int memberId);
        Task<ApiResponse<bool>> MakeModeratorAsync(int memberId);
        Task<ApiResponse<bool>> RevokeModeratorAsync(int memberId);
        Task<ApiResponse<bool>> JoinChatAsync(int chatId, int memberId);
        Task<ApiResponse<bool>> LeaveChatAsync(int chatId, int memberId);
    }
}
