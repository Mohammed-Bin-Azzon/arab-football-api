using ArabFootball.Api.Features.Enums;
using ArabFootball.Api.Features.Matchs.MatchDto;
using ArabFootball.Shared.Helpers;

namespace ArabFootball.Api.Features.Matchs
{
    public interface IMatchService
    {
        Task<ApiResponse<PaginatedResult<MatchDetailsDto>>> GetAllMatchesAsync(int pageNumber = 1, int pageSize = 10, string? search = null);
        Task<ApiResponse<MatchDetailsDto>> GetMatchByIdAsync(int matchId);
        Task<ApiResponse<MatchDetailsDto>> CreateMatchAsync(CreateMatchDto dto, int adminId);
        Task<ApiResponse<MatchDetailsDto>> UpdateMatchAsync(int matchId, UpdateMatchDto dto);

        Task<ApiResponse<object>> DeleteMatchAsync(int matchId);
        Task<ApiResponse<object>> ChangeStatusAsync(int matchId, MatchStatus status);
        Task<ApiResponse<object>> OpenPredictionsAsync(int matchId);
        Task<ApiResponse<object>> ClosePredictionsAsync(int matchId);
        Task<ApiResponse<object>> LinkChatAsync(int matchId, string chatUrl);
        Task<ApiResponse<object>> UnlinkChatAsync(int matchId);
    }
}