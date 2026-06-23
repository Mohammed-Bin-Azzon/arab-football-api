namespace ArabFootball.Api.Features.Fans.Dtos
{
    public class FanBasicProfileDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string? Bio { get; set; }
        public string? ProfilePicUrl { get; set; }

        public string? FavoriteTeamCode { get; set; }
        public string? FavoritePlayerCode { get; set; }

        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }

         public int Points { get; set; }
    }
}