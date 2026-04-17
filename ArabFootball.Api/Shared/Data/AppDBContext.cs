using ArabFootball.Api.Shared.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ArabFootball.Api.Shared.Data
{
    public class AppDBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Fan> Fans { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<Prediction> Predictions { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Admin>().ToTable("Admins");
            modelBuilder.Entity<Fan>().ToTable("Fans");




            base.OnModelCreating(modelBuilder);

            

            
            modelBuilder.Entity<Follow>(entity =>
            {
                
                entity.HasKey(f => new { f.ObserverId, f.TargetId });

                
                entity.HasOne(f => f.Observer)
                    .WithMany() 
                    .HasForeignKey(f => f.ObserverId)
                    .OnDelete(DeleteBehavior.Restrict); 

                entity.HasOne(f => f.Target)
                    .WithMany() 
                    .HasForeignKey(f => f.TargetId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
