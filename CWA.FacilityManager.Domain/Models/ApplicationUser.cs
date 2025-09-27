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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<UserHistory> UserHistories { get; set; } = new List<UserHistory>();

        // Computed property with null handling
        public string FullName 
        { 
            get 
            {
                var firstName = string.IsNullOrEmpty(FirstName) ? "User" : FirstName;
                var lastName = string.IsNullOrEmpty(LastName) ? "" : LastName;
                return string.IsNullOrEmpty(lastName) ? firstName : $"{firstName} {lastName}";
            }
        }
    }
}
