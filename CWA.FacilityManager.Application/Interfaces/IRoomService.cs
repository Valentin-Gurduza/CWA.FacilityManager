using CWA.FacilityManager.Domain.Models;

namespace CWA.FacilityManager.Application.Interfaces
{
    public interface IRoomService
    {
        Task<IEnumerable<Room>> GetAllRoomsAsync();
        Task<Room?> GetRoomByIdAsync(int id);
        Task<IEnumerable<Room>> GetRoomsByBuildingAsync(int buildingId);
        Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime startDate, DateTime endDate, int? capacity = null);
        Task<Room> CreateRoomAsync(Room room);
        Task<Room> UpdateRoomAsync(Room room);
        Task<bool> DeleteRoomAsync(int id);
        Task<IEnumerable<Room>> SearchRoomsAsync(string searchTerm);
    }
}