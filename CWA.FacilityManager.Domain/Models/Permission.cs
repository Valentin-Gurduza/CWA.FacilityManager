namespace CWA.FacilityManager.Domain.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Module { get; set; } = string.Empty; // e.g., "UserManagement", "FacilityManagement"
        public string Resource { get; set; } = string.Empty; // e.g., "Users", "Roles", "Facilities"
        public string Action { get; set; } = string.Empty; // e.g., "View", "Create", "Edit", "Delete"
        public bool IsSystemPermission { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        
        // Computed property for policy name
        public string PolicyName => $"{Module}.{Resource}.{Action}";
    }
}