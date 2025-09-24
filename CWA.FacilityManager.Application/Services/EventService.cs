using CWA.FacilityManager.Application.Interfaces;
using CWA.FacilityManager.Domain.Models;
using CWA.FacilityManager.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CWA.FacilityManager.Application.Services
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;

        public EventService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _context.Events
                .Include(e => e.Room)
                    .ThenInclude(r => r.Building)
                .Include(e => e.CreatedBy)
                .OrderBy(e => e.StartDateTime)
                .ToListAsync();
        }

        public async Task<Event?> GetEventByIdAsync(int id)
        {
            return await _context.Events
                .Include(e => e.Room)
                    .ThenInclude(r => r.Building)
                .Include(e => e.CreatedBy)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Event>> GetEventsByRoomAsync(int roomId)
        {
            return await _context.Events
                .Include(e => e.Room)
                    .ThenInclude(r => r.Building)
                .Include(e => e.CreatedBy)
                .Where(e => e.RoomId == roomId)
                .OrderBy(e => e.StartDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Events
                .Include(e => e.Room)
                    .ThenInclude(r => r.Building)
                .Include(e => e.CreatedBy)
                .Where(e => e.StartDateTime >= startDate && e.StartDateTime <= endDate)
                .OrderBy(e => e.StartDateTime)
                .ToListAsync();
        }

        public async Task<Event> CreateEventAsync(Event eventItem)
        {
            eventItem.CreatedAt = DateTime.UtcNow;
            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();
            return eventItem;
        }

        public async Task<Event> UpdateEventAsync(Event eventItem)
        {
            _context.Events.Update(eventItem);
            await _context.SaveChangesAsync();
            return eventItem;
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null) return false;

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime startTime, DateTime endTime, int? excludeEventId = null)
        {
            var conflictingEvents = await _context.Events
                .Where(e => e.RoomId == roomId &&
                    e.Id != excludeEventId &&
                    ((e.StartDateTime < endTime && e.EndDateTime > startTime)))
                .AnyAsync();

            return !conflictingEvents;
        }

        public async Task<IEnumerable<Event>> GetUpcomingEventsAsync(int days = 7)
        {
            var endDate = DateTime.UtcNow.AddDays(days);
            return await _context.Events
                .Include(e => e.Room)
                    .ThenInclude(r => r.Building)
                .Include(e => e.CreatedBy)
                .Where(e => e.StartDateTime >= DateTime.UtcNow && e.StartDateTime <= endDate)
                .OrderBy(e => e.StartDateTime)
                .ToListAsync();
        }
    }
}