using CWA.FacilityManager.Domain.Enums;

namespace CWA.FacilityManager.Shared.DTOs
{
    public class UserHistoryDto
    {
        public int Id { get; set; }
        public UserHistoryType HistoryType { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Details { get; set; }
        public int? RelatedEntityId { get; set; }
        public string? RelatedEntityType { get; set; }
        public DateTime CreatedAt { get; set; }
        public string HistoryTypeDisplay => HistoryType.ToString().Replace("_", " ");
    }

    public class BookingHistoryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public BookingStatus Status { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public int? AttendeeCount { get; set; }
        public decimal? TotalCost { get; set; }
        public DateTime CreatedAt { get; set; }
        public string StatusDisplay => Status.ToString();
        public bool IsPastEvent => EndDate < DateTime.UtcNow;
        public bool IsUpcoming => StartDate > DateTime.UtcNow;
        public bool IsActive => Status == BookingStatus.Confirmed || Status == BookingStatus.InProgress;
    }
}