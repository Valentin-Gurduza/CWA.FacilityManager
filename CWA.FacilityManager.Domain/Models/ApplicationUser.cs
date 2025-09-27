using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CWA.FacilityManager.Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Bio { get; set; }

        [StringLength(200)]
        public string? Department { get; set; }

        [StringLength(200)]
        public string? Position { get; set; }

        [StringLength(200)]
        public string? JobTitle { get; set; }

        public string? ProfilePictureUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<UserHistory> UserHistories { get; set; } = new List<UserHistory>();
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        // Computed properties with proper null handling
        public string FullName 
        { 
            get 
            {
                var firstName = string.IsNullOrEmpty(FirstName) ? "User" : FirstName;
                var lastName = string.IsNullOrEmpty(LastName) ? "" : LastName;
                return string.IsNullOrEmpty(lastName) ? firstName : $"{firstName} {lastName}";
            }
        }

        public string DisplayName => string.IsNullOrEmpty(FullName) ? Email ?? UserName ?? "Unknown User" : FullName;
    }
}