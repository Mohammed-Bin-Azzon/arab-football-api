using ArabFootball.Api.Features.Enums;

namespace ArabFootball.Api.Features.Matchs.MatchDto
{
    public class MatchDetailsDto
    {
        public int Id { get; set; }
        public string HomeTeam { get; set; } = null!;
        public string AwayTeam { get; set; } = null!;
        public string League { get; set; } = null!;

        public DateTime StartTime { get; set; }

        public string MatchDate => StartTime.ToString("yyyy-MM-dd");

        public int Hour
        {
            get
            {
                if (StartTime.Hour == 0)
                    return 12;

                if (StartTime.Hour > 12)
                    return StartTime.Hour - 12;

                return StartTime.Hour;
            }
        }

        public int Minute => StartTime.Minute;

        public string Period => StartTime.Hour < 12 ? "صباح" : "مساء";

        public MatchStatus Status { get; set; }
        public PredictionState PredictionState { get; set; }
        public string? ChatUrl { get; set; }
    }
}