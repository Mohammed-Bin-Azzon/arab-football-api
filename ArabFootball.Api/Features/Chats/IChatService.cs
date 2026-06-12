using ArabFootball.Api.Features.Chats.ChatDto;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Shared.Helpers;

namespace ArabFootball.Api.Features.Chats
{
    public interface IChatService
    {
        // Get
        Task<ApiResponse<PaginatedResult<Chat>>> GetAllChatsAsync(int pageNumber = 1, int pageSize = 10, string? search = null);
        Task<ApiResponse<Chat>> GetChatByIdAsync(int chatId);
        Task<ApiResponse<List<ChatResponseDto>>> GetMyChatsAsync(int userId);
        Task<ApiResponse<ChatResponseDto>> GetMatchChatAsync(int matchId);

        // Create
        Task<ApiResponse<ChatResponseDto>> CreatePrivateChatAsync(CreatePrivateChatDto dto);
        Task<ApiResponse<ChatResponseDto>> CreateGroupChatAsync(CreateGroupChatDto dto);
        Task<ApiResponse<ChatResponseDto>> CreateMatchChatAsync(int matchId);
        
    }
}
