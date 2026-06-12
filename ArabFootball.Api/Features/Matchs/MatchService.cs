using System.Net;
using ArabFootball.Api.Features.Enums;
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
        private readonly IFileService _fileService;

        public MatchService(AppDBContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<ApiResponse<PaginatedResult<MatchDetailsDto>>> GetAllMatchesAsync(int pageNumber = 1, int pageSize = 10, string? search = null)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var query = _context.Matches.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(m =>
                    EF.Functions.Like(m.HomeTeam, $"%{search}%") ||
                    EF.Functions.Like(m.AwayTeam, $"%{search}%") ||
                    EF.Functions.Like(m.League, $"%{search}%"));
            }

            var totalCount = await query.CountAsync();

            var matches = await query
                .OrderByDescending(m => m.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MatchDetailsDto
                {
                    Id = m.Id,
                    HomeTeam = m.HomeTeam,
                    HomeTeamLogoUrl = m.HomeTeamLogoUrl,
                    AwayTeam = m.AwayTeam,
                    AwayTeamLogoUrl = m.AwayTeamLogoUrl,
                    League = m.League,
                    StartTime = m.StartTime,
                    Status = m.Status,
                    PredictionState = m.PredictionState,
                    ChatUrl = m.ChatUrl
                })
                .ToListAsync();

            var paginated = PaginatedResult<MatchDetailsDto>.Success(matches, totalCount, pageNumber, pageSize);

            string message = matches.Any() ? "جميع المباريات" : "لا توجد مباريات";

            return ApiResponse<PaginatedResult<MatchDetailsDto>>.Success(paginated, message);
        }

        public async Task<ApiResponse<MatchDetailsDto>> GetMatchByIdAsync(int matchId)
        {
            var match = await _context.Matches
                .AsNoTracking()
                .Where(m => m.Id == matchId)
                .Select(m => new MatchDetailsDto
                {
                    Id = m.Id,
                    HomeTeam = m.HomeTeam,
                    HomeTeamLogoUrl = m.HomeTeamLogoUrl,
                    AwayTeam = m.AwayTeam,
                    AwayTeamLogoUrl = m.AwayTeamLogoUrl,
                    League = m.League,
                    StartTime = m.StartTime,
                    Status = m.Status,
                    PredictionState = m.PredictionState,
                    ChatUrl = m.ChatUrl,
                })
                .FirstOrDefaultAsync();

            if (match == null)
            {
                return ApiResponse<MatchDetailsDto>.Error(HttpStatusCode.NotFound, "المباراة غير موجودة.");
            }

            return ApiResponse<MatchDetailsDto>.Success(match, "تم جلب المباراة بنجاح.");
        }

        public async Task<ApiResponse<MatchDetailsDto>> CreateMatchAsync(CreateMatchDto dto, int adminId)
        {
            var adminExists = await _context.Admins.AnyAsync(a => a.Id == adminId);
            if (!adminExists)
            {
                return ApiResponse<MatchDetailsDto>.Error(HttpStatusCode.BadRequest, "المشرف غير موجود.");
            }

            if (!SaudiDateTimeBuilder.TryBuild(
                    dto.MatchDate,
                    dto.Hour,
                    dto.Minute,
                    dto.Period,
                    out var startTimeSaudi,
                    out var dateTimeError))
            {
                return ApiResponse<MatchDetailsDto>.Error(
                    HttpStatusCode.BadRequest,
                    dateTimeError);
            }

            if (startTimeSaudi <= SaudiTime.Now())
            {
                return ApiResponse<MatchDetailsDto>.Error(
                    HttpStatusCode.BadRequest,
                    "وقت المباراة يجب أن يكون في المستقبل بتوقيت السعودية.");
            }

            string? homeLogoUrl = null;
            string? awayLogoUrl = null;

            if (dto.HomeTeamLogo != null)
            {
                homeLogoUrl = await _fileService.SaveFileAsync(
                    dto.HomeTeamLogo,
                    "matches"
                );
            }

            if (dto.AwayTeamLogo != null)
            {
                awayLogoUrl = await _fileService.SaveFileAsync(
                    dto.AwayTeamLogo,
                    "matches"
                );
            }

            var match = new Match
            {
                AdminId = adminId,
                HomeTeam = dto.HomeTeam.Trim(),
                HomeTeamLogoUrl = homeLogoUrl,
                AwayTeam = dto.AwayTeam.Trim(),
                AwayTeamLogoUrl = awayLogoUrl,
                League = dto.League.Trim(),
                StartTime = startTimeSaudi,
                Status = MatchStatus.Upcoming,
                PredictionState = PredictionState.Open
            };

            await _context.Matches.AddAsync(match);
            await _context.SaveChangesAsync();

            var result = new MatchDetailsDto
            {
                Id = match.Id,
                HomeTeam = match.HomeTeam,
                HomeTeamLogoUrl = match.HomeTeamLogoUrl,
                AwayTeam = match.AwayTeam,
                AwayTeamLogoUrl= match.AwayTeamLogoUrl,
                League = match.League,
                StartTime = match.StartTime,
                Status = match.Status,
                PredictionState = match.PredictionState,
                ChatUrl = match.ChatUrl,
            };

            return ApiResponse<MatchDetailsDto>.Success(result, "تم إنشاء المباراة بنجاح.");
        }

        public async Task<ApiResponse<MatchDetailsDto>> UpdateMatchAsync(int matchId, UpdateMatchDto dto)
        {
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
            if (match == null)
            {
                return ApiResponse<MatchDetailsDto>.Error(HttpStatusCode.NotFound, "المباراة غير موجودة.");
            }

            if (!SaudiDateTimeBuilder.TryBuild(
                    dto.MatchDate,
                    dto.Hour,
                    dto.Minute,
                    dto.Period,
                    out var startTimeSaudi,
                    out var dateTimeError))
            {
                return ApiResponse<MatchDetailsDto>.Error(
                    HttpStatusCode.BadRequest,
                    dateTimeError);
            }

            if (startTimeSaudi <= SaudiTime.Now())
            {
                return ApiResponse<MatchDetailsDto>.Error(
                    HttpStatusCode.BadRequest,
                    "وقت المباراة يجب أن يكون في المستقبل بتوقيت السعودية.");
            }

            if (dto.HomeTeamLogo != null)
            {
                if (!string.IsNullOrWhiteSpace(match.HomeTeamLogoUrl))
                {
                    _fileService.DeleteFile(match.HomeTeamLogoUrl);
                }

                match.HomeTeamLogoUrl = await _fileService.SaveFileAsync(
                    dto.HomeTeamLogo,
                    "matches"
                );
            }

            if (dto.AwayTeamLogo != null)
            {
                if (!string.IsNullOrWhiteSpace(match.AwayTeamLogoUrl))
                {
                    _fileService.DeleteFile(match.AwayTeamLogoUrl);
                }

                match.AwayTeamLogoUrl = await _fileService.SaveFileAsync(
                    dto.AwayTeamLogo,
                    "matches"
                );
            }

            match.HomeTeam = dto.HomeTeam.Trim();
            match.AwayTeam = dto.AwayTeam.Trim();
            match.League = dto.League.Trim();
            match.StartTime = startTimeSaudi;

            await _context.SaveChangesAsync();

            var result = new MatchDetailsDto
            {
                Id = match.Id,
                HomeTeam = match.HomeTeam,
                HomeTeamLogoUrl = match.HomeTeamLogoUrl,
                AwayTeam = match.AwayTeam,
                AwayTeamLogoUrl = match.AwayTeamLogoUrl,
                League = match.League,
                StartTime = match.StartTime,
                Status = match.Status,
                PredictionState = match.PredictionState,
                ChatUrl = match.ChatUrl,
            };

            return ApiResponse<MatchDetailsDto>.Success(result, "تم تحديث المباراة بنجاح.");
        }

        public async Task<ApiResponse<bool>> DeleteMatchAsync(int matchId)
        {
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
            if (match == null)
            {
                return ApiResponse<bool>.Error(
                    HttpStatusCode.NotFound,
                    "المباراة غير موجودة.");
            }

            if (!string.IsNullOrWhiteSpace(match.HomeTeamLogoUrl))
            {
                _fileService.DeleteFile(match.HomeTeamLogoUrl);
            }

            if (!string.IsNullOrWhiteSpace(match.AwayTeamLogoUrl))
            {
                _fileService.DeleteFile(match.AwayTeamLogoUrl);
            }

            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "تم حذف المباراة بنجاح.");
        }

        public async Task<ApiResponse<bool>> ChangeStatusAsync(int matchId, MatchStatus status)
        {
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
            if (match == null)
            {
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound, "المباراة غير موجودة.");
            }

            if (!Enum.IsDefined(typeof(MatchStatus), status))
            {
                return ApiResponse<bool>.Error(HttpStatusCode.BadRequest,"حالة المباراة غير صحيحة");
            }

            match.Status = status;
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "تم تحديث حالة المباراة بنجاح.");
        }

        public async Task<ApiResponse<bool>> OpenPredictionsAsync(int matchId)
        {
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
            if (match == null)
            {
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound, "المباراة غير موجودة.");
            }

            if (match.PredictionState == PredictionState.Open)
            {
                return ApiResponse<bool>.Error(HttpStatusCode.BadRequest, "التوقع للمباراة مفتوح بالفعل");
            }

            if (SaudiTime.Now() >= match.StartTime)
            {
                return ApiResponse<bool>.Error(
                    HttpStatusCode.BadRequest,
                    "لا يمكن فتح التوقعات لأن وقت المباراة بدأ أو انتهى.");
            }

            match.PredictionState = PredictionState.Open;
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "تم فتح التوقعات بنجاح.");
        }

        public async Task<ApiResponse<bool>> ClosePredictionsAsync(int matchId)
        {
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
            if (match == null)
            {
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound, "المباراة غير موجودة.");
            }

            if (match.PredictionState == PredictionState.Closed)
            {
                return ApiResponse<bool>.Error(HttpStatusCode.BadRequest, "التوقع للمباراة مغلق بالفعل");
            }

            match.PredictionState = PredictionState.Closed;
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "تم إغلاق التوقعات بنجاح.");
        }

        public async Task<ApiResponse<bool>> LinkChatAsync(int matchId, string chatUrl)
        {
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
            if (match == null)
            {
                return ApiResponse<bool>.Error(HttpStatusCode.NotFound, "المباراة غير موجودة.");
            }

            match.ChatUrl = chatUrl;
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "تم ربط المحادثة بنجاح.");
        }

        public async Task<ApiResponse<bool>> UnlinkChatAsync(int matchId)
        {
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
            if (match == null)
            {
                return ApiResponse<bool>.Error(
                    HttpStatusCode.NotFound,
                    "المباراة غير موجودة.");
            }

            match.ChatUrl = null;
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "تم فك ربط المحادثة بنجاح.");
        }
    }
}