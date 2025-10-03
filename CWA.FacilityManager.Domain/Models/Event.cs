using System.ComponentModel.DataAnnotations;
using CWA.FacilityManager.Domain.Enums;

namespace CWA.FacilityManager.Domain.Models
{
    public enum EventType
    {
        Meeting,
        Course,
        Conference,
        Training,
        Workshop
    }

    public enum EventStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        Cancelled = 4
    }

    public class Event
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public EventType Type { get; set; }

        [Required]
        public DateTime StartDateTime { get; set; }

        [Required]
        public DateTime EndDateTime { get; set; }

        [StringLength(100)]
        public string? Organizer { get; set; }

        [StringLength(200)]
        public string? OrganizerCompany { get; set; }

        [StringLength(100)]
        public string? ContactName { get; set; }

        [StringLength(20)]
        public string? ContactPhone { get; set; }

        [StringLength(200)]
        public string? ContactEmail { get; set; }

        public int ExpectedAttendees { get; set; }

        public EventStatus Status { get; set; } = EventStatus.Pending;

        public bool IsConfirmed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Foreign keys
        public int RoomId { get; set; }
        public string? CreatedById { get; set; }
        public string? ApprovedById { get; set; }

        public DateTime? ApprovedAt { get; set; }

        // Navigation properties
        public virtual Room Room { get; set; } = null!;
        public virtual ApplicationUser? CreatedBy { get; set; }
        public virtual ApplicationUser? ApprovedBy { get; set; }
    }
}