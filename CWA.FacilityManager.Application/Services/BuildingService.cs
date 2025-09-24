using CWA.FacilityManager.Application.Interfaces;
using CWA.FacilityManager.Domain.Models;
using CWA.FacilityManager.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CWA.FacilityManager.Application.Services
{
    public class BuildingService : IBuildingService
    {
        private readonly ApplicationDbContext _context;

        public BuildingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Building>> GetAllBuildingsAsync()
        {
            return await _context.Buildings
                .Include(b => b.Rooms.Where(r => r.IsActive))
                .Where(b => b.IsActive)
                .OrderBy(b => b.Name)
                .ToListAsync();
        }

        public async Task<Building?> GetBuildingByIdAsync(int id)
        {
            return await _context.Buildings
                .Include(b => b.Rooms.Where(r => r.IsActive))
                .FirstOrDefaultAsync(b => b.Id == id && b.IsActive);
        }

        public async Task<Building> CreateBuildingAsync(Building building)
        {
            building.CreatedAt = DateTime.UtcNow;
            _context.Buildings.Add(building);
            await _context.SaveChangesAsync();
            return building;
        }

        public async Task<Building> UpdateBuildingAsync(Building building)
        {
            _context.Buildings.Update(building);
            await _context.SaveChangesAsync();
            return building;
        }

        public async Task<bool> DeleteBuildingAsync(int id)
        {
            var building = await _context.Buildings.FindAsync(id);
            if (building == null) return false;

            building.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBuildingPermanentlyAsync(int id)
        {
            var building = await _context.Buildings
                .Include(b => b.Rooms)
                .FirstOrDefaultAsync(b => b.Id == id);
            
            if (building == null) return false;

            // Check if building has any active rooms
            if (building.Rooms.Any(r => r.IsActive))
            {
                throw new InvalidOperationException("Cannot delete building that contains active rooms.");
            }

            // Permanently delete the building from database
            _context.Buildings.Remove(building);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}