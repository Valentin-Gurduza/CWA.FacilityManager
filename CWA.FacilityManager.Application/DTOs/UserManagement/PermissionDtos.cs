namespace CWA.FacilityManager.Application.DTOs.UserManagement
{
    public class PermissionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Module { get; set; } = string.Empty;
        public string Resource { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public bool IsSystemPermission { get; set; }
        public string PolicyName { get; set; } = string.Empty;
        public bool IsGranted { get; set; } // For role-permission assignments
    }

    public class PermissionGroupDto
    {
        public string Module { get; set; } = string.Empty;
        public string ModuleDisplayName { get; set; } = string.Empty;
        public List<PermissionResourceDto> Resources { get; set; } = new List<PermissionResourceDto>();
    }

    public class PermissionResourceDto
    {
        public string Resource { get; set; } = string.Empty;
        public string ResourceDisplayName { get; set; } = string.Empty;
        public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
    }

    public class CreatePermissionDto
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Module { get; set; } = string.Empty;
        public string Resource { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
    }
}