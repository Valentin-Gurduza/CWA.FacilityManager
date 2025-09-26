using CWA.FacilityManager.Application.DTOs.UserManagement;

namespace CWA.FacilityManager.Application.Interfaces.UserManagement
{
    public interface IPermissionService
    {
        // Permission CRUD operations
        Task<PermissionDto?> GetPermissionByIdAsync(int permissionId);
        Task<PermissionDto?> GetPermissionByNameAsync(string permissionName);
        Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync();
        Task<IEnumerable<PermissionDto>> GetPermissionsByModuleAsync(string module);
        Task<IEnumerable<PermissionDto>> GetPermissionsByResourceAsync(string resource);
        Task<IEnumerable<PermissionGroupDto>> GetPermissionsGroupedAsync();
        Task<PermissionDto> CreatePermissionAsync(CreatePermissionDto createPermissionDto);
        Task<bool> DeletePermissionAsync(int permissionId);

        // Permission validation
        Task<bool> PermissionExistsAsync(int permissionId);
        Task<bool> PermissionNameExistsAsync(string permissionName);
        Task<bool> IsSystemPermissionAsync(int permissionId);
        Task<bool> CanDeletePermissionAsync(int permissionId);

        // Permission policy management
        Task<string> GetPolicyNameAsync(int permissionId);
        Task<PermissionDto?> GetPermissionByPolicyNameAsync(string policyName);
        Task<IEnumerable<string>> GetAllPolicyNamesAsync();

        // Seed and initialization
        Task SeedDefaultPermissionsAsync();
        Task<bool> InitializeSystemPermissionsAsync();

        // Search and utilities
        Task<IEnumerable<PermissionDto>> SearchPermissionsAsync(string searchTerm);
        Task<IEnumerable<string>> GetModulesAsync();
        Task<IEnumerable<string>> GetResourcesAsync();
        Task<IEnumerable<string>> GetActionsAsync();
    }
}