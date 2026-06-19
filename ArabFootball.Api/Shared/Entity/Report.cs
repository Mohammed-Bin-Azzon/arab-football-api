namespace ArabFootball.Api.Shared.Entity
{
    public class Report
    {
        public int Id { get; set; }

        public int ReporterId { get; set; }
        public Fan Reporter { get; set; } = null!;

        public int? AdminId { get; set; }
        public Admin? Admin { get; set; }

        public TargetType TargetType { get; set; }

        public int TargetId { get; set; }

        public ReasonType Reason { get; set; }

        public ReportStatus Status { get; set; } = ReportStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum TargetType
    {
        Fan = 0,
        Post = 1,
        Comment = 2,
        Message = 3
    }

    public enum ReportStatus
    {
        Pending = 0,
        Reviewed = 1,
        Rejected = 2
    }

    public enum ReasonType
    {
        Abuse = 0, // إساءة

        Misinformation = 1, // معلومات مضللة

        Spam = 2 // إزعاج
    }
}
