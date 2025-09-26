using CWA.FacilityManager.Application.DTOs.UserManagement;

namespace CWA.FacilityManager.Application.Interfaces.UserManagement
{
    public interface IRoleManagementService
    {
        // Role CRUD operations
        Task<RoleDto?> GetRoleByIdAsync(string roleId);
        Task<RoleDto?> GetRoleByNameAsync(string roleName);
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<IEnumerable<RoleDto>> GetActiveRolesAsync();
        Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto, string createdBy);
        Task<RoleDto> UpdateRoleAsync(UpdateRoleDto updateRoleDto, string modifiedBy);
        Task<bool> DeleteRoleAsync(string roleId, string deletedBy);
        Task<bool> ActivateRoleAsync(string roleId, string modifiedBy);
        Task<bool> DeactivateRoleAsync(string roleId, string modifiedBy);

        // Role permission management
        Task<bool> AssignPermissionToRoleAsync(string roleId, int permissionId, string grantedBy);
        Task<bool> RemovePermissionFromRoleAsync(string roleId, int permissionId, string removedBy);
        Task<bool> AssignPermissionsToRoleAsync(string roleId, IEnumerable<int> permissionIds, string grantedBy);
        Task<bool> RemovePermissionsFromRoleAsync(string roleId, IEnumerable<int> permissionIds, string removedBy);
        Task<bool> UpdateRolePermissionsAsync(string roleId, IEnumerable<int> permissionIds, string modifiedBy);
        Task<IEnumerable<PermissionDto>> GetRolePermissionsAsync(string roleId);
        Task<bool> RoleHasPermissionAsync(string roleId, int permissionId);

        // Role hierarchy and priority
        Task<bool> UpdateRolePriorityAsync(string roleId, int priority, string modifiedBy);
        Task<IEnumerable<RoleDto>> GetRolesByPriorityAsync();
        Task<RoleDto?> GetHighestPriorityRoleForUserAsync(string userId);

        // Role validation
        Task<bool> RoleExistsAsync(string roleId);
        Task<bool> RoleNameExistsAsync(string roleName, string? excludeRoleId = null);
        Task<bool> CanDeleteRoleAsync(string roleId);
        Task<bool> IsSystemRoleAsync(string roleId);

        // Search and filtering
        Task<IEnumerable<RoleDto>> SearchRolesAsync(string searchTerm);
        Task<IEnumerable<RoleDto>> GetRolesPaginatedAsync(int page, int pageSize, string? searchTerm = null);
        Task<int> GetRolesCountAsync(string? searchTerm = null);
    }
}