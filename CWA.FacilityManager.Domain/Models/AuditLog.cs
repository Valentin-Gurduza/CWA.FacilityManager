using System.ComponentModel.DataAnnotations;

namespace CWA.FacilityManager.Domain.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        [Required]
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ActionType { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Entity { get; set; } = string.Empty;

        [StringLength(100)]
        public string? EntityId { get; set; }

        [StringLength(4000)]
        public string? Data { get; set; }

        [StringLength(50)]
        public string? IpAddress { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
