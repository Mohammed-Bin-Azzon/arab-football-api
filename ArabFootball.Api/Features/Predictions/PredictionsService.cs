using ArabFootball.Api.Features.Enums;
using ArabFootball.Api.Features.Predictions.PredictionsDto;
using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
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

        public async Task<PredictionDto> SubmitPredictionAsync(int fanId, SubmitPredictionDto dto)
        {
            var fanExists = await _context.Fans.AnyAsync(f => f.Id == fanId);
            if (!fanExists)
                throw new InvalidOperationException("المستخدم غير موجود.");

            var match = await _context.Matches
                .FirstOrDefaultAsync(m => m.Id == dto.MatchId);

            if (match == null)
                throw new KeyNotFoundException("المباراة غير موجودة.");

            if (match.PredictionState != PredictionState.Open)
                throw new InvalidOperationException("التوقعات مغلقة لهذه المباراة.");

            if (DateTime.UtcNow >= match.StartTime)
                throw new InvalidOperationException("انتهى وقت إرسال أو تعديل التوقع لهذه المباراة.");

            var existingPrediction = await _context.Predictions
                .FirstOrDefaultAsync(p => p.FanId == fanId && p.MatchId == dto.MatchId);

            if (existingPrediction != null)
            {
                if (existingPrediction.IsProcessed)
                    throw new InvalidOperationException("لا يمكن تعديل التوقع بعد معالجته.");

                existingPrediction.PredictedHomeScore = dto.HomeScore;
                existingPrediction.PredictedAwayScore = dto.AwayScore;

                await _context.SaveChangesAsync();

                return new PredictionDto
                {
                    Id = existingPrediction.Id,
                    MatchId = existingPrediction.MatchId,
                    PredictedHomeScore = existingPrediction.PredictedHomeScore,
                    PredictedAwayScore = existingPrediction.PredictedAwayScore,
                    IsProcessed = existingPrediction.IsProcessed,
                    PointsEarned = existingPrediction.PointsEarned,
                    CreatedAt = existingPrediction.CreatedAt
                };
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
                throw new InvalidOperationException("يوجد توقع مسجل بالفعل لهذه المباراة.");
            }

            return new PredictionDto
            {
                Id = prediction.Id,
                MatchId = prediction.MatchId,
                PredictedHomeScore = prediction.PredictedHomeScore,
                PredictedAwayScore = prediction.PredictedAwayScore,
                IsProcessed = prediction.IsProcessed,
                PointsEarned = prediction.PointsEarned,
                CreatedAt = prediction.CreatedAt
            };
        }

        public async Task<List<PredictionDto>> GetMyPredictionsAsync(int fanId)
        {
            return await _context.Predictions
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
        }
    }
}