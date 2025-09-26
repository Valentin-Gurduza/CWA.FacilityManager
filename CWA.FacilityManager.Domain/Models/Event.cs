using System.ComponentModel.DataAnnotations;

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
        public string? ContactEmail { get; set; }

        public int ExpectedAttendees { get; set; }

        public bool IsConfirmed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public int RoomId { get; set; }
        public string? CreatedById { get; set; }

        // Navigation properties
        public virtual Room Room { get; set; } = null!;
        public virtual ApplicationUser? CreatedBy { get; set; }
    }
}