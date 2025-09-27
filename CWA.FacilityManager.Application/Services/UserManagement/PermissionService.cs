using AutoMapper;
using CWA.FacilityManager.Application.DTOs.UserManagement;
using CWA.FacilityManager.Application.Interfaces.UserManagement;
using CWA.FacilityManager.Domain.Models;
using CWA.FacilityManager.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CWA.FacilityManager.Application.Services.UserManagement
{
    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<PermissionService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PermissionDto?> GetPermissionByIdAsync(int permissionId)
        {
            try
            {
                var permission = await _context.Permissions
                    .FirstOrDefaultAsync(p => p.Id == permissionId);

                return permission != null ? _mapper.Map<PermissionDto>(permission) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permission by ID: {PermissionId}", permissionId);
                throw;
            }
        }

        public async Task<PermissionDto?> GetPermissionByNameAsync(string permissionName)
        {
            try
            {
                var permission = await _context.Permissions
                    .FirstOrDefaultAsync(p => p.Name == permissionName);

                return permission != null ? _mapper.Map<PermissionDto>(permission) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permission by name: {PermissionName}", permissionName);
                throw;
            }
        }

        public async Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync()
        {
            try
            {
                var permissions = await _context.Permissions
                    .OrderBy(p => p.Module)
                    .ThenBy(p => p.Resource)
                    .ThenBy(p => p.Action)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all permissions");
                throw;
            }
        }

        public async Task<IEnumerable<PermissionDto>> GetPermissionsByModuleAsync(string module)
        {
            try
            {
                var permissions = await _context.Permissions
                    .Where(p => p.Module == module)
                    .OrderBy(p => p.Resource)
                    .ThenBy(p => p.Action)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permissions by module: {Module}", module);
                throw;
            }
        }

        public async Task<IEnumerable<PermissionDto>> GetPermissionsByResourceAsync(string resource)
        {
            try
            {
                var permissions = await _context.Permissions
                    .Where(p => p.Resource == resource)
                    .OrderBy(p => p.Module)
                    .ThenBy(p => p.Action)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permissions by resource: {Resource}", resource);
                throw;
            }
        }

        public async Task<IEnumerable<PermissionGroupDto>> GetPermissionsGroupedAsync()
        {
            try
            {
                var permissions = await _context.Permissions
                    .OrderBy(p => p.Module)
                    .ThenBy(p => p.Resource)
                    .ThenBy(p => p.Action)
                    .ToListAsync();

                var groupedPermissions = permissions
                    .GroupBy(p => p.Module)
                    .Select(moduleGroup => new PermissionGroupDto
                    {
                        Module = moduleGroup.Key,
                        ModuleDisplayName = FormatDisplayName(moduleGroup.Key),
                        Resources = moduleGroup
                            .GroupBy(p => p.Resource)
                            .Select(resourceGroup => new PermissionResourceDto
                            {
                                Resource = resourceGroup.Key,
                                ResourceDisplayName = FormatDisplayName(resourceGroup.Key),
                                Permissions = _mapper.Map<List<PermissionDto>>(resourceGroup)
                            })
                            .ToList()
                    })
                    .ToList();

                return groupedPermissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting grouped permissions");
                throw;
            }
        }

        public async Task<PermissionDto> CreatePermissionAsync(CreatePermissionDto createPermissionDto)
        {
            try
            {
                var permission = _mapper.Map<Permission>(createPermissionDto);
                
                _context.Permissions.Add(permission);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Permission created successfully: {PermissionId}", permission.Id);
                return _mapper.Map<PermissionDto>(permission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating permission: {PermissionName}", createPermissionDto.Name);
                throw;
            }
        }

        public async Task<bool> DeletePermissionAsync(int permissionId)
        {
            try
            {
                var permission = await _context.Permissions
                    .Include(p => p.RolePermissions)
                    .FirstOrDefaultAsync(p => p.Id == permissionId);

                if (permission == null) return false;

                if (permission.IsSystemPermission)
                {
                    throw new InvalidOperationException("Cannot delete system permissions");
                }

                // Check if permission is in use
                var rolesUsingPermission = await _context.RolePermissions
                    .Where(rp => rp.PermissionId == permissionId && rp.IsActive)
                    .CountAsync();

                if (rolesUsingPermission > 0)
                {
                    throw new InvalidOperationException($"Cannot delete permission. {rolesUsingPermission} roles are currently using this permission.");
                }

                _context.Permissions.Remove(permission);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Permission deleted successfully: {PermissionId}", permissionId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting permission: {PermissionId}", permissionId);
                throw;
            }
        }

        // Permission validation
        public async Task<bool> PermissionExistsAsync(int permissionId)
        {
            try
            {
                return await _context.Permissions
                    .AnyAsync(p => p.Id == permissionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if permission exists: {PermissionId}", permissionId);
                throw;
            }
        }

        public async Task<bool> PermissionNameExistsAsync(string permissionName)
        {
            try
            {
                return await _context.Permissions
                    .AnyAsync(p => p.Name == permissionName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if permission name exists: {PermissionName}", permissionName);
                throw;
            }
        }

        public async Task<bool> IsSystemPermissionAsync(int permissionId)
        {
            try
            {
                var permission = await _context.Permissions
                    .FirstOrDefaultAsync(p => p.Id == permissionId);

                return permission?.IsSystemPermission ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if permission is system permission: {PermissionId}", permissionId);
                throw;
            }
        }

        public async Task<bool> CanDeletePermissionAsync(int permissionId)
        {
            try
            {
                var permission = await _context.Permissions
                    .FirstOrDefaultAsync(p => p.Id == permissionId);

                if (permission == null) return false;
                if (permission.IsSystemPermission) return false;

                var rolesUsingPermission = await _context.RolePermissions
                    .Where(rp => rp.PermissionId == permissionId && rp.IsActive)
                    .CountAsync();

                return rolesUsingPermission == 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if permission can be deleted: {PermissionId}", permissionId);
                throw;
            }
        }

        // Permission policy management
        public async Task<string> GetPolicyNameAsync(int permissionId)
        {
            try
            {
                var permission = await _context.Permissions
                    .FirstOrDefaultAsync(p => p.Id == permissionId);

                return permission?.PolicyName ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting policy name for permission: {PermissionId}", permissionId);
                throw;
            }
        }

        public async Task<PermissionDto?> GetPermissionByPolicyNameAsync(string policyName)
        {
            try
            {
                var permission = await _context.Permissions
                    .FirstOrDefaultAsync(p => p.PolicyName == policyName);

                return permission != null ? _mapper.Map<PermissionDto>(permission) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permission by policy name: {PolicyName}", policyName);
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetAllPolicyNamesAsync()
        {
            try
            {
                var policyNames = await _context.Permissions
                    .Select(p => p.PolicyName)
                    .ToListAsync();

                return policyNames;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all policy names");
                throw;
            }
        }

        // Seed and initialization
        public async Task SeedDefaultPermissionsAsync()
        {
            try
            {
                var existingPermissions = await _context.Permissions.CountAsync();
                if (existingPermissions > 0)
                {
                    _logger.LogInformation("Permissions already seeded. Skipping seed operation.");
                    return;
                }

                _logger.LogInformation("Seeding default permissions...");
                
                // The permissions are already seeded in the DbContext via ModelBuilder
                // This method can be used to add additional permissions programmatically
                
                await _context.SaveChangesAsync();
                _logger.LogInformation("Default permissions seeded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding default permissions");
                throw;
            }
        }

        public async Task<bool> InitializeSystemPermissionsAsync()
        {
            try
            {
                // This method can be used to ensure all system permissions are properly initialized
                // For now, we'll just verify that the seeded permissions exist
                
                var systemPermissions = await _context.Permissions
                    .Where(p => p.IsSystemPermission)
                    .CountAsync();

                if (systemPermissions == 0)
                {
                    await SeedDefaultPermissionsAsync();
                }

                _logger.LogInformation("System permissions initialized. Count: {SystemPermissionsCount}", systemPermissions);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing system permissions");
                throw;
            }
        }

        // Search and utilities
        public async Task<IEnumerable<PermissionDto>> SearchPermissionsAsync(string searchTerm)
        {
            try
            {
                var permissions = await _context.Permissions
                    .Where(p => p.Name.Contains(searchTerm) ||
                               p.DisplayName.Contains(searchTerm) ||
                               p.Description!.Contains(searchTerm) ||
                               p.Module.Contains(searchTerm) ||
                               p.Resource.Contains(searchTerm))
                    .OrderBy(p => p.Module)
                    .ThenBy(p => p.Resource)
                    .ThenBy(p => p.Action)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching permissions: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetModulesAsync()
        {
            try
            {
                var modules = await _context.Permissions
                    .Select(p => p.Module)
                    .Distinct()
                    .OrderBy(m => m)
                    .ToListAsync();

                return modules;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting modules");
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetResourcesAsync()
        {
            try
            {
                var resources = await _context.Permissions
                    .Select(p => p.Resource)
                    .Distinct()
                    .OrderBy(r => r)
                    .ToListAsync();

                return resources;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting resources");
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetActionsAsync()
        {
            try
            {
                var actions = await _context.Permissions
                    .Select(p => p.Action)
                    .Distinct()
                    .OrderBy(a => a)
                    .ToListAsync();

                return actions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting actions");
                throw;
            }
        }

        private static string FormatDisplayName(string name)
        {
            // Convert PascalCase to Title Case with spaces
            // UserManagement -> User Management
            return System.Text.RegularExpressions.Regex.Replace(name, "([A-Z])", " $1").Trim();
        }
    }
}