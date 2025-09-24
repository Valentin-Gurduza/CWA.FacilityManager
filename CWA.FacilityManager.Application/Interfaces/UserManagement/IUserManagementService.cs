using CWA.FacilityManager.Application.DTOs.UserManagement;
using CWA.FacilityManager.Domain.Models;

namespace CWA.FacilityManager.Application.Interfaces.UserManagement
{
    public interface IUserManagementService
    {
        // User CRUD operations
        Task<UserDto?> GetUserByIdAsync(string userId);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<UserDto?> GetUserByUsernameAsync(string username);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<IEnumerable<UserDto>> GetActiveUsersAsync();
        Task<IEnumerable<UserDto>> GetInactiveUsersAsync();
        Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string roleName);
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto, string createdBy);
        Task<UserDto> UpdateUserAsync(UpdateUserDto updateUserDto, string modifiedBy);
        Task<bool> DeleteUserAsync(string userId, string deletedBy);
        Task<bool> ActivateUserAsync(string userId, string modifiedBy);
        Task<bool> DeactivateUserAsync(string userId, string modifiedBy);

        // User role management
        Task<bool> AssignRoleToUserAsync(string userId, string roleId, string assignedBy, DateTime? expiresAt = null, string? notes = null);
        Task<bool> RemoveRoleFromUserAsync(string userId, string roleId, string removedBy);
        Task<IEnumerable<string>> GetUserRolesAsync(string userId);
        Task<IEnumerable<UserDto>> GetUsersInRoleAsync(string roleId);

        // User permissions
        Task<IEnumerable<string>> GetUserPermissionsAsync(string userId);
        Task<bool> UserHasPermissionAsync(string userId, string permission);
        Task<bool> UserHasAnyPermissionAsync(string userId, params string[] permissions);
        Task<bool> UserHasAllPermissionsAsync(string userId, params string[] permissions);

        // User authentication helpers
        Task<bool> CheckPasswordAsync(string userId, string password);
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<bool> ResetPasswordAsync(string userId, string newPassword, string resetBy);
        Task<bool> LockUserAsync(string userId, DateTime? lockoutEnd, string lockedBy);
        Task<bool> UnlockUserAsync(string userId, string unlockedBy);

        // User profile management
        Task<bool> UpdateUserProfileAsync(string userId, string? firstName, string? lastName, string? department, string? jobTitle, string? profilePictureUrl);
        Task<bool> UpdateLastLoginAsync(string userId);

        // Search and filtering
        Task<IEnumerable<UserDto>> SearchUsersAsync(string searchTerm);
        Task<IEnumerable<UserDto>> GetUsersPaginatedAsync(int page, int pageSize, string? searchTerm = null, string? sortBy = null, bool sortDescending = false);
        Task<int> GetUsersCountAsync(string? searchTerm = null);
    }
}