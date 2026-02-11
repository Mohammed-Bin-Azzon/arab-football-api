namespace ArabFootball.Api.Features.Fans.Dtos
{
    public class FanProfileDto
    {
        public int Id { get; set; } 
        public string Username { get; set; } 
        public string DisplayName { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePicUrl { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public double Points { get; set; }
    }
}