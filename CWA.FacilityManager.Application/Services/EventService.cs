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
                .AsNoTracking()
                .Include(e => e.Room)
                    .ThenInclude(r => r.Building)
                .Include(e => e.CreatedBy)
                .Include(e => e.ApprovedBy)
                .OrderBy(e => e.StartDateTime)
                .ToListAsync();
        }

        public async Task<Event?> GetEventByIdAsync(int id)
        {
            return await _context.Events
                .AsNoTracking()
                .Include(e => e.Room)
                    .ThenInclude(r => r.Building)
                .Include(e => e.CreatedBy)
                .Include(e => e.ApprovedBy)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Event>> GetEventsByRoomAsync(int roomId)
        {
            return await _context.Events
                .AsNoTracking()
                .Include(e => e.Room)
                    .ThenInclude(r => r.Building)
                .Include(e => e.CreatedBy)
                .Include(e => e.ApprovedBy)
                .Where(e => e.RoomId == roomId)
                .OrderBy(e => e.StartDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Events
                .AsNoTracking()
                .Include(e => e.Room)
                    .ThenInclude(r => r.Building)
                .Include(e => e.CreatedBy)
                .Include(e => e.ApprovedBy)
                .Where(e => e.StartDateTime >= startDate && e.StartDateTime <= endDate)
                .OrderBy(e => e.StartDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByStatusAsync(EventStatus status)
        {
            return await _context.Events
                .AsNoTracking()
                .Include(e => e.Room)
                    .ThenInclude(r => r.Building)
                .Include(e => e.CreatedBy)
                .Include(e => e.ApprovedBy)
                .Where(e => e.Status == status)
                .OrderBy(e => e.StartDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetPendingEventsAsync()
        {
            return await GetEventsByStatusAsync(EventStatus.Pending);
        }

        public async Task<IEnumerable<Event>> GetUserEventsAsync(string userId)
        {
            return await _context.Events
                .AsNoTracking()
                .Include(e => e.Room)
                    .ThenInclude(r => r.Building)
                .Include(e => e.CreatedBy)
                .Include(e => e.ApprovedBy)
                .Where(e => e.CreatedById == userId)
                .OrderBy(e => e.StartDateTime)
                .ToListAsync();
        }

        public async Task<Event> CreateEventAsync(Event eventItem)
        {
            eventItem.CreatedAt = DateTime.UtcNow;
            eventItem.Status = EventStatus.Pending;
            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();
            return eventItem;
        }

        public async Task<Event> UpdateEventAsync(Event eventItem)
        {
            var existingEvent = await _context.Events.FindAsync(eventItem.Id);
            if (existingEvent == null)
            {
                throw new InvalidOperationException($"Event with ID {eventItem.Id} not found.");
            }

            // Update properties
            existingEvent.Title = eventItem.Title;
            existingEvent.Description = eventItem.Description;
            existingEvent.Type = eventItem.Type;
            existingEvent.StartDateTime = eventItem.StartDateTime;
            existingEvent.EndDateTime = eventItem.EndDateTime;
            existingEvent.Organizer = eventItem.Organizer;
            existingEvent.OrganizerCompany = eventItem.OrganizerCompany;
            existingEvent.ContactName = eventItem.ContactName;
            existingEvent.ContactPhone = eventItem.ContactPhone;
            existingEvent.ContactEmail = eventItem.ContactEmail;
            existingEvent.ExpectedAttendees = eventItem.ExpectedAttendees;
            existingEvent.IsConfirmed = eventItem.IsConfirmed;
            existingEvent.RoomId = eventItem.RoomId;
            existingEvent.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingEvent;
        }

        public async Task<bool> ApproveEventAsync(int eventId, string approvedById)
        {
            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null) return false;

            eventItem.Status = EventStatus.Approved;
            eventItem.ApprovedById = approvedById;
            eventItem.ApprovedAt = DateTime.UtcNow;
            eventItem.IsConfirmed = true;
            eventItem.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectEventAsync(int eventId, string approvedById)
        {
            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null) return false;

            eventItem.Status = EventStatus.Rejected;
            eventItem.ApprovedById = approvedById;
            eventItem.ApprovedAt = DateTime.UtcNow;
            eventItem.IsConfirmed = false;
            eventItem.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelEventAsync(int eventId)
        {
            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null) return false;

            eventItem.Status = EventStatus.Cancelled;
            eventItem.IsConfirmed = false;
            eventItem.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
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
                    e.Status != EventStatus.Rejected &&
                    e.Status != EventStatus.Cancelled &&
                    ((e.StartDateTime < endTime && e.EndDateTime > startTime)))
                .AnyAsync();

            return !conflictingEvents;
        }

        public async Task<IEnumerable<Event>> GetConflictingEventsAsync(int roomId, DateTime startTime, DateTime endTime, int? excludeEventId = null)
        {
            return await _context.Events
                .AsNoTracking()
                .Include(e => e.Room)
                    .ThenInclude(r => r.Building)
                .Include(e => e.CreatedBy)
                .Include(e => e.ApprovedBy)
                .Where(e => e.RoomId == roomId &&
                    e.Id != excludeEventId &&
                    e.Status != EventStatus.Rejected &&
                    e.Status != EventStatus.Cancelled &&
                    ((e.StartDateTime < endTime && e.EndDateTime > startTime)))
                .OrderBy(e => e.StartDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetUpcomingEventsAsync(int days = 7)
        {
            var endDate = DateTime.UtcNow.AddDays(days);
            return await _context.Events
                .AsNoTracking()
                .Include(e => e.Room)
                    .ThenInclude(r => r.Building)
                .Include(e => e.CreatedBy)
                .Include(e => e.ApprovedBy)
                .Where(e => e.StartDateTime >= DateTime.UtcNow && 
                    e.StartDateTime <= endDate &&
                    e.Status == EventStatus.Approved)
                .OrderBy(e => e.StartDateTime)
                .ToListAsync();
        }
    }
}