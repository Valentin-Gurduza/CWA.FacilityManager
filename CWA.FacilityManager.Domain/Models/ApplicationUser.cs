using Microsoft.AspNetCore.Identity;

namespace CWA.FacilityManager.Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ProfilePictureUrl { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string? Department { get; set; }
        public string? JobTitle { get; set; }
        
        // Navigation properties
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        
        // Computed properties
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string DisplayName => string.IsNullOrEmpty(FullName) ? Email ?? UserName ?? "Unknown User" : FullName;
    }
}
