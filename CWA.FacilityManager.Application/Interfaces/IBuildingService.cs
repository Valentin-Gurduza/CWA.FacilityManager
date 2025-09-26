using CWA.FacilityManager.Domain.Models;

namespace CWA.FacilityManager.Application.Interfaces
{
    public interface IBuildingService
    {
        Task<IEnumerable<Building>> GetAllBuildingsAsync();
        Task<Building?> GetBuildingByIdAsync(int id);
        Task<Building> CreateBuildingAsync(Building building);
        Task<Building> UpdateBuildingAsync(Building building);
        Task<bool> DeleteBuildingAsync(int id);
        Task<bool> DeleteBuildingPermanentlyAsync(int id);
    }
}