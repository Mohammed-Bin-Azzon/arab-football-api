namespace ArabFootball.Api.Features.Fans.Dtos
{
    public class FanAdminDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
    }
}