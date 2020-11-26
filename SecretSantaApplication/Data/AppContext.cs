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
    }
}