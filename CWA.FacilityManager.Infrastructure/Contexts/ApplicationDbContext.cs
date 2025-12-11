using System;
using System.Collections.Generic;
using CWA.FacilityManager.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CWA.FacilityManager.Infrastructure.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string,
        IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>,
        IdentityUserToken<string>>
    {
        // Constructor
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets from all branches
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<UserHistory> UserHistories { get; set; }
        public DbSet<CalendarTask> CalendarTasks { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure ApplicationUser (combining both approaches)
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.JobTitle).HasMaxLength(100);
                entity.Property(e => e.CreatedBy).HasMaxLength(450);
                entity.Property(e => e.ModifiedBy).HasMaxLength(450);
                entity.Property(e => e.ProfilePictureUrl).HasMaxLength(500);
                
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.Department);

                // Navigation properties from User-profile-history
                entity.HasMany(u => u.Bookings)
                    .WithOne(b => b.User)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Restrict); // Changed from Cascade to Restrict

                entity.HasMany(u => u.UserHistories)
                    .WithOne(h => h.User)
                    .HasForeignKey(h => h.UserId)
                    .OnDelete(DeleteBehavior.Restrict); // Changed from Cascade to Restrict
            });

            // Configure ApplicationRole
            builder.Entity<ApplicationRole>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.CreatedBy).HasMaxLength(450);
                entity.Property(e => e.ModifiedBy).HasMaxLength(450);
                entity.Property(e => e.RoleType).HasConversion<int>();
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.Priority);
                entity.HasIndex(e => e.IsSystemRole);
                entity.HasIndex(e => e.RoleType);
            });

            // Configure UserRole (junction table)
            builder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });

                entity.Property(e => e.AssignedBy).HasMaxLength(450);
                entity.Property(e => e.Notes).HasMaxLength(1000);

                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.AssignedAt);
                entity.HasIndex(e => e.ExpiresAt);

                entity.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Room (combining both approaches)
            builder.Entity<Room>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.RoomNumber).HasMaxLength(20);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Location).HasMaxLength(200);
                entity.Property(e => e.Equipment).HasMaxLength(1000);
                entity.Property(e => e.Amenities).HasMaxLength(1000);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.Activity).HasConversion<int>();
                entity.Property(r => r.HourlyRate).HasPrecision(18, 2);

                // Relationships
                entity.HasMany(r => r.Bookings)
                    .WithOne(b => b.Room)
                    .HasForeignKey(b => b.RoomId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Building)
                    .WithMany(b => b.Rooms)
                    .HasForeignKey(r => r.BuildingId)
                    .OnDelete(DeleteBehavior.Restrict); // Changed from Cascade to Restrict

                entity.HasMany(r => r.Events)
                    .WithOne(e => e.Room)
                    .HasForeignKey(e => e.RoomId)
                    .OnDelete(DeleteBehavior.Restrict); // Changed from Cascade to Restrict
            });

            // Configure Booking (from User-profile-history)
            builder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.TotalCost).HasPrecision(18, 2);

                entity.HasIndex(b => new { b.RoomId, b.StartDate, b.EndDate })
                    .HasDatabaseName("IX_Booking_Room_DateRange");

                entity.HasIndex(b => b.UserId)
                    .HasDatabaseName("IX_Booking_UserId");
            });

            // Configure UserHistory (from User-profile-history)
            builder.Entity<UserHistory>(entity =>
            {
                entity.HasKey(h => h.Id);
                entity.HasIndex(h => new { h.UserId, h.CreatedAt })
                    .HasDatabaseName("IX_UserHistory_User_Date");
            });

            // Configure Permission
            builder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Module).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Resource).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(50);

                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => new { e.Module, e.Resource, e.Action }).IsUnique();
                entity.HasIndex(e => e.Module);
                entity.HasIndex(e => e.Resource);
                entity.HasIndex(e => e.IsSystemPermission);
            });

            // Configure RolePermission (junction table)
            builder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.GrantedBy).HasMaxLength(450);

                entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.GrantedAt);

                entity.HasOne(rp => rp.Role)
                    .WithMany(r => r.RolePermissions)
                    .HasForeignKey(rp => rp.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rp => rp.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(rp => rp.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Building entity
            builder.Entity<Building>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Address).HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // Configure Event entity
            builder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Organizer).HasMaxLength(100);
                entity.Property(e => e.OrganizerCompany).HasMaxLength(200);
                entity.Property(e => e.ContactName).HasMaxLength(100);
                entity.Property(e => e.ContactPhone).HasMaxLength(20);
                entity.Property(e => e.ContactEmail).HasMaxLength(200);
                entity.Property(e => e.Type).HasConversion<string>();
                entity.Property(e => e.Status).HasConversion<int>();

                // Fixed cascade delete paths
                entity.HasOne(e => e.CreatedBy)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedById)
                    .OnDelete(DeleteBehavior.NoAction); // Changed from SetNull to NoAction

                entity.HasOne(e => e.ApprovedBy)
                    .WithMany()
                    .HasForeignKey(e => e.ApprovedById)
                    .OnDelete(DeleteBehavior.NoAction); // Changed from SetNull to NoAction
                
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => new { e.RoomId, e.StartDateTime, e.EndDateTime });
            });

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

                entity.HasOne(e => e.AssignedUser)
                    .WithMany()
                    .HasForeignKey(e => e.AssignedUserId)
                    .OnDelete(DeleteBehavior.NoAction); // Changed from SetNull to NoAction

                entity.HasIndex(e => e.StartDate);
                entity.HasIndex(e => e.EndDate);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.AssignedUserId);
                entity.HasIndex(e => e.Category);
            });

            // Configure AuditLog entity
            builder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(450);
                entity.Property(e => e.ActionType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Entity).IsRequired().HasMaxLength(100);
                entity.Property(e => e.EntityId).HasMaxLength(100);
                entity.Property(e => e.Data).HasMaxLength(4000);
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.Property(e => e.UserAgent).HasMaxLength(500);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.AuditLogs)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict); // Changed from Cascade to Restrict

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Timestamp);
                entity.HasIndex(e => new { e.Entity, e.EntityId });
                entity.HasIndex(e => e.ActionType);
            });

            // Seed data
            SeedDefaultPermissions(builder);
            SeedDefaultRoles(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                optionsBuilder.EnableSensitiveDataLogging();
            }

            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
        }

        private static void SeedDefaultPermissions(ModelBuilder builder)
        {
            var permissions = new List<Permission>();
            var permissionId = 1;
            var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // User Management Permissions
            var userManagementActions = new[] { "View", "Create", "Edit", "Delete", "Assign", "Unassign" };
            foreach (var action in userManagementActions)
            {
                permissions.Add(new Permission
                {
                    Id = permissionId++,
                    Name = $"UserManagement.Users.{action}",
                    DisplayName = $"{action} Users",
                    Description = $"Permission to {action.ToLower()} users",
                    Module = "UserManagement",
                    Resource = "Users",
                    Action = action,
                    IsSystemPermission = true,
                    CreatedAt = seedDate
                });
            }

            // Role Management Permissions
            var roleManagementActions = new[] { "View", "Create", "Edit", "Delete", "AssignPermissions" };
            foreach (var action in roleManagementActions)
            {
                permissions.Add(new Permission
                {
                    Id = permissionId++,
                    Name = $"UserManagement.Roles.{action}",
                    DisplayName = $"{action} Roles",
                    Description = $"Permission to {action.ToLower()} roles",
                    Module = "UserManagement",
                    Resource = "Roles",
                    Action = action,
                    IsSystemPermission = true,
                    CreatedAt = seedDate
                });
            }

            // Permission Management Permissions
            var permissionManagementActions = new[] { "View", "Create", "Delete" };
            foreach (var action in permissionManagementActions)
            {
                permissions.Add(new Permission
                {
                    Id = permissionId++,
                    Name = $"UserManagement.Permissions.{action}",
                    DisplayName = $"{action} Permissions",
                    Description = $"Permission to {action.ToLower()} permissions",
                    Module = "UserManagement",
                    Resource = "Permissions",
                    Action = action,
                    IsSystemPermission = true,
                    CreatedAt = seedDate
                });
            }

            // Facility Management Permissions
            var facilityActions = new[] { "View", "Create", "Edit", "Delete" };
            foreach (var action in facilityActions)
            {
                permissions.Add(new Permission
                {
                    Id = permissionId++,
                    Name = $"FacilityManagement.Facilities.{action}",
                    DisplayName = $"{action} Facilities",
                    Description = $"Permission to {action.ToLower()} facilities",
                    Module = "FacilityManagement",
                    Resource = "Facilities",
                    Action = action,
                    IsSystemPermission = true,
                    CreatedAt = seedDate
                });
            }

            // Room Management Permissions
            var roomActions = new[] { "View", "Create", "Edit", "Delete", "ViewSchedule", "ManageSchedule" };
            foreach (var action in roomActions)
            {
                permissions.Add(new Permission
                {
                    Id = permissionId++,
                    Name = $"FacilityManagement.Rooms.{action}",
                    DisplayName = $"{action} Rooms",
                    Description = $"Permission to {action.ToLower()} rooms",
                    Module = "FacilityManagement",
                    Resource = "Rooms",
                    Action = action,
                    IsSystemPermission = true,
                    CreatedAt = seedDate
                });
            }

            // Event/Booking Management Permissions
            var eventActions = new[] { "View", "Create", "Edit", "Delete", "Approve", "Reject", "Cancel" };
            foreach (var action in eventActions)
            {
                permissions.Add(new Permission
                {
                    Id = permissionId++,
                    Name = $"EventManagement.Events.{action}",
                    DisplayName = $"{action} Events",
                    Description = $"Permission to {action.ToLower()} events",
                    Module = "EventManagement",
                    Resource = "Events",
                    Action = action,
                    IsSystemPermission = true,
                    CreatedAt = seedDate
                });
            }

            // Booking Management Permissions
            var bookingActions = new[] { "View", "Create", "Edit", "Delete", "ViewAll", "Approve", "Reject" };
            foreach (var action in bookingActions)
            {
                permissions.Add(new Permission
                {
                    Id = permissionId++,
                    Name = $"BookingManagement.Bookings.{action}",
                    DisplayName = $"{action} Bookings",
                    Description = $"Permission to {action.ToLower()} bookings",
                    Module = "BookingManagement",
                    Resource = "Bookings",
                    Action = action,
                    IsSystemPermission = true,
                    CreatedAt = seedDate
                });
            }

            // Report Permissions
            var reportActions = new[] { "View", "Generate", "Export" };
            foreach (var action in reportActions)
            {
                permissions.Add(new Permission
                {
                    Id = permissionId++,
                    Name = $"ReportManagement.Reports.{action}",
                    DisplayName = $"{action} Reports",
                    Description = $"Permission to {action.ToLower()} reports",
                    Module = "ReportManagement",
                    Resource = "Reports",
                    Action = action,
                    IsSystemPermission = true,
                    CreatedAt = seedDate
                });
            }

            // System Configuration Permissions
            var configActions = new[] { "View", "Edit", "Backup", "Restore" };
            foreach (var action in configActions)
            {
                permissions.Add(new Permission
                {
                    Id = permissionId++,
                    Name = $"SystemManagement.Configuration.{action}",
                    DisplayName = $"{action} Configuration",
                    Description = $"Permission to {action.ToLower()} system configuration",
                    Module = "SystemManagement",
                    Resource = "Configuration",
                    Action = action,
                    IsSystemPermission = true,
                    CreatedAt = seedDate
                });
            }

            builder.Entity<Permission>().HasData(permissions);
        }

        private static void SeedDefaultRoles(ModelBuilder builder)
        {
            var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var roles = new[]
            {
                new ApplicationRole
                {
                    Id = "550e8400-e29b-41d4-a716-446655440001",
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR",
                    Description = "System administrator with full access to all features",
                    Priority = 100,
                    IsSystemRole = true,
                    IsActive = true,
                    RoleType = Domain.Enums.RoleType.Administrator,
                    CreatedAt = seedDate
                },
                new ApplicationRole
                {
                    Id = "550e8400-e29b-41d4-a716-446655440002",
                    Name = "Secretary",
                    NormalizedName = "SECRETARY",
                    Description = "Secretary with access to manage bookings, view reports, and manage facilities",
                    Priority = 50,
                    IsSystemRole = true,
                    IsActive = true,
                    RoleType = Domain.Enums.RoleType.Secretary,
                    CreatedAt = seedDate
                },
                new ApplicationRole
                {
                    Id = "550e8400-e29b-41d4-a716-446655440003",
                    Name = "Renter",
                    NormalizedName = "RENTER",
                    Description = "Renter with access to view facilities and create booking requests",
                    Priority = 10,
                    IsSystemRole = true,
                    IsActive = true,
                    RoleType = Domain.Enums.RoleType.Renter,
                    CreatedAt = seedDate
                }
            };

            builder.Entity<ApplicationRole>().HasData(roles);
        }
    }
}