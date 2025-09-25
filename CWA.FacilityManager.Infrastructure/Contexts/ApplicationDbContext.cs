using CWA.FacilityManager.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CWA.FacilityManager.Infrastructure.Contexts
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<CalendarTask> CalendarTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure CalendarTask entity
            builder.Entity<CalendarTask>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(e => e.Description)
                    .HasMaxLength(1000);
                
                entity.Property(e => e.Color)
                    .HasMaxLength(50);
                
                entity.Property(e => e.Location)
                    .HasMaxLength(200);
                
                entity.Property(e => e.Category)
                    .HasMaxLength(100);
                
                entity.Property(e => e.Tags)
                    .HasMaxLength(500);
                
                // Configure relationship with ApplicationUser
                entity.HasOne(e => e.AssignedUser)
                    .WithMany()
                    .HasForeignKey(e => e.AssignedUserId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                // Configure indexes for performance
                entity.HasIndex(e => e.StartDate);
                entity.HasIndex(e => e.EndDate);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.AssignedUserId);
                entity.HasIndex(e => e.Category);
            });
        }
    }
}
