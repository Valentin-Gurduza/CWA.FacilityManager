using CWA.FacilityManager.Domain.Models;
using CWA.FacilityManager.Domain.Enums;

namespace CWA.FacilityManager.Application.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<IEnumerable<Booking>> GetBookingsByUserAsync(string userId);
        Task<IEnumerable<Booking>> GetBookingsByRoomAsync(int roomId);
        Task<IEnumerable<Booking>> GetBookingsByRoomAndDateRangeAsync(int roomId, DateTime startDate, DateTime endDate);
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime startDate, DateTime endDate, int? excludeBookingId = null);
        Task<Booking> CreateBookingAsync(Booking booking);
        Task<Booking> UpdateBookingAsync(Booking booking);
        Task<bool> CancelBookingAsync(int id, string cancellationReason, string userId);
        Task<bool> ConfirmBookingAsync(int id, string userId);
        Task<bool> CompleteBookingAsync(int id, string userId);
        Task<bool> DeleteBookingAsync(int id);
        Task<IEnumerable<Booking>> SearchBookingsAsync(string searchTerm);
        Task<decimal?> CalculateTotalCostAsync(int roomId, DateTime startDate, DateTime endDate);
    }
}