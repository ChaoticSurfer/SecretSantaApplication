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

        public DbSet<UserToRoom> UserToRooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<UserToRoom>()
                .HasKey(userToRoom => new {userToRoom.EmailAddress, userToRoom.Name});

            modelBuilder.Entity<UserToRoom>()
                .HasOne(userToRoom => userToRoom.Room)
                .WithMany(r => r.UserToRooms)
                .HasForeignKey(userToRoom => userToRoom.Name);

            modelBuilder.Entity<UserToRoom>()
                .HasOne(userToRoom => userToRoom.User)
                .WithMany(user => user.UserToRooms)
                .HasForeignKey(userToRoom => userToRoom.EmailAddress);
        }
    }
}