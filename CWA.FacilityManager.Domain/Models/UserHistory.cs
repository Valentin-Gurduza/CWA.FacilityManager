using CWA.FacilityManager.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CWA.FacilityManager.Domain.Models
{
    public class UserHistory
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public UserHistoryType HistoryType { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Details { get; set; }

        public int? RelatedEntityId { get; set; }

        [StringLength(100)]
        public string? RelatedEntityType { get; set; }

        [StringLength(50)]
        public string? IpAddress { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ApplicationUser User { get; set; } = null!;
    }
}