using ArabFootball.Api.Shared.Entity;
using Microsoft.EntityFrameworkCore;

namespace ArabFootball.Api.Shared.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }
        
        public DbSet<User> Users => Set<User>();
        public DbSet<Fan> Fans => Set<Fan>();
        public DbSet<Admin> Admins => Set<Admin>();
        public DbSet<Follow> Follows => Set<Follow>();
        public DbSet<Match> Matches => Set<Match>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<Chat> Chats => Set<Chat>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Like> Likes => Set<Like>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Bookmark> Bookmarks => Set<Bookmark>();
        public DbSet<Prediction> Predictions => Set<Prediction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // Inheritance: User / Fan / Admin
            // =========================
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Fan>().ToTable("Fans");
            modelBuilder.Entity<Admin>().ToTable("Admins");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // =========================
            // Fan
            // =========================
            modelBuilder.Entity<Fan>(entity =>
            {
                entity.Property(f => f.DisplayName)
                    .HasMaxLength(100);

                entity.Property(f => f.Bio)
                    .HasMaxLength(500);
            });

            //يمنع تكرار نفس المستخدم في نفس الشات
            modelBuilder.Entity<ChatMember>()
                .HasIndex(x => new { x.ChatId, x.FanId })
                .IsUnique();



            // =========================
            // Follow
            // =========================

            modelBuilder.Entity<Follow>(entity =>
            {
                entity.HasKey(f => new { f.FollowerId, f.FollowedFanId });

                entity.HasOne(f => f.Follower)
                    .WithMany(f => f.Following)
                    .HasForeignKey(f => f.FollowerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(f => f.FollowedFan)
                    .WithMany(f => f.Followers)
                    .HasForeignKey(f => f.FollowedFanId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t =>
                    t.HasCheckConstraint("CK_Follows_NoSelfFollow", "[FollowerId] <> [FollowedFanId]"));
            });

            // =========================
            // Post
            // =========================
            modelBuilder.Entity<Post>(entity =>
            {
                entity.Property(p => p.Caption)
                    .HasMaxLength(1000);

                entity.Property(p => p.MediaUrl)
                    .IsRequired();

                entity.HasOne(p => p.Fan)
                    .WithMany(f => f.Posts)
                    .HasForeignKey(p => p.FanId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // =========================
            // Like
            // =========================
            modelBuilder.Entity<Like>(entity =>
            {
                entity.HasIndex(l => new { l.FanId, l.PostId })
                    .IsUnique();

                entity.HasOne(l => l.Fan)
                    .WithMany(f => f.Likes)
                    .HasForeignKey(l => l.FanId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(l => l.Post)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(l => l.PostId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // =========================
            // Comment
            // =========================
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(c => c.Content)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.HasOne(c => c.Fan)
                    .WithMany(f => f.Comments)
                    .HasForeignKey(c => c.FanId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Post)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(c => c.PostId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // =========================
            // Bookmark
            // =========================
            modelBuilder.Entity<Bookmark>(entity =>
            {
                entity.HasIndex(b => new { b.FanId, b.PostId })
                    .IsUnique();

                entity.HasOne(b => b.Fan)
                    .WithMany(f => f.Bookmarks)
                    .HasForeignKey(b => b.FanId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.Post)
                    .WithMany(p => p.Bookmarks)
                    .HasForeignKey(b => b.PostId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // =========================
            // Match
            // =========================
            modelBuilder.Entity<Match>(entity =>
            {
                entity.Property(m => m.HomeTeam)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(m => m.AwayTeam)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(m => m.League)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(m => m.Admin)
                    .WithMany()
                    .HasForeignKey(m => m.AdminId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // =========================
            // Prediction
            // =========================
            modelBuilder.Entity<Prediction>(entity =>
            {
                entity.HasIndex(p => new { p.FanId, p.MatchId })
                    .IsUnique();

                entity.HasOne(p => p.Fan)
                    .WithMany(f => f.Predictions)
                    .HasForeignKey(p => p.FanId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Match)
                    .WithMany(m => m.Predictions)
                    .HasForeignKey(p => p.MatchId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Predictions_HomeScore_NonNegative", "[PredictedHomeScore] >= 0");
                    t.HasCheckConstraint("CK_Predictions_AwayScore_NonNegative", "[PredictedAwayScore] >= 0");
                });
            });
        }
    }
}