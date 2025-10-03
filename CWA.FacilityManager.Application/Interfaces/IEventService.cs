using CWA.FacilityManager.Domain.Models;

namespace CWA.FacilityManager.Application.Interfaces
{
    public interface IEventService
    {
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<Event?> GetEventByIdAsync(int id);
        Task<IEnumerable<Event>> GetEventsByRoomAsync(int roomId);
        Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Event>> GetEventsByStatusAsync(EventStatus status);
        Task<IEnumerable<Event>> GetPendingEventsAsync();
        Task<Event> CreateEventAsync(Event eventItem);
        Task<Event> UpdateEventAsync(Event eventItem);
        Task<bool> DeleteEventAsync(int id);
        Task<bool> ApproveEventAsync(int eventId, string approvedById);
        Task<bool> RejectEventAsync(int eventId, string approvedById);
        Task<bool> CancelEventAsync(int eventId);
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime startTime, DateTime endTime, int? excludeEventId = null);
        Task<IEnumerable<Event>> GetConflictingEventsAsync(int roomId, DateTime startTime, DateTime endTime, int? excludeEventId = null);
        Task<IEnumerable<Event>> GetUpcomingEventsAsync(int days = 7);
        Task<IEnumerable<Event>> GetUserEventsAsync(string userId);
    }
}