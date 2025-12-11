using CWA.FacilityManager.Application.Interfaces;
using CWA.FacilityManager.Domain.Models;
using CWA.FacilityManager.Domain.Enums;
using CWA.FacilityManager.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CWA.FacilityManager.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserProfileService _userProfileService;

        public BookingService(ApplicationDbContext context, IUserProfileService userProfileService)
        {
            _context = context;
            _userProfileService = userProfileService;
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings
                .AsNoTracking()
                .Include(b => b.User)
                .Include(b => b.Room)
                .ThenInclude(r => r.Building)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Include(b => b.User)
                .Include(b => b.Room)
                .ThenInclude(r => r.Building)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserAsync(string userId)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Include(b => b.Room)
                .ThenInclude(r => r.Building)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByRoomAsync(int roomId)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Include(b => b.User)
                .Where(b => b.RoomId == roomId)
                .OrderByDescending(b => b.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByRoomAndDateRangeAsync(int roomId, DateTime startDate, DateTime endDate)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Include(b => b.User)
                .Where(b => b.RoomId == roomId && 
                           b.StartDate < endDate && 
                           b.EndDate > startDate &&
                           b.Status != BookingStatus.Cancelled)
                .OrderBy(b => b.StartDate)
                .ToListAsync();
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime startDate, DateTime endDate, int? excludeBookingId = null)
        {
            var conflictingBookings = await _context.Bookings
                .AsNoTracking()
                .Where(b => b.RoomId == roomId &&
                           b.StartDate < endDate &&
                           b.EndDate > startDate &&
                           b.Status != BookingStatus.Cancelled &&
                           (excludeBookingId == null || b.Id != excludeBookingId))
                .AnyAsync();

            return !conflictingBookings;
        }

        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            // Validate room availability
            var isAvailable = await IsRoomAvailableAsync(booking.RoomId, booking.StartDate, booking.EndDate);
            if (!isAvailable)
            {
                throw new InvalidOperationException("The room is not available for the selected time period.");
            }

            // Calculate total cost if room has hourly rate
            booking.TotalCost = await CalculateTotalCostAsync(booking.RoomId, booking.StartDate, booking.EndDate);

            // Set timestamps
            booking.CreatedAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;

            // Set initial status
            if (booking.Status == 0)
            {
                booking.Status = BookingStatus.Pending;
            }

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Add user history
            await _userProfileService.AddUserHistoryAsync(
                booking.UserId,
                UserHistoryType.BookingCreated,
                $"Created booking for room {booking.Room?.Name ?? booking.RoomId.ToString()}",
                $"Booking ID: {booking.Id}, Date: {booking.StartDate:yyyy-MM-dd HH:mm} - {booking.EndDate:yyyy-MM-dd HH:mm}",
                booking.Id,
                nameof(Booking)
            );

            return booking;
        }

        public async Task<Booking> UpdateBookingAsync(Booking booking)
        {
            var existingBooking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == booking.Id);

            if (existingBooking == null)
            {
                throw new InvalidOperationException($"Booking with ID {booking.Id} not found.");
            }

            // Check if dates are being changed and validate availability
            if (existingBooking.StartDate != booking.StartDate || 
                existingBooking.EndDate != booking.EndDate || 
                existingBooking.RoomId != booking.RoomId)
            {
                var isAvailable = await IsRoomAvailableAsync(booking.RoomId, booking.StartDate, booking.EndDate, booking.Id);
                if (!isAvailable)
                {
                    throw new InvalidOperationException("The room is not available for the selected time period.");
                }
            }

            // Update properties
            existingBooking.Title = booking.Title;
            existingBooking.Description = booking.Description;
            existingBooking.StartDate = booking.StartDate;
            existingBooking.EndDate = booking.EndDate;
            existingBooking.AttendeeCount = booking.AttendeeCount;
            existingBooking.SpecialRequirements = booking.SpecialRequirements;
            existingBooking.UpdatedAt = DateTime.UtcNow;

            // Recalculate cost if dates changed
            if (existingBooking.StartDate != booking.StartDate || existingBooking.EndDate != booking.EndDate)
            {
                existingBooking.TotalCost = await CalculateTotalCostAsync(booking.RoomId, booking.StartDate, booking.EndDate);
            }

            await _context.SaveChangesAsync();

            // Add user history
            await _userProfileService.AddUserHistoryAsync(
                existingBooking.UserId,
                UserHistoryType.BookingModified,
                $"Modified booking for room {existingBooking.Room?.Name ?? existingBooking.RoomId.ToString()}",
                $"Booking ID: {existingBooking.Id}, Updated: {DateTime.UtcNow:yyyy-MM-dd HH:mm}",
                existingBooking.Id,
                nameof(Booking)
            );

            return existingBooking;
        }

        public async Task<bool> CancelBookingAsync(int id, string cancellationReason, string userId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return false;

            booking.Status = BookingStatus.Cancelled;
            booking.CancellationReason = cancellationReason;
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Add user history
            await _userProfileService.AddUserHistoryAsync(
                userId,
                UserHistoryType.BookingCancelled,
                $"Cancelled booking for room {booking.Room?.Name ?? booking.RoomId.ToString()}",
                $"Booking ID: {id}, Reason: {cancellationReason}",
                id,
                nameof(Booking)
            );

            return true;
        }

        public async Task<bool> ConfirmBookingAsync(int id, string userId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return false;

            booking.Status = BookingStatus.Confirmed;
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Add user history
            await _userProfileService.AddUserHistoryAsync(
                userId,
                UserHistoryType.BookingModified,
                $"Confirmed booking for room {booking.Room?.Name ?? booking.RoomId.ToString()}",
                $"Booking ID: {id}, Confirmed at: {DateTime.UtcNow:yyyy-MM-dd HH:mm}",
                id,
                nameof(Booking)
            );

            return true;
        }

        public async Task<bool> CompleteBookingAsync(int id, string userId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return false;

            booking.Status = BookingStatus.Completed;
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Add user history
            await _userProfileService.AddUserHistoryAsync(
                userId,
                UserHistoryType.BookingCompleted,
                $"Completed booking for room {booking.Room?.Name ?? booking.RoomId.ToString()}",
                $"Booking ID: {id}, Completed at: {DateTime.UtcNow:yyyy-MM-dd HH:mm}",
                id,
                nameof(Booking)
            );

            return true;
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return false;

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Booking>> SearchBookingsAsync(string searchTerm)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Include(b => b.User)
                .Include(b => b.Room)
                .ThenInclude(r => r.Building)
                .Where(b => b.Title.Contains(searchTerm) ||
                           b.Description!.Contains(searchTerm) ||
                           b.User.FirstName.Contains(searchTerm) ||
                           b.User.LastName.Contains(searchTerm) ||
                           b.User.Email!.Contains(searchTerm) ||
                           b.Room.Name.Contains(searchTerm))
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<decimal?> CalculateTotalCostAsync(int roomId, DateTime startDate, DateTime endDate)
        {
            var room = await _context.Rooms
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room?.HourlyRate == null) return null;

            var duration = endDate - startDate;
            var totalHours = (decimal)duration.TotalHours;

            return totalHours * room.HourlyRate;
        }
    }
}