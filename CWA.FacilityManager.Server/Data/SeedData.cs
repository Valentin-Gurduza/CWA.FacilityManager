using CWA.FacilityManager.Infrastructure.Contexts;
using CWA.FacilityManager.Domain.Models;
using CWA.FacilityManager.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CWA.FacilityManager.Server.Data
{
    public static class SeedData
    {
        // Default administrator credentials
        public const string DefaultAdminEmail = "admin@facilitymanager.local";
        public const string DefaultAdminPassword = "Admin@123";
        public const string DefaultAdminUsername = "admin";

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

        /// <summary>
        /// Seeds the default administrator user if no users exist
        /// </summary>
        public static async Task SeedDefaultAdminUser(
            UserManager<ApplicationUser> userManager, 
            RoleManager<ApplicationRole> roleManager,
            ILogger logger)
        {
            try
            {
                // Check if any users exist
                if (await userManager.Users.AnyAsync())
                {
                    logger.LogInformation("Users already exist. Skipping default admin user creation.");
                    return;
                }

                // Ensure Administrator role exists
                var adminRole = await roleManager.FindByNameAsync("Administrator");
                if (adminRole == null)
                {
                    logger.LogWarning("Administrator role not found. Cannot create default admin user.");
                    return;
                }

                // Create default admin user
                var adminUser = new ApplicationUser
                {
                    UserName = DefaultAdminUsername,
                    Email = DefaultAdminEmail,
                    EmailConfirmed = true,
                    FirstName = "System",
                    LastName = "Administrator",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    PhoneNumberConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, DefaultAdminPassword);
                
                if (result.Succeeded)
                {
                    // Assign Administrator role
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                    
                    logger.LogInformation("Default administrator user created successfully.");
                    logger.LogInformation("Email: {Email}, Password: {Password}", DefaultAdminEmail, DefaultAdminPassword);
                    logger.LogWarning("IMPORTANT: Change the default administrator password after first login!");
                }
                else
                {
                    logger.LogError("Failed to create default admin user: {Errors}", 
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating default administrator user");
            }
        }
    }
}