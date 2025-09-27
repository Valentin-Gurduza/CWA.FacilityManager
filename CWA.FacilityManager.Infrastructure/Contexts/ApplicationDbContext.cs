using CWA.FacilityManager.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CWA.FacilityManager.Infrastructure.Contexts
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<UserHistory> UserHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure ApplicationUser
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasMany(u => u.Bookings)
                    .WithOne(b => b.User)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.UserHistories)
                    .WithOne(h => h.User)
                    .HasForeignKey(h => h.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Room
            builder.Entity<Room>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.HourlyRate)
                    .HasPrecision(18, 2);

                entity.HasMany(r => r.Bookings)
                    .WithOne(b => b.Room)
                    .HasForeignKey(b => b.RoomId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Booking
            builder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.TotalCost)
                    .HasPrecision(18, 2);

                entity.HasIndex(b => new { b.RoomId, b.StartDate, b.EndDate })
                    .HasDatabaseName("IX_Booking_Room_DateRange");

                entity.HasIndex(b => b.UserId)
                    .HasDatabaseName("IX_Booking_UserId");
            });

            // Configure UserHistory
            builder.Entity<UserHistory>(entity =>
            {
                entity.HasKey(h => h.Id);
                entity.HasIndex(h => new { h.UserId, h.CreatedAt })
                    .HasDatabaseName("IX_UserHistory_User_Date");
            });
        }
    }
}
