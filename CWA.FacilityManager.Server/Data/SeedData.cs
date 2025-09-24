using CWA.FacilityManager.Infrastructure.Contexts;
using CWA.FacilityManager.Domain.Models;
using CWA.FacilityManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CWA.FacilityManager.Server.Data
{
    public static class SeedData
    {
        public static async Task Initialize(ApplicationDbContext context)
        {
            // Ensure the database is created
            await context.Database.EnsureCreatedAsync();

            // Check if data already exists
            if (context.Buildings.Any())
                return; // Database has been seeded

            // Add sample buildings
            var buildings = new[]
            {
                new Building
                {
                    Name = "Main Building",
                    Address = "123 Main Street",
                    Description = "Primary academic building",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Building
                {
                    Name = "Science Building",
                    Address = "456 Science Ave",
                    Description = "Science and technology building",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.Buildings.AddRange(buildings);
            await context.SaveChangesAsync();

            // Add sample rooms
            var mainBuilding = buildings[0];
            var scienceBuilding = buildings[1];

            var rooms = new[]
            {
                new Room
                {
                    Name = "Conference Room A",
                    RoomNumber = "101",
                    Capacity = 20,
                    Description = "Large conference room with projector",
                    Activity = ActivityType.Meeting,
                    Date = DateTime.Today.AddDays(1),
                    Time = TimeSpan.FromHours(9),
                    BuildingId = mainBuilding.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Room
                {
                    Name = "Computer Lab 1",
                    RoomNumber = "201",
                    Capacity = 30,
                    Description = "Computer lab with 30 workstations",
                    Activity = ActivityType.Course,
                    Date = DateTime.Today.AddDays(2),
                    Time = TimeSpan.FromHours(10),
                    BuildingId = scienceBuilding.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.Rooms.AddRange(rooms);
            await context.SaveChangesAsync();
        }
    }
}