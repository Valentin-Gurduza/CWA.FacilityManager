using CWA.FacilityManager.Domain.Models;

namespace CWA.FacilityManager.Application.Interfaces
{
    public interface IAuditLogService
    {
        Task LogAsync(string userId, string actionType, string entity, string? entityId, string? data, string? ipAddress = null, string? userAgent = null);
        Task<IEnumerable<AuditLog>> GetUserAuditLogsAsync(string userId, int pageNumber = 1, int pageSize = 50);
        Task<IEnumerable<AuditLog>> GetEntityAuditLogsAsync(string entity, string entityId, int pageNumber = 1, int pageSize = 50);
        Task<IEnumerable<AuditLog>> GetAuditLogsByActionTypeAsync(string actionType, int pageNumber = 1, int pageSize = 50);
        Task<IEnumerable<AuditLog>> GetRecentAuditLogsAsync(int pageNumber = 1, int pageSize = 50);
    }
}
