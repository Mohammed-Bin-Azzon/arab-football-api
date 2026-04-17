using ArabFootball.Api.Shared.Entity;
using ArabFootball.Shared.Helpers;

namespace ArabFootball.Api.Features.Chats
{
    public interface IChatService
    {
        // Get
        Task<ApiResponse<PaginatedResult<Chat>>> GetAllChats(int pageNumber = 1, int pageSize = 10, string? search = null);
        Task<ApiResponse<Chat>> GetChatById(int chatId);

        // Create
        Task<ApiResponse<Chat>> CreatePrivateChat(int fan1Id, int fan2Id);
        Task<ApiResponse<Chat>> CreateGroupChat(string title, List<int> memberIds);
        Task<ApiResponse<Chat>> CreateMatchChat(int matchId);

        // Members
        Task<ApiResponse<bool>> AddMember(int chatId, int fanId);
        Task<ApiResponse<bool>> RemoveMember(int chatId, int fanId);

        
    }
}
