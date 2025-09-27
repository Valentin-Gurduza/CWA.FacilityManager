using CWA.FacilityManager.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CWA.FacilityManager.Domain.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int RoomId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public int? AttendeeCount { get; set; }

        [StringLength(1000)]
        public string? SpecialRequirements { get; set; }

        public decimal? TotalCost { get; set; }

        [StringLength(500)]
        public string? CancellationReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual Room Room { get; set; } = null!;

        // Computed properties
        public TimeSpan Duration => EndDate - StartDate;
        public bool IsActive => Status == BookingStatus.Confirmed || Status == BookingStatus.InProgress;
        public bool IsPastEvent => EndDate < DateTime.UtcNow;
    }
}