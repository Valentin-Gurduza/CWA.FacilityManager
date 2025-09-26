using System.ComponentModel.DataAnnotations;

namespace CWA.FacilityManager.Domain.Models
{
    public class CalendarTask
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        
        public bool IsAllDay { get; set; }
        
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        
        public TaskStatus Status { get; set; } = TaskStatus.Pending;
        
        [MaxLength(50)]
        public string? Color { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Foreign key to ApplicationUser
        public string? AssignedUserId { get; set; }
        
        // Navigation property
        public ApplicationUser? AssignedUser { get; set; }
        
        // Foreign key for facility/location if applicable
        public int? FacilityId { get; set; }
        
        [MaxLength(200)]
        public string? Location { get; set; }
        
        // Recurrence properties
        public bool IsRecurring { get; set; }
        
        public RecurrenceType? RecurrenceType { get; set; }
        
        public int? RecurrenceInterval { get; set; }
        
        public DateTime? RecurrenceEndDate { get; set; }
        
        // Category - now using enum instead of string
        public TaskCategory Category { get; set; } = TaskCategory.Meeting;
        
        [MaxLength(500)]
        public string? Tags { get; set; }
    }
}