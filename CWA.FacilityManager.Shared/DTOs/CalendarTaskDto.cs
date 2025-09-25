using System.ComponentModel.DataAnnotations;

namespace CWA.FacilityManager.Shared.DTOs
{
    public class CalendarTaskDto
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        
        public bool IsAllDay { get; set; }
        
        public TaskPriorityDto Priority { get; set; } = TaskPriorityDto.Medium;
        
        public TaskStatusDto Status { get; set; } = TaskStatusDto.Pending;
        
        [StringLength(50)]
        public string? Color { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public string? AssignedUserId { get; set; }
        
        public string? AssignedUserName { get; set; }
        
        public int? FacilityId { get; set; }
        
        [StringLength(200)]
        public string? Location { get; set; }
        
        public bool IsRecurring { get; set; }
        
        public RecurrenceTypeDto? RecurrenceType { get; set; }
        
        public int? RecurrenceInterval { get; set; }
        
        public DateTime? RecurrenceEndDate { get; set; }
        
        public TaskCategoryDto Category { get; set; } = TaskCategoryDto.Meeting;
        
        [StringLength(500)]
        public string? Tags { get; set; }
    }
    
    public class CreateCalendarTaskDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        
        public bool IsAllDay { get; set; }
        
        public TaskPriorityDto Priority { get; set; } = TaskPriorityDto.Medium;
        
        [StringLength(50)]
        public string? Color { get; set; }
        
        public string? AssignedUserId { get; set; }
        
        public int? FacilityId { get; set; }
        
        [StringLength(200)]
        public string? Location { get; set; }
        
        public bool IsRecurring { get; set; }
        
        public RecurrenceTypeDto? RecurrenceType { get; set; }
        
        public int? RecurrenceInterval { get; set; }
        
        public DateTime? RecurrenceEndDate { get; set; }
        
        public TaskCategoryDto Category { get; set; } = TaskCategoryDto.Meeting;
        
        [StringLength(500)]
        public string? Tags { get; set; }
    }
    
    public class UpdateCalendarTaskDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        
        public bool IsAllDay { get; set; }
        
        public TaskPriorityDto Priority { get; set; } = TaskPriorityDto.Medium;
        
        public TaskStatusDto Status { get; set; } = TaskStatusDto.Pending;
        
        [StringLength(50)]
        public string? Color { get; set; }
        
        public string? AssignedUserId { get; set; }
        
        public int? FacilityId { get; set; }
        
        [StringLength(200)]
        public string? Location { get; set; }
        
        public bool IsRecurring { get; set; }
        
        public RecurrenceTypeDto? RecurrenceType { get; set; }
        
        public int? RecurrenceInterval { get; set; }
        
        public DateTime? RecurrenceEndDate { get; set; }
        
        public TaskCategoryDto Category { get; set; } = TaskCategoryDto.Meeting;
        
        [StringLength(500)]
        public string? Tags { get; set; }
    }
}