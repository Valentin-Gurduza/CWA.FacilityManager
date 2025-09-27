using CWA.FacilityManager.Application.Interfaces;
using CWA.FacilityManager.Domain.Models;
using CWA.FacilityManager.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CWA.FacilityManager.Application.Services
{
    public class RoomService : IRoomService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBuildingService _buildingService;

        public RoomService(ApplicationDbContext context, IBuildingService buildingService)
        {
            _context = context;
            _buildingService = buildingService;
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            return await _context.Rooms
                .AsNoTracking() // Prevent tracking for read-only operations
                .Include(r => r.Building)
                .Include(r => r.Events.Where(e => e.StartDateTime >= DateTime.UtcNow))
                .Where(r => r.IsActive)
                .OrderBy(r => r.Building.Name)
                .ThenBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<Room?> GetRoomByIdAsync(int id)
        {
            return await _context.Rooms
                .AsNoTracking() // Prevent tracking for read-only operations
                .Include(r => r.Building)
                .Include(r => r.Events)
                .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);
        }

        public async Task<IEnumerable<Room>> GetRoomsByBuildingAsync(int buildingId)
        {
            return await _context.Rooms
                .AsNoTracking() // Prevent tracking for read-only operations
                .Include(r => r.Building)
                .Include(r => r.Events.Where(e => e.StartDateTime >= DateTime.UtcNow))
                .Where(r => r.BuildingId == buildingId && r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<Room> CreateRoomAsync(Room room)
        {
            room.CreatedAt = DateTime.UtcNow;
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<Room> UpdateRoomAsync(Room room)
        {
            // Get the existing entity from the database
            var existingRoom = await _context.Rooms.FindAsync(room.Id);
            if (existingRoom == null)
            {
                throw new InvalidOperationException($"Room with ID {room.Id} not found.");
            }

            // Update only the properties that can be changed
            existingRoom.Name = room.Name;
            existingRoom.RoomNumber = room.RoomNumber;
            existingRoom.Capacity = room.Capacity;
            existingRoom.Description = room.Description;
            existingRoom.Activity = room.Activity;
            existingRoom.Date = room.Date;
            existingRoom.Time = room.Time;
            existingRoom.BuildingId = room.BuildingId;
            existingRoom.IsActive = room.IsActive;
            // Don't update CreatedAt - keep the original value

            await _context.SaveChangesAsync();
            return existingRoom;
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Events) // Include events to handle cascade deletion
                .Include(r => r.Building) // Include building to check if it becomes empty
                .FirstOrDefaultAsync(r => r.Id == id);
            
            if (room == null) return false;

            // Check if room has active events
            var hasActiveEvents = room.Events.Any(e => e.EndDateTime > DateTime.UtcNow);
            if (hasActiveEvents)
            {
                throw new InvalidOperationException("Cannot delete room with active or future events. Please cancel or move the events first.");
            }

            // Store building ID before deleting the room
            var buildingId = room.BuildingId;

            // Physically delete the room from the database
            // This will also delete associated events due to cascade delete configuration
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            // Check if the building has any remaining active rooms
            var remainingRooms = await _context.Rooms
                .Where(r => r.BuildingId == buildingId && r.IsActive)
                .CountAsync();

            // If no active rooms remain in the building, delete the building permanently
            if (remainingRooms == 0)
            {
                try
                {
                    await _buildingService.DeleteBuildingPermanentlyAsync(buildingId);
                }
                catch (Exception)
                {
                    // If building deletion fails, it's not critical for room deletion
                    // The building will remain but can be cleaned up later
                }
            }

            return true;
        }

        public async Task<IEnumerable<Room>> SearchRoomsAsync(string searchTerm)
        {
            return await _context.Rooms
                .AsNoTracking() // Prevent tracking for read-only operations
                .Include(r => r.Building)
                .Include(r => r.Events.Where(e => e.StartDateTime >= DateTime.UtcNow))
                .Where(r => r.IsActive && 
                    (r.Name.Contains(searchTerm) || 
                     r.RoomNumber!.Contains(searchTerm) ||
                     r.Building.Name.Contains(searchTerm)))
                .OrderBy(r => r.Building.Name)
                .ThenBy(r => r.Name)
                .ToListAsync();
        }
    }
}