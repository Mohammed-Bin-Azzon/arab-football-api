using ArabFootball.Api.Shared.Entity;
using ArabFootball.Shared.Helpers;

namespace ArabFootball.Api.Features.ChatMembers
{
    public interface IChatMemberService
    {
        Task<ApiResponse<PaginatedResult<ChatMember>>> GetAllChatMembers(int pageNumber = 1, int pageSize = 10, string? search = null);

        Task<ApiResponse<bool>> MuteMember(int chatId, int fanId);
        Task<ApiResponse<bool>> UnmuteMember(int chatId, int fanId);

        Task<ApiResponse<bool>> MakeModerator(int chatId, int fanId);
        Task<ApiResponse<bool>> RevokeModerator(int chatId, int fanId);

        Task<ApiResponse<bool>> JoinChat(int chatId, int fanId);
        Task<ApiResponse<bool>> LeaveChat(int chatId, int fanId);
    }
}
