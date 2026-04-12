using ArabFootball.Api.Features.Enums;
using ArabFootball.Api.Features.Matchs;
using ArabFootball.Api.Features.Matchs.MatchDto;
using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Api.Shared.Helpers;
using ArabFootball.Shared.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ArabFootball.Api.Features.Matchs
{
    public class MatchService : IMatchService
    {
        private readonly AppDBContext _context;

        public MatchService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<MatchDetailsDto>> GetAllMatchesAsync(int pageNumber = 1, int pageSize = 10, string? search = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _context.Matches.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(m =>
                    EF.Functions.Like(m.HomeTeam, $"%{search}%") ||
                    EF.Functions.Like(m.AwayTeam, $"%{search}%") ||
                    EF.Functions.Like(m.League, $"%{search}%"));
            }

            var totalCount = await query.CountAsync();

            var matches = await query
                .OrderByDescending(m => m.StartTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MatchDetailsDto
                {
                    Id = m.Id,
                    HomeTeam = m.HomeTeam,
                    AwayTeam = m.AwayTeam,
                    League = m.League,
                    StartTime = m.StartTime,
                    Status = m.Status,
                    PredictionState = m.PredictionState,
                    ChatUrl = m.ChatUrl,
                    StatsJson = m.StatsJson
                })
                .ToListAsync();

            return PaginatedResult<MatchDetailsDto>.Success(matches, totalCount, pageNumber, pageSize);
        }

        public async Task<MatchDetailsDto?> GetMatchByIdAsync(int matchId)
        {
            return await _context.Matches
                .AsNoTracking()
                .Where(m => m.Id == matchId)
                .Select(m => new MatchDetailsDto
                {
                    Id = m.Id,
                    HomeTeam = m.HomeTeam,
                    AwayTeam = m.AwayTeam,
                    League = m.League,
                    StartTime = m.StartTime,
                    Status = m.Status,
                    PredictionState = m.PredictionState,
                    ChatUrl = m.ChatUrl,
                    StatsJson = m.StatsJson
                })
                .FirstOrDefaultAsync();
        }

        public async Task<MatchDetailsDto> CreateMatchAsync(CreateMatchDto dto, int adminId)
        {
            var adminExists = await _context.Admins.AnyAsync(a => a.Id == adminId);
            if (!adminExists)
                throw new InvalidOperationException("المشرف غير موجود.");

            var match = new Match
            {
                AdminId = adminId,
                HomeTeam = dto.HomeTeam.Trim(),
                AwayTeam = dto.AwayTeam.Trim(),
                League = dto.League.Trim(),
                StartTime = dto.StartTime.ToUniversalTime(),
                StatsJson = dto.StatsJson,
                Status = MatchStatus.Upcoming,
                PredictionState = PredictionState.Open
            };

            await _context.Matches.AddAsync(match);
            await _context.SaveChangesAsync();

            return new MatchDetailsDto
            {
                Id = match.Id,
                HomeTeam = match.HomeTeam,
                AwayTeam = match.AwayTeam,
                League = match.League,
                StartTime = dto.StartTime.ToUniversalTime(),
                Status = match.Status,
                PredictionState = match.PredictionState,
                ChatUrl = match.ChatUrl,
                StatsJson = match.StatsJson
            };
        }

        public async Task<MatchDetailsDto> UpdateMatchAsync(int matchId, UpdateMatchDto dto)
        {
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
            if (match == null)
                throw new KeyNotFoundException("المباراة غير موجودة.");

            match.HomeTeam = dto.HomeTeam.Trim();
            match.AwayTeam = dto.AwayTeam.Trim();
            match.League = dto.League.Trim();
            match.StartTime = dto.StartTime.ToUniversalTime();
            match.StatsJson = dto.StatsJson;

            await _context.SaveChangesAsync();

            return new MatchDetailsDto
            {
                Id = match.Id,
                HomeTeam = match.HomeTeam,
                AwayTeam = match.AwayTeam,
                League = match.League,
                StartTime = dto.StartTime.ToUniversalTime(),
                Status = match.Status,
                PredictionState = match.PredictionState,
                ChatUrl = match.ChatUrl,
                StatsJson = match.StatsJson
            };
        }

        public async Task<bool> DeleteMatchAsync(int matchId)
        {
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
            if (match == null) return false;

            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangeStatusAsync(int matchId, MatchStatus status)
        {
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
            if (match == null) return false;

            match.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> OpenPredictionsAsync(int matchId)
        {
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
            if (match == null) return false;

            match.PredictionState = PredictionState.Open;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClosePredictionsAsync(int matchId)
        {
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
            if (match == null) return false;

            match.PredictionState = PredictionState.Closed;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LinkChatAsync(int matchId, string chatUrl)
        {
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
            if (match == null) return false;

            match.ChatUrl = chatUrl;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnlinkChatAsync(int matchId)
        {
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
            if (match == null) return false;

            match.ChatUrl = null;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}