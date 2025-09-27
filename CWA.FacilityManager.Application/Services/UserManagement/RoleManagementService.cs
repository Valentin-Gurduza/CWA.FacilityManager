using AutoMapper;
using CWA.FacilityManager.Application.DTOs.UserManagement;
using CWA.FacilityManager.Application.Interfaces.UserManagement;
using CWA.FacilityManager.Domain.Models;
using CWA.FacilityManager.Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CWA.FacilityManager.Application.Services.UserManagement
{
    public class RoleManagementService : IRoleManagementService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<RoleManagementService> _logger;

        public RoleManagementService(
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<RoleManagementService> logger)
        {
            _roleManager = roleManager;
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<RoleDto?> GetRoleByIdAsync(string roleId)
        {
            try
            {
                var role = await _context.Roles
                    .Include(r => r.RolePermissions.Where(rp => rp.IsActive))
                    .ThenInclude(rp => rp.Permission)
                    .Include(r => r.UserRoles.Where(ur => ur.IsActive))
                    .FirstOrDefaultAsync(r => r.Id == roleId);

                return role != null ? _mapper.Map<RoleDto>(role) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role by ID: {RoleId}", roleId);
                throw;
            }
        }

        public async Task<RoleDto?> GetRoleByNameAsync(string roleName)
        {
            try
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                return role != null ? await GetRoleByIdAsync(role.Id) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role by name: {RoleName}", roleName);
                throw;
            }
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _context.Roles
                    .Include(r => r.RolePermissions.Where(rp => rp.IsActive))
                    .ThenInclude(rp => rp.Permission)
                    .Include(r => r.UserRoles.Where(ur => ur.IsActive))
                    .ToListAsync();

                return _mapper.Map<IEnumerable<RoleDto>>(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all roles");
                throw;
            }
        }

        public async Task<IEnumerable<RoleDto>> GetActiveRolesAsync()
        {
            try
            {
                var roles = await _context.Roles
                    .Where(r => r.IsActive)
                    .Include(r => r.RolePermissions.Where(rp => rp.IsActive))
                        .ThenInclude(rp => rp.Permission)
                    .Include(r => r.UserRoles.Where(ur => ur.IsActive))
                    .OrderByDescending(r => r.Priority)
                    .ThenBy(r => r.Name)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<RoleDto>>(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active roles");
                throw;
            }
        }

        public async Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto, string createdBy)
        {
            try
            {
                var role = _mapper.Map<ApplicationRole>(createRoleDto);
                role.CreatedBy = createdBy;

                var result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to create role: {errors}");
                }

                // Assign permissions if provided
                if (createRoleDto.PermissionIds.Any())
                {
                    await AssignPermissionsToRoleAsync(role.Id, createRoleDto.PermissionIds, createdBy);
                }

                _logger.LogInformation("Role created successfully: {RoleId} by {CreatedBy}", role.Id, createdBy);
                return (await GetRoleByIdAsync(role.Id))!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role: {RoleName}", createRoleDto.Name);
                throw;
            }
        }

        public async Task<RoleDto> UpdateRoleAsync(UpdateRoleDto updateRoleDto, string modifiedBy)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(updateRoleDto.Id);
                if (role == null)
                    throw new ArgumentException($"Role with ID {updateRoleDto.Id} not found");

                role.Name = updateRoleDto.Name;
                role.Description = updateRoleDto.Description;
                role.Priority = updateRoleDto.Priority;
                role.IsActive = updateRoleDto.IsActive;
                role.ModifiedBy = modifiedBy;
                role.LastModifiedAt = DateTime.UtcNow;

                var result = await _roleManager.UpdateAsync(role);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to update role: {errors}");
                }

                _logger.LogInformation("Role updated successfully: {RoleId} by {ModifiedBy}", role.Id, modifiedBy);
                return (await GetRoleByIdAsync(role.Id))!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role: {RoleId}", updateRoleDto.Id);
                throw;
            }
        }

        public async Task<bool> DeleteRoleAsync(string roleId, string deletedBy)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null) return false;

                if (role.IsSystemRole)
                {
                    throw new InvalidOperationException("Cannot delete system roles");
                }

                // Check if role is in use
                var usersInRole = await _context.UserRoles
                    .Where(ur => ur.RoleId == roleId && ur.IsActive)
                    .CountAsync();

                if (usersInRole > 0)
                {
                    throw new InvalidOperationException($"Cannot delete role. {usersInRole} users are currently assigned to this role.");
                }

                // Soft delete by deactivating
                role.IsActive = false;
                role.ModifiedBy = deletedBy;
                role.LastModifiedAt = DateTime.UtcNow;

                var result = await _roleManager.UpdateAsync(role);
                
                _logger.LogInformation("Role deleted successfully: {RoleId} by {DeletedBy}", roleId, deletedBy);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role: {RoleId}", roleId);
                throw;
            }
        }

        public async Task<bool> ActivateRoleAsync(string roleId, string modifiedBy)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null) return false;

                role.IsActive = true;
                role.ModifiedBy = modifiedBy;
                role.LastModifiedAt = DateTime.UtcNow;

                var result = await _roleManager.UpdateAsync(role);
                
                _logger.LogInformation("Role activated successfully: {RoleId} by {ModifiedBy}", roleId, modifiedBy);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating role: {RoleId}", roleId);
                throw;
            }
        }

        public async Task<bool> DeactivateRoleAsync(string roleId, string modifiedBy)
        {
            return await DeleteRoleAsync(roleId, modifiedBy);
        }

        // Permission management methods
        public async Task<bool> AssignPermissionToRoleAsync(string roleId, int permissionId, string grantedBy)
        {
            try
            {
                var existingRolePermission = await _context.RolePermissions
                    .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

                if (existingRolePermission != null)
                {
                    existingRolePermission.IsActive = true;
                    existingRolePermission.GrantedBy = grantedBy;
                    existingRolePermission.GrantedAt = DateTime.UtcNow;
                }
                else
                {
                    var rolePermission = new RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = permissionId,
                        GrantedBy = grantedBy,
                        IsActive = true
                    };

                    _context.RolePermissions.Add(rolePermission);
                }

                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Permission {PermissionId} assigned to role {RoleId} by {GrantedBy}", permissionId, roleId, grantedBy);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning permission {PermissionId} to role {RoleId}", permissionId, roleId);
                throw;
            }
        }

        public async Task<bool> RemovePermissionFromRoleAsync(string roleId, int permissionId, string removedBy)
        {
            try
            {
                var rolePermission = await _context.RolePermissions
                    .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

                if (rolePermission == null) return false;

                rolePermission.IsActive = false;
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Permission {PermissionId} removed from role {RoleId} by {RemovedBy}", permissionId, roleId, removedBy);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing permission {PermissionId} from role {RoleId}", permissionId, roleId);
                throw;
            }
        }

        public async Task<bool> AssignPermissionsToRoleAsync(string roleId, IEnumerable<int> permissionIds, string grantedBy)
        {
            try
            {
                foreach (var permissionId in permissionIds)
                {
                    await AssignPermissionToRoleAsync(roleId, permissionId, grantedBy);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning permissions to role {RoleId}", roleId);
                throw;
            }
        }

        public async Task<bool> RemovePermissionsFromRoleAsync(string roleId, IEnumerable<int> permissionIds, string removedBy)
        {
            try
            {
                foreach (var permissionId in permissionIds)
                {
                    await RemovePermissionFromRoleAsync(roleId, permissionId, removedBy);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing permissions from role {RoleId}", roleId);
                throw;
            }
        }

        public async Task<bool> UpdateRolePermissionsAsync(string roleId, IEnumerable<int> permissionIds, string modifiedBy)
        {
            try
            {
                // Get current role permissions
                var currentPermissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == roleId && rp.IsActive)
                    .Select(rp => rp.PermissionId)
                    .ToListAsync();

                // Permissions to add
                var permissionsToAdd = permissionIds.Except(currentPermissions);
                
                // Permissions to remove
                var permissionsToRemove = currentPermissions.Except(permissionIds);

                // Add new permissions
                foreach (var permissionId in permissionsToAdd)
                {
                    await AssignPermissionToRoleAsync(roleId, permissionId, modifiedBy);
                }

                // Remove permissions
                foreach (var permissionId in permissionsToRemove)
                {
                    await RemovePermissionFromRoleAsync(roleId, permissionId, modifiedBy);
                }

                _logger.LogInformation("Role permissions updated for role {RoleId} by {ModifiedBy}", roleId, modifiedBy);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role permissions for role {RoleId}", roleId);
                throw;
            }
        }

        public async Task<IEnumerable<PermissionDto>> GetRolePermissionsAsync(string roleId)
        {
            try
            {
                var permissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == roleId && rp.IsActive)
                    .Include(rp => rp.Permission)
                    .Select(rp => rp.Permission)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role permissions: {RoleId}", roleId);
                throw;
            }
        }

        public async Task<bool> RoleHasPermissionAsync(string roleId, int permissionId)
        {
            try
            {
                return await _context.RolePermissions
                    .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId && rp.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking role permission: {RoleId}, {PermissionId}", roleId, permissionId);
                throw;
            }
        }

        // Role hierarchy and priority
        public async Task<bool> UpdateRolePriorityAsync(string roleId, int priority, string modifiedBy)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null) return false;

                role.Priority = priority;
                role.ModifiedBy = modifiedBy;
                role.LastModifiedAt = DateTime.UtcNow;

                var result = await _roleManager.UpdateAsync(role);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role priority: {RoleId}", roleId);
                throw;
            }
        }

        public async Task<IEnumerable<RoleDto>> GetRolesByPriorityAsync()
        {
            try
            {
                var roles = await _context.Roles
                    .Where(r => r.IsActive)
                    .OrderByDescending(r => r.Priority)
                    .ThenBy(r => r.Name)
                    .Include(r => r.RolePermissions.Where(rp => rp.IsActive))
                    .ThenInclude(rp => rp.Permission)
                    .Include(r => r.UserRoles.Where(ur => ur.IsActive))
                    .ToListAsync();

                return _mapper.Map<IEnumerable<RoleDto>>(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles by priority");
                throw;
            }
        }

        public async Task<RoleDto?> GetHighestPriorityRoleForUserAsync(string userId)
        {
            try
            {
                var role = await _context.UserRoles
                    .Where(ur => ur.UserId == userId && ur.IsActive)
                    .Include(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions.Where(rp => rp.IsActive))
                    .ThenInclude(rp => rp.Permission)
                    .OrderByDescending(ur => ur.Role.Priority)
                    .Select(ur => ur.Role)
                    .FirstOrDefaultAsync();

                return role != null ? _mapper.Map<RoleDto>(role) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting highest priority role for user: {UserId}", userId);
                throw;
            }
        }

        // Role validation
        public async Task<bool> RoleExistsAsync(string roleId)
        {
            try
            {
                return await _roleManager.FindByIdAsync(roleId) != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if role exists: {RoleId}", roleId);
                throw;
            }
        }

        public async Task<bool> RoleNameExistsAsync(string roleName, string? excludeRoleId = null)
        {
            try
            {
                var query = _context.Roles.Where(r => r.Name == roleName);
                
                if (!string.IsNullOrEmpty(excludeRoleId))
                {
                    query = query.Where(r => r.Id != excludeRoleId);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if role name exists: {RoleName}", roleName);
                throw;
            }
        }

        public async Task<bool> CanDeleteRoleAsync(string roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null) return false;

                if (role.IsSystemRole) return false;

                var usersInRole = await _context.UserRoles
                    .Where(ur => ur.RoleId == roleId && ur.IsActive)
                    .CountAsync();

                return usersInRole == 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if role can be deleted: {RoleId}", roleId);
                throw;
            }
        }

        public async Task<bool> IsSystemRoleAsync(string roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                return role?.IsSystemRole ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if role is system role: {RoleId}", roleId);
                throw;
            }
        }

        // Search and filtering
        public async Task<IEnumerable<RoleDto>> SearchRolesAsync(string searchTerm)
        {
            try
            {
                var roles = await _context.Roles
                    .Where(r => r.Name!.Contains(searchTerm) || 
                               r.Description!.Contains(searchTerm))
                    .Include(r => r.RolePermissions.Where(rp => rp.IsActive))
                    .ThenInclude(rp => rp.Permission)
                    .Include(r => r.UserRoles.Where(ur => ur.IsActive))
                    .ToListAsync();

                return _mapper.Map<IEnumerable<RoleDto>>(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching roles: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<RoleDto>> GetRolesPaginatedAsync(int page, int pageSize, string? searchTerm = null)
        {
            try
            {
                var query = _context.Roles.AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(r => r.Name!.Contains(searchTerm) || 
                                           r.Description!.Contains(searchTerm));
                }

                var roles = await query
                    .OrderBy(r => r.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Include(r => r.RolePermissions.Where(rp => rp.IsActive))
                    .ThenInclude(rp => rp.Permission)
                    .Include(r => r.UserRoles.Where(ur => ur.IsActive))
                    .ToListAsync();

                return _mapper.Map<IEnumerable<RoleDto>>(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated roles");
                throw;
            }
        }

        public async Task<int> GetRolesCountAsync(string? searchTerm = null)
        {
            try
            {
                var query = _context.Roles.AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(r => r.Name!.Contains(searchTerm) || 
                                           r.Description!.Contains(searchTerm));
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles count");
                throw;
            }
        }
    }
}