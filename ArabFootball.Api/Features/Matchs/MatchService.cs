using ArabFootball.Api.Features.Auth.AuthDto;
using ArabFootball.Api.Features.Enums;
using ArabFootball.Api.Features.Matchs.MatchDto;
using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ArabFootball.Api.Features.Matchs
{
    public class MatchService: IMatchService
    {
        private readonly AppDBContext _context;

        public MatchService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<PaginatedResult<Match>>> GetAllMatchesAsync(int pageNumber = 1, int pageSize = 10, string? search = null)
        {
            var matches = await _context.Matches.
            OrderByDescending(m => m.StartTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            var totalCount = matches.Count();

            var paginated = PaginatedResult<Match>.Success(matches, totalCount, pageNumber, pageSize);


            return ApiResponse<PaginatedResult<Match>>.Success(paginated);
        }

        public async Task<ApiResponse<Match>> GetMatchByIdAsync(int matchId)
        {
            var match = await _context.Matches
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null) 
                return ApiResponse<Match>.Error(HttpStatusCode.NotFound,"Not found match");
            return ApiResponse<Match>.Success(match);
        }

        public async Task<ApiResponse<Match>> CreateMatchAsync(CreateMatchDto dto, int adminId)
        {
            var match = new Match
            {
                AdminId = adminId,
                HomeTeam = dto.HomeTeam,
                AwayTeam = dto.AwayTeam,
                League = dto.League,
                StartTime = dto.StartTime,
                Stats = dto.Stats
            };

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            return ApiResponse<Match>.Success(match,"Created Match");
        }

        public async Task<ApiResponse<Match>> UpdateMatchAsync(int matchId, UpdateMatchDto dto)
        {
            var match = await _context.Matches.FindAsync(matchId);
            if (match == null)
                return ApiResponse<Match>.Error(HttpStatusCode.NotFound, "Not found match");

            match.HomeTeam = dto.HomeTeam;
            match.AwayTeam = dto.AwayTeam;
            match.League = dto.League;
            match.StartTime = dto.StartTime;
            match.Stats = dto.Stats;

            await _context.SaveChangesAsync();
            return ApiResponse<Match>.Success(match,"Updated Successed");
        }

        public async Task<ApiResponse<bool>> DeleteMatchAsync(int matchId)
        {
            var match = await _context.Matches.FindAsync(matchId);
            if (match == null)
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound, "Not found match");

            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.Success(true, "Match deleted");
        }

        public async Task<ApiResponse<bool>> ChangeStatusAsync(int matchId, MatchStatus status)
        {
            var match = await _context.Matches.FindAsync(matchId);
            if (match == null)
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound, "Not found match");

            match.Status = status;
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.Success(true, "Match Status Changed");
        }

        public async Task<ApiResponse<bool>> OpenPredictionsAsync(int matchId)
        {
            var match = await _context.Matches.FindAsync(matchId);
            if (match == null)
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound, "Not found match");


            match.PredictionState = PredictionState.Open;
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.Success(true, "Match Predictions Open");
        }

        public async Task<ApiResponse<bool>> ClosePredictionsAsync(int matchId)
        {
            var match = await _context.Matches.FindAsync(matchId);
            if (match == null)
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound, "Not found match");

            match.PredictionState = PredictionState.Closed;
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.Success(true, "Match Predictions Closed");
        }

        public async Task<ApiResponse<bool>> LinkChatAsync(int matchId, string chatUrl)
        {
            var match = await _context.Matches.FindAsync(matchId);
            if (match == null)
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound, "Not found match");

            match.ChatUrl = chatUrl;
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.Success(true, "Match link with chat");
        }

        public async Task<ApiResponse<bool>> UnlinkChatAsync(int matchId)
        {
            var match = await _context.Matches.FindAsync(matchId);
            if (match == null)
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound, "Not found match");


            match.ChatUrl = null;
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.Success(true, "Match unlink with chat");
        }
    }
}
