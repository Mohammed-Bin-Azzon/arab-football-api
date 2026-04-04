namespace ArabFootball.Api.Shared.Entity
{
    public class Prediction
    {
        public int Id { get; set; }

        public int FanId { get; set; }
        public Fan Fan { get; set; }

        public int MatchId { get; set; } // رقم المباراة (من الـ API الخارجي أو جدولك مستقبلاً)

        public int PredictedHomeScore { get; set; } // توقع أهداف الفريق المستضيف
        public int PredictedAwayScore { get; set; } // توقع أهداف الفريق الضيف

        public bool IsProcessed { get; set; } = false; 
        public int PointsEarned { get; set; } = 0; 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
