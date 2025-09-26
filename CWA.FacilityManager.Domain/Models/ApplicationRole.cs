using Microsoft.AspNetCore.Identity;
using CWA.FacilityManager.Domain.Enums;

namespace CWA.FacilityManager.Domain.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsSystemRole { get; set; } = false; // System roles cannot be deleted
        public int Priority { get; set; } = 0; // Higher number = higher priority
        public RoleType RoleType { get; set; } = RoleType.Custom; // Add role type
        
        // Navigation properties
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}