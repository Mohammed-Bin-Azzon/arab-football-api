using System.Net;
using ArabFootball.Api.Features.Enums;
using ArabFootball.Api.Features.Predictions.PredictionsDto;
using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
using ArabFootball.Shared.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ArabFootball.Api.Features.Predictions
{
    public class PredictionsService : IPredictionsService
    {
        private readonly AppDBContext _context;

        public PredictionsService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<PredictionDto>> SubmitPredictionAsync(int fanId, SubmitPredictionDto dto)
        {
            try
            {
                var fanExists = await _context.Fans.AnyAsync(f => f.Id == fanId);
                if (!fanExists)
                {
                    return ApiResponse<PredictionDto>.Fail(
                        HttpStatusCode.NotFound,
                        "المستخدم غير موجود.");
                }

                var match = await _context.Matches
                    .FirstOrDefaultAsync(m => m.Id == dto.MatchId);

                if (match == null)
                {
                    return ApiResponse<PredictionDto>.Fail(
                        HttpStatusCode.NotFound,
                        "المباراة غير موجودة.");
                }

                if (match.PredictionState != PredictionState.Open)
                {
                    return ApiResponse<PredictionDto>.Fail(
                        HttpStatusCode.BadRequest,
                        "التوقعات مغلقة لهذه المباراة.");
                }

                if (DateTime.UtcNow >= match.StartTime)
                {
                    return ApiResponse<PredictionDto>.Fail(
                        HttpStatusCode.BadRequest,
                        "انتهى وقت إرسال أو تعديل التوقع لهذه المباراة.");
                }

                var existingPrediction = await _context.Predictions
                    .FirstOrDefaultAsync(p => p.FanId == fanId && p.MatchId == dto.MatchId);

                if (existingPrediction != null)
                {
                    if (existingPrediction.IsProcessed)
                    {
                        return ApiResponse<PredictionDto>.Fail(
                            HttpStatusCode.BadRequest,
                            "لا يمكن تعديل التوقع بعد معالجته.");
                    }

                    existingPrediction.PredictedHomeScore = dto.HomeScore;
                    existingPrediction.PredictedAwayScore = dto.AwayScore;

                    await _context.SaveChangesAsync();

                    var updatedPrediction = new PredictionDto
                    {
                        Id = existingPrediction.Id,
                        MatchId = existingPrediction.MatchId,
                        PredictedHomeScore = existingPrediction.PredictedHomeScore,
                        PredictedAwayScore = existingPrediction.PredictedAwayScore,
                        IsProcessed = existingPrediction.IsProcessed,
                        PointsEarned = existingPrediction.PointsEarned,
                        CreatedAt = existingPrediction.CreatedAt
                    };

                    return ApiResponse<PredictionDto>.Success(
                        updatedPrediction,
                        "تم تحديث التوقع بنجاح.");
                }

                var prediction = new Prediction
                {
                    FanId = fanId,
                    MatchId = dto.MatchId,
                    PredictedHomeScore = dto.HomeScore,
                    PredictedAwayScore = dto.AwayScore,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Predictions.AddAsync(prediction);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    return ApiResponse<PredictionDto>.Fail(
                        HttpStatusCode.BadRequest,
                        "يوجد توقع مسجل بالفعل لهذه المباراة.");
                }

                var result = new PredictionDto
                {
                    Id = prediction.Id,
                    MatchId = prediction.MatchId,
                    PredictedHomeScore = prediction.PredictedHomeScore,
                    PredictedAwayScore = prediction.PredictedAwayScore,
                    IsProcessed = prediction.IsProcessed,
                    PointsEarned = prediction.PointsEarned,
                    CreatedAt = prediction.CreatedAt
                };

                return ApiResponse<PredictionDto>.Success(
                    result,
                    "تم إرسال التوقع بنجاح.");
            }
            catch (Exception)
            {
                return ApiResponse<PredictionDto>.Fail(
                    HttpStatusCode.InternalServerError,
                    "حدث خطأ أثناء إرسال التوقع.");
            }
        }

        public async Task<ApiResponse<List<PredictionDto>>> GetMyPredictionsAsync(int fanId)
        {
            try
            {
                var predictions = await _context.Predictions
                    .AsNoTracking()
                    .Where(p => p.FanId == fanId)
                    .OrderByDescending(p => p.CreatedAt)
                    .Select(p => new PredictionDto
                    {
                        Id = p.Id,
                        MatchId = p.MatchId,
                        PredictedHomeScore = p.PredictedHomeScore,
                        PredictedAwayScore = p.PredictedAwayScore,
                        IsProcessed = p.IsProcessed,
                        PointsEarned = p.PointsEarned,
                        CreatedAt = p.CreatedAt
                    })
                    .ToListAsync();

                return ApiResponse<List<PredictionDto>>.Success(
                    predictions,
                    "تم جلب توقعاتك بنجاح.");
            }
            catch (Exception)
            {
                return ApiResponse<List<PredictionDto>>.Fail(
                    HttpStatusCode.InternalServerError,
                    "حدث خطأ أثناء جلب التوقعات.");
            }
        }
    }
}