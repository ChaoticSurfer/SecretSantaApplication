using Microsoft.EntityFrameworkCore;
using SecretSantaApplication.Models;

namespace SecretSantaApplication.Data
{
    public class Db_AppContext : DbContext
    {
        public Db_AppContext(DbContextOptions<Db_AppContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<SecretSanta> SecretSantas { get; set; }
        public DbSet<Room> Rooms { get; set; }
    }
}