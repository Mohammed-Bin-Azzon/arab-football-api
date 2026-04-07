using ArabFootball.Api.Features.Enums;
using ArabFootball.Api.Features.Matchs.MatchDto;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Shared.Helpers;

namespace ArabFootball.Api.Features.Matchs
{
    public interface IMatchService
    {
        Task<ApiResponse<PaginatedResult<Match>>> GetAllMatchesAsync(int pageNumber = 1, int pageSize = 10, string? search = null);

        Task<ApiResponse<Match>> GetMatchByIdAsync(int matchId);
        Task<ApiResponse<Match>> CreateMatchAsync(CreateMatchDto dto, int adminId);

        Task<ApiResponse<Match>> UpdateMatchAsync(int matchId, UpdateMatchDto dto);

        Task<ApiResponse<bool>> DeleteMatchAsync(int matchId);

        Task<ApiResponse<bool>> ChangeStatusAsync(int matchId, MatchStatus status);

        Task<ApiResponse<bool>> OpenPredictionsAsync(int matchId);

        Task<ApiResponse<bool>> ClosePredictionsAsync(int matchId);

        Task<ApiResponse<bool>> LinkChatAsync(int matchId, string chatUrl);

        Task<ApiResponse<bool>> UnlinkChatAsync(int matchId);
    }
}
