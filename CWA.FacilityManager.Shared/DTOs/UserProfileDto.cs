using System.ComponentModel.DataAnnotations;

namespace CWA.FacilityManager.Shared.DTOs
{
    public class UserProfileDto
    {
        public string Id { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        [StringLength(200)]
        public string? Department { get; set; }

        [StringLength(200)]
        public string? Position { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string FullName { get; set; } = string.Empty;
        
        // Keep simple roles for backward compatibility
        public List<string> Roles { get; set; } = new List<string>();
        
        // Add detailed roles information
        public List<UserRoleDetailDto> DetailedRoles { get; set; } = new List<UserRoleDetailDto>();
    }

    public class UserRoleDetailDto
    {
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Priority { get; set; }
        public bool IsSystemRole { get; set; }
        public bool IsActive { get; set; }
        public string RoleType { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
        public string? AssignedBy { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? Notes { get; set; }
        public List<PermissionDetailDto> Permissions { get; set; } = new List<PermissionDetailDto>();
    }

    public class PermissionDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Module { get; set; } = string.Empty;
        public string Resource { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public bool IsSystemPermission { get; set; }
    }

    public class UpdateUserProfileDto
    {
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        [StringLength(200)]
        public string? Department { get; set; }

        [StringLength(200)]
        public string? Position { get; set; }
    }
}