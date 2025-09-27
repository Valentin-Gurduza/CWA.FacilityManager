using System.ComponentModel.DataAnnotations;
using CWA.FacilityManager.Domain.Enums;

namespace CWA.FacilityManager.Domain.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Room name is required")]
        [StringLength(100, ErrorMessage = "Room name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Room number cannot exceed 20 characters")]
        public string? RoomNumber { get; set; }

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 1000, ErrorMessage = "Capacity must be between 1 and 1000")]
        public int Capacity { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [StringLength(1000)]
        public string? Equipment { get; set; }

        [Required(ErrorMessage = "Activity type is required")]
        public ActivityType Activity { get; set; } = ActivityType.Course;

        public bool IsAvailable { get; set; } = true;
        public bool IsActive { get; set; } = true;

        public decimal? HourlyRate { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Time is required")]
        public TimeSpan Time { get; set; } = TimeSpan.FromHours(9); // Default to 9:00 AM

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key
        [Required(ErrorMessage = "Please select a building")]
        public int BuildingId { get; set; }

        // Navigation properties
        public virtual Building Building { get; set; } = null!;
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    }
}