using System;
using System.Collections.Generic;
using CWA.FacilityManager.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CWA.FacilityManager.Infrastructure.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Rooms-task-cwa DbSets
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Event> Events { get; set; }

        // Merge-Test DbSets
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Merge-Test: Configure ApplicationUser
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
            });

            // Merge-Test: Configure ApplicationRole
            builder.Entity<ApplicationRole>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.CreatedBy).HasMaxLength(450);
                entity.Property(e => e.ModifiedBy).HasMaxLength(450);
                entity.Property(e => e.RoleType).HasConversion<int>(); // Store enum as int
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.Priority);
                entity.HasIndex(e => e.IsSystemRole);
                entity.HasIndex(e => e.RoleType);
            });

            // Merge-Test: Configure UserRole (junction table)
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

            // Merge-Test: Configure Permission
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

            // Merge-Test: Configure RolePermission (junction table)
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

            // Rooms-task-cwa: Configure Building entity
            builder.Entity<Building>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Address).HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // Rooms-task-cwa: Configure Room entity
            builder.Entity<Room>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.RoomNumber).HasMaxLength(20);
                entity.Property(e => e.Description).HasMaxLength(500);
                // Store Activity as integer (enum underlying value)
                entity.Property(e => e.Activity).HasConversion<int>();
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Time).IsRequired();

                entity.HasOne(r => r.Building)
                    .WithMany(b => b.Rooms)
                    .HasForeignKey(r => r.BuildingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Rooms-task-cwa: Configure Event entity
            builder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Organizer).HasMaxLength(100);
                entity.Property(e => e.ContactEmail).HasMaxLength(200);
                entity.Property(e => e.Type).HasConversion<string>();

                entity.HasOne(e => e.Room)
                    .WithMany(r => r.Events)
                    .HasForeignKey(e => e.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.CreatedBy)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedById)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Merge-Test: Seed default permissions and roles
            SeedDefaultPermissions(builder);
            SeedDefaultRoles(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // Enable sensitive data logging in development to help with debugging
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                optionsBuilder.EnableSensitiveDataLogging();
            }

            // Configure query tracking behavior
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
        }

        private static void SeedDefaultPermissions(ModelBuilder builder)
        {
            var permissions = new List<Permission>();
            var permissionId = 1;
            // Use a fixed date for seeding to avoid migration issues
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
                    Id = "550e8400-e29b-41d4-a716-446655440001", // Static GUID for Administrator
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
                    Id = "550e8400-e29b-41d4-a716-446655440002", // Static GUID for Secretary
                    Name = "Secretary",
                    NormalizedName = "SECRETARY",
                    Description = "Secretary with access to manage bookings, view reports, and manage facilities",
                    Priority = 80,
                    IsSystemRole = true,
                    IsActive = true,
                    RoleType = Domain.Enums.RoleType.Secretary,
                    CreatedAt = seedDate
                },
                new ApplicationRole
                {
                    Id = "550e8400-e29b-41d4-a716-446655440003", // Static GUID for Renter
                    Name = "Renter",
                    NormalizedName = "RENTER",
                    Description = "Renter with access to view facilities and create booking requests",
                    Priority = 60,
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