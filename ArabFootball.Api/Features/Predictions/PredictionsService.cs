using ArabFootball.Api.Features.Predictions.PredictionsDto;
using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
using Microsoft.AspNetCore.Identity;
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

        public async Task<PredictionDto> SubmitPredictionAsync(SubmitPredictionDto dto)
        {
            var existingPrediction = await _context.Predictions
                .FirstOrDefaultAsync(p => p.FanId == dto.FanId && p.MatchId == dto.MatchId);

            if (existingPrediction != null)
            {

                existingPrediction.PredictedHomeScore = dto.HomeScore;
                existingPrediction.PredictedAwayScore = dto.AwayScore;
                existingPrediction.CreatedAt = DateTime.UtcNow; 
            }
            else
            {
                
                var newPrediction = new Prediction
                {
                    FanId = dto.FanId,
                    MatchId = dto.MatchId,
                    PredictedHomeScore = dto.HomeScore,
                    PredictedAwayScore = dto.AwayScore
                };
                await _context.Predictions.AddAsync(newPrediction);
                existingPrediction = newPrediction; 
            }

            await _context.SaveChangesAsync();

            return new PredictionDto
            {
                Id = existingPrediction.Id,
                MatchId = existingPrediction.MatchId,
                PredictedHomeScore = existingPrediction.PredictedHomeScore,
                PredictedAwayScore = existingPrediction.PredictedAwayScore,
                IsProcessed = existingPrediction.IsProcessed,
                PointsEarned = existingPrediction.PointsEarned
            };
        }

        public async Task<List<PredictionDto>> GetFanPredictionsAsync(int fanId)
        {
            return await _context.Predictions
                .Where(p => p.FanId == fanId)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PredictionDto
                {
                    Id = p.Id,
                    MatchId = p.MatchId,
                    PredictedHomeScore = p.PredictedHomeScore,
                    PredictedAwayScore = p.PredictedAwayScore,
                    IsProcessed = p.IsProcessed,
                    PointsEarned = p.PointsEarned
                }).ToListAsync();
        }
    }
}
