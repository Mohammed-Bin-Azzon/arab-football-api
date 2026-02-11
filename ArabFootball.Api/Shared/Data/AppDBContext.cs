using ArabFootball.Api.Shared.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ArabFootball.Api.Shared.Data
{
    public class AppDBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Fan> Fans { get; set; }
    }
}
