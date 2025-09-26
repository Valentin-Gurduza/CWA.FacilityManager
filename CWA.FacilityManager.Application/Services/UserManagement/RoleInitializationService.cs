using CWA.FacilityManager.Application.Interfaces.UserManagement;
using CWA.FacilityManager.Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CWA.FacilityManager.Domain.Models;

namespace CWA.FacilityManager.Application.Services.UserManagement
{
    public class RoleInitializationService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<RoleInitializationService> _logger;
        private readonly string _systemUserId = "system-initialization";

        public RoleInitializationService(
            ApplicationDbContext context,
            RoleManager<ApplicationRole> roleManager,
            ILogger<RoleInitializationService> logger)
        {
            _context = context;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task InitializeDefaultRolePermissionsAsync()
        {
            try
            {
                _logger.LogInformation("Starting default role permissions initialization...");

                // Check if permissions are already assigned
                var existingRolePermissions = await _context.RolePermissions.AnyAsync();
                if (existingRolePermissions)
                {
                    _logger.LogInformation("Role permissions already exist. Skipping initialization.");
                    return;
                }

                // Get all roles and permissions
                var roles = await _context.Roles.Where(r => r.IsSystemRole).ToListAsync();
                var permissions = await _context.Permissions.ToListAsync();

                foreach (var role in roles)
                {
                    switch (role.Name)
                    {
                        case "Administrator":
                            await AssignAdministratorPermissionsAsync(role.Id, permissions);
                            break;
                        case "Secretary":
                            await AssignSecretaryPermissionsAsync(role.Id, permissions);
                            break;
                        case "Renter":
                            await AssignRenterPermissionsAsync(role.Id, permissions);
                            break;
                    }
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Default role permissions initialization completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during role permissions initialization");
                throw;
            }
        }

        private Task AssignAdministratorPermissionsAsync(string roleId, List<Permission> permissions)
        {
            _logger.LogInformation("Assigning Administrator permissions...");

            // Administrator has ALL permissions
            var rolePermissions = permissions.Select(p => new RolePermission
            {
                RoleId = roleId,
                PermissionId = p.Id,
                GrantedBy = _systemUserId,
                IsActive = true,
                GrantedAt = DateTime.UtcNow
            });

            _context.RolePermissions.AddRange(rolePermissions);
            _logger.LogInformation("Administrator permissions assigned: {Count}", permissions.Count);
            return Task.CompletedTask;
        }

        private Task AssignSecretaryPermissionsAsync(string roleId, List<Permission> permissions)
        {
            _logger.LogInformation("Assigning Secretary permissions...");

            var secretaryPermissions = new[]
            {
                // User Management - Limited (only view users)
                "UserManagement.Users.View",
                
                // Facility Management - Full access
                "FacilityManagement.Facilities.View",
                "FacilityManagement.Facilities.Create",
                "FacilityManagement.Facilities.Edit",
                "FacilityManagement.Facilities.Delete",
                
                // Room Management - Full access
                "FacilityManagement.Rooms.View",
                "FacilityManagement.Rooms.Create",
                "FacilityManagement.Rooms.Edit",
                "FacilityManagement.Rooms.Delete",
                "FacilityManagement.Rooms.ViewSchedule",
                "FacilityManagement.Rooms.ManageSchedule",
                
                // Event Management - Full access
                "EventManagement.Events.View",
                "EventManagement.Events.Create",
                "EventManagement.Events.Edit",
                "EventManagement.Events.Delete",
                "EventManagement.Events.Approve",
                "EventManagement.Events.Reject",
                "EventManagement.Events.Cancel",
                
                // Booking Management - Full access
                "BookingManagement.Bookings.View",
                "BookingManagement.Bookings.Create",
                "BookingManagement.Bookings.Edit",
                "BookingManagement.Bookings.Delete",
                "BookingManagement.Bookings.ViewAll",
                "BookingManagement.Bookings.Approve",
                "BookingManagement.Bookings.Reject",
                
                // Reports - Full access
                "ReportManagement.Reports.View",
                "ReportManagement.Reports.Generate",
                "ReportManagement.Reports.Export"
            };

            var relevantPermissions = permissions.Where(p => secretaryPermissions.Contains(p.Name)).ToList();
            
            var rolePermissions = relevantPermissions.Select(p => new RolePermission
            {
                RoleId = roleId,
                PermissionId = p.Id,
                GrantedBy = _systemUserId,
                IsActive = true,
                GrantedAt = DateTime.UtcNow
            });

            _context.RolePermissions.AddRange(rolePermissions);
            _logger.LogInformation("Secretary permissions assigned: {Count}", relevantPermissions.Count);
            return Task.CompletedTask;
        }

        private Task AssignRenterPermissionsAsync(string roleId, List<Permission> permissions)
        {
            _logger.LogInformation("Assigning Renter permissions...");

            var renterPermissions = new[]
            {
                // Facility Management - View only
                "FacilityManagement.Facilities.View",
                
                // Room Management - View and schedule only
                "FacilityManagement.Rooms.View",
                "FacilityManagement.Rooms.ViewSchedule",
                
                // Event Management - Limited (own events only)
                "EventManagement.Events.View",
                "EventManagement.Events.Create",
                "EventManagement.Events.Edit",
                
                // Booking Management - Limited (own bookings only)
                "BookingManagement.Bookings.View",
                "BookingManagement.Bookings.Create",
                "BookingManagement.Bookings.Edit",
                
                // Reports - View only (own data)
                "ReportManagement.Reports.View"
            };

            var relevantPermissions = permissions.Where(p => renterPermissions.Contains(p.Name)).ToList();
            
            var rolePermissions = relevantPermissions.Select(p => new RolePermission
            {
                RoleId = roleId,
                PermissionId = p.Id,
                GrantedBy = _systemUserId,
                IsActive = true,
                GrantedAt = DateTime.UtcNow
            });

            _context.RolePermissions.AddRange(rolePermissions);
            _logger.LogInformation("Renter permissions assigned: {Count}", relevantPermissions.Count);
            return Task.CompletedTask;
        }

        public async Task<Dictionary<string, List<string>>> GetRolePermissionsSummaryAsync()
        {
            try
            {
                var result = new Dictionary<string, List<string>>();

                var rolePermissions = await _context.RolePermissions
                    .Include(rp => rp.Role)
                    .Include(rp => rp.Permission)
                    .Where(rp => rp.IsActive && rp.Role.IsSystemRole)
                    .GroupBy(rp => rp.Role.Name)
                    .ToListAsync();

                foreach (var group in rolePermissions)
                {
                    result[group.Key!] = group.Select(rp => rp.Permission.Name).OrderBy(n => n).ToList();
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role permissions summary");
                throw;
            }
        }

        public async Task<bool> ValidateRolePermissionsAsync()
        {
            try
            {
                var roles = await _context.Roles.Where(r => r.IsSystemRole).ToListAsync();
                var allValid = true;

                foreach (var role in roles)
                {
                    var permissionCount = await _context.RolePermissions
                        .Where(rp => rp.RoleId == role.Id && rp.IsActive)
                        .CountAsync();

                    var expectedCount = role.Name switch
                    {
                        "Administrator" => await _context.Permissions.CountAsync(),
                        "Secretary" => 21, // Expected number of secretary permissions
                        "Renter" => 11,    // Expected number of renter permissions
                        _ => 0
                    };

                    if (permissionCount != expectedCount)
                    {
                        _logger.LogWarning("Role {RoleName} has {ActualCount} permissions, expected {ExpectedCount}",
                            role.Name, permissionCount, expectedCount);
                        allValid = false;
                    }
                }

                return allValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating role permissions");
                throw;
            }
        }
    }
}