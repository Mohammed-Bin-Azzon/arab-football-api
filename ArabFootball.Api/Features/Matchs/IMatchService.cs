using ArabFootball.Api.Features.Enums;
using ArabFootball.Api.Features.Matchs.MatchDto;
using ArabFootball.Shared.Helpers;

namespace ArabFootball.Api.Features.Matchs
{
    public interface IMatchService
    {
        Task<PaginatedResult<MatchDetailsDto>> GetAllMatchesAsync(int pageNumber = 1, int pageSize = 10, string? search = null);
        Task<MatchDetailsDto?> GetMatchByIdAsync(int matchId);
        Task<MatchDetailsDto> CreateMatchAsync(CreateMatchDto dto, int adminId);
        Task<MatchDetailsDto> UpdateMatchAsync(int matchId, UpdateMatchDto dto);

        Task<bool> DeleteMatchAsync(int matchId);
        Task<bool> ChangeStatusAsync(int matchId, MatchStatus status);
        Task<bool> OpenPredictionsAsync(int matchId);
        Task<bool> ClosePredictionsAsync(int matchId);
        Task<bool> LinkChatAsync(int matchId, string chatUrl);
        Task<bool> UnlinkChatAsync(int matchId);
    }
}