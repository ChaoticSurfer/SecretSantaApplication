using Microsoft.EntityFrameworkCore;
using SecretSantaApplication.Models;

namespace SecretSantaApplication.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<SecretSanta> SecretSantas { get; set; }
        public DbSet<Room> Rooms { get; set; }
    }
}