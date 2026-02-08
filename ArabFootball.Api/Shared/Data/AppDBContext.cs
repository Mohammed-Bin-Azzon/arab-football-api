using ArabFootball.Api.Shared.Entity;
using Microsoft.EntityFrameworkCore;

namespace ArabFootball.Api.Shared.Data
{
    public class AppDBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
    }
}
