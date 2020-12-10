using Microsoft.EntityFrameworkCore;
using SecretSantaApplication.Models;

namespace SecretSantaApplication.Data
{
    public class AppContext : DbContext
    {
        public AppContext(DbContextOptions<AppContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<SecretSanta> SecretSantas { get; set; }
    }
}