using System.Net;
using ArabFootball.Api.Features.Enums;
using ArabFootball.Api.Features.Matchs.MatchDto;
using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
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

        public async Task<ApiResponse<PaginatedResult<MatchDetailsDto>>> GetAllMatchesAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null)
        {
            try
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

                var result = PaginatedResult<MatchDetailsDto>.Success(matches, totalCount, pageNumber, pageSize);

                return ApiResponse<PaginatedResult<MatchDetailsDto>>.Success(
                    result,
                    "تم جلب المباريات بنجاح.");
            }
            catch (Exception)
            {
                return ApiResponse<PaginatedResult<MatchDetailsDto>>.Error(
                    HttpStatusCode.InternalServerError,
                    "حدث خطأ أثناء جلب المباريات.");
            }
        }

        public async Task<ApiResponse<MatchDetailsDto>> GetMatchByIdAsync(int matchId)
        {
            try
            {
                var match = await _context.Matches
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

                if (match == null)
                {
                    return ApiResponse<MatchDetailsDto>.Error(
                        HttpStatusCode.NotFound,
                        "المباراة غير موجودة.");
                }

                return ApiResponse<MatchDetailsDto>.Success(
                    match,
                    "تم جلب المباراة بنجاح.");
            }
            catch (Exception)
            {
                return ApiResponse<MatchDetailsDto>.Error(
                    HttpStatusCode.InternalServerError,
                    "حدث خطأ أثناء جلب المباراة.");
            }
        }

        public async Task<ApiResponse<MatchDetailsDto>> CreateMatchAsync(CreateMatchDto dto, int adminId)
        {
            try
            {
                var adminExists = await _context.Admins.AnyAsync(a => a.Id == adminId);
                if (!adminExists)
                {
                    return ApiResponse<MatchDetailsDto>.Error(
                        HttpStatusCode.BadRequest,
                        "المشرف غير موجود.");
                }

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

                var result = new MatchDetailsDto
                {
                    Id = match.Id,
                    HomeTeam = match.HomeTeam,
                    AwayTeam = match.AwayTeam,
                    League = match.League,
                    StartTime = match.StartTime,
                    Status = match.Status,
                    PredictionState = match.PredictionState,
                    ChatUrl = match.ChatUrl,
                    StatsJson = match.StatsJson
                };

                return ApiResponse<MatchDetailsDto>.Success(
                    result,
                    "تم إنشاء المباراة بنجاح.");
            }
            catch (Exception)
            {
                return ApiResponse<MatchDetailsDto>.Error(
                    HttpStatusCode.InternalServerError,
                    "حدث خطأ أثناء إنشاء المباراة.");
            }
        }

        public async Task<ApiResponse<MatchDetailsDto>> UpdateMatchAsync(int matchId, UpdateMatchDto dto)
        {
            try
            {
                var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
                if (match == null)
                {
                    return ApiResponse<MatchDetailsDto>.Error(
                        HttpStatusCode.NotFound,
                        "المباراة غير موجودة.");
                }

                match.HomeTeam = dto.HomeTeam.Trim();
                match.AwayTeam = dto.AwayTeam.Trim();
                match.League = dto.League.Trim();
                match.StartTime = dto.StartTime.ToUniversalTime();
                match.StatsJson = dto.StatsJson;

                await _context.SaveChangesAsync();

                var result = new MatchDetailsDto
                {
                    Id = match.Id,
                    HomeTeam = match.HomeTeam,
                    AwayTeam = match.AwayTeam,
                    League = match.League,
                    StartTime = match.StartTime,
                    Status = match.Status,
                    PredictionState = match.PredictionState,
                    ChatUrl = match.ChatUrl,
                    StatsJson = match.StatsJson
                };

                return ApiResponse<MatchDetailsDto>.Success(
                    result,
                    "تم تحديث المباراة بنجاح.");
            }
            catch (Exception)
            {
                return ApiResponse<MatchDetailsDto>.Error(
                    HttpStatusCode.InternalServerError,
                    "حدث خطأ أثناء تحديث المباراة.");
            }
        }

        public async Task<ApiResponse<object>> DeleteMatchAsync(int matchId)
        {
            try
            {
                var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
                if (match == null)
                {
                    return ApiResponse<object>.Error(
                        HttpStatusCode.NotFound,
                        "المباراة غير موجودة.");
                }

                _context.Matches.Remove(match);
                await _context.SaveChangesAsync();

                return ApiResponse<object>.Success(null, "تم حذف المباراة بنجاح.");
            }
            catch (Exception)
            {
                return ApiResponse<object>.Error(
                    HttpStatusCode.InternalServerError,
                    "حدث خطأ أثناء حذف المباراة.");
            }
        }

        public async Task<ApiResponse<object>> ChangeStatusAsync(int matchId, MatchStatus status)
        {
            try
            {
                var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
                if (match == null)
                {
                    return ApiResponse<object>.Error(
                        HttpStatusCode.NotFound,
                        "المباراة غير موجودة.");
                }

                match.Status = status;
                await _context.SaveChangesAsync();

                return ApiResponse<object>.Success(null, "تم تحديث حالة المباراة بنجاح.");
            }
            catch (Exception)
            {
                return ApiResponse<object>.Error(
                    HttpStatusCode.InternalServerError,
                    "حدث خطأ أثناء تحديث حالة المباراة.");
            }
        }

        public async Task<ApiResponse<object>> OpenPredictionsAsync(int matchId)
        {
            try
            {
                var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
                if (match == null)
                {
                    return ApiResponse<object>.Error(
                        HttpStatusCode.NotFound,
                        "المباراة غير موجودة.");
                }

                match.PredictionState = PredictionState.Open;
                await _context.SaveChangesAsync();

                return ApiResponse<object>.Success(null, "تم فتح التوقعات بنجاح.");
            }
            catch (Exception)
            {
                return ApiResponse<object>.Error(
                    HttpStatusCode.InternalServerError,
                    "حدث خطأ أثناء فتح التوقعات.");
            }
        }

        public async Task<ApiResponse<object>> ClosePredictionsAsync(int matchId)
        {
            try
            {
                var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
                if (match == null)
                {
                    return ApiResponse<object>.Error(
                        HttpStatusCode.NotFound,
                        "المباراة غير موجودة.");
                }

                match.PredictionState = PredictionState.Closed;
                await _context.SaveChangesAsync();

                return ApiResponse<object>.Success(null, "تم إغلاق التوقعات بنجاح.");
            }
            catch (Exception)
            {
                return ApiResponse<object>.Error(
                    HttpStatusCode.InternalServerError,
                    "حدث خطأ أثناء إغلاق التوقعات.");
            }
        }

        public async Task<ApiResponse<object>> LinkChatAsync(int matchId, string chatUrl)
        {
            try
            {
                var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
                if (match == null)
                {
                    return ApiResponse<object>.Error(
                        HttpStatusCode.NotFound,
                        "المباراة غير موجودة.");
                }

                match.ChatUrl = chatUrl;
                await _context.SaveChangesAsync();

                return ApiResponse<object>.Success(null, "تم ربط المحادثة بنجاح.");
            }
            catch (Exception)
            {
                return ApiResponse<object>.Error(
                    HttpStatusCode.InternalServerError,
                    "حدث خطأ أثناء ربط المحادثة.");
            }
        }

        public async Task<ApiResponse<object>> UnlinkChatAsync(int matchId)
        {
            try
            {
                var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
                if (match == null)
                {
                    return ApiResponse<object>.Error(
                        HttpStatusCode.NotFound,
                        "المباراة غير موجودة.");
                }

                match.ChatUrl = null;
                await _context.SaveChangesAsync();

                return ApiResponse<object>.Success(null, "تم فك ربط المحادثة بنجاح.");
            }
            catch (Exception)
            {
                return ApiResponse<object>.Error(
                    HttpStatusCode.InternalServerError,
                    "حدث خطأ أثناء فك ربط المحادثة.");
            }
        }
    }
}