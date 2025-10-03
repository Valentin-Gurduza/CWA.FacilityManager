using CWA.FacilityManager.Application.Interfaces;
using CWA.FacilityManager.Domain.Models;
using CWA.FacilityManager.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CWA.FacilityManager.Application.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(ApplicationDbContext context, ILogger<AuditLogService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LogAsync(string userId, string actionType, string entity, string? entityId, string? data, string? ipAddress = null, string? userAgent = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    UserId = userId,
                    ActionType = actionType,
                    Entity = entity,
                    EntityId = entityId,
                    Data = data,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    Timestamp = DateTime.UtcNow
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Audit log created: User {UserId} performed {ActionType} on {Entity} {EntityId}",
                    userId, actionType, entity, entityId ?? "N/A");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating audit log for User {UserId}, Action {ActionType}, Entity {Entity}",
                    userId, actionType, entity);
                // Don't throw - audit logging should not break the application
            }
        }

        public async Task<IEnumerable<AuditLog>> GetUserAuditLogsAsync(string userId, int pageNumber = 1, int pageSize = 50)
        {
            return await _context.AuditLogs
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(a => a.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetEntityAuditLogsAsync(string entity, string entityId, int pageNumber = 1, int pageSize = 50)
        {
            return await _context.AuditLogs
                .Where(a => a.Entity == entity && a.EntityId == entityId)
                .OrderByDescending(a => a.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(a => a.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsByActionTypeAsync(string actionType, int pageNumber = 1, int pageSize = 50)
        {
            return await _context.AuditLogs
                .Where(a => a.ActionType == actionType)
                .OrderByDescending(a => a.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(a => a.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetRecentAuditLogsAsync(int pageNumber = 1, int pageSize = 50)
        {
            return await _context.AuditLogs
                .OrderByDescending(a => a.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(a => a.User)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
