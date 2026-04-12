using ArabFootball.Api.Features.Enums;
using ArabFootball.Api.Shared.Data;
using ArabFootball.Api.Shared.Entity;
using Microsoft.EntityFrameworkCore;

namespace ArabFootball.Api.Shared.Seeding
{
    public static class DbInitializer
    {
        public static async Task SeedAdminsAsync(AppDBContext context)
        {
            var admins = new List<Admin>
            {
                new Admin
                {
                    Username = "Abdualrhman",
                    Email = "a@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Role = UserRole.Admin
                },
                new Admin
                {
                    Username = "admin2",
                    Email = "admin2@arabfootball.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Role = UserRole.Admin
                }
            };

            foreach (var admin in admins)
            {
                var exists = await context.Admins.AnyAsync(a => a.Email == admin.Email);
                if (!exists)
                {
                    context.Admins.Add(admin);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}