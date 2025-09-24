using CWA.FacilityManager.Domain.Enums;

namespace CWA.FacilityManager.Application.DTOs.UserManagement
{
    public class RoleDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? NormalizedName { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystemRole { get; set; }
        public int Priority { get; set; }
        public RoleType RoleType { get; set; }
        public int UserCount { get; set; }
        public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
    }

    public class CreateRoleDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Priority { get; set; } = 0;
        public RoleType RoleType { get; set; } = RoleType.Custom;
        public List<int> PermissionIds { get; set; } = new List<int>();
    }

    public class UpdateRoleDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; }
        public RoleType RoleType { get; set; }
    }

    public class RolePermissionDto
    {
        public string RoleId { get; set; } = string.Empty;
        public int PermissionId { get; set; }
        public bool IsGranted { get; set; }
    }
}