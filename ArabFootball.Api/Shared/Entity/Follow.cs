namespace ArabFootball.Api.Shared.Entity
{
    public class Follow
    {
        
        public int ObserverId { get; set; }
        public Fan Observer { get; set; }

        
        public int TargetId { get; set; }
        public Fan Target { get; set; }

  
        public DateTime FollowDate { get; set; } = DateTime.UtcNow;
    }
}