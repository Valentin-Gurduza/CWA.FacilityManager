using CWA.FacilityManager.Application.Services;
using CWA.FacilityManager.Domain.Models;
using CWA.FacilityManager.Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CWA.FacilityManager.Server.Extensions
{
    public static class DatabaseExtensions
    {
        public static async Task<IApplicationBuilder> SeedDatabaseAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var extensionLogger = loggerFactory.CreateLogger("DatabaseExtensions");
                var roleInitLogger = loggerFactory.CreateLogger<RoleInitializationService>();

                // Ensure database is created and migrations are applied
                await context.Database.MigrateAsync();

                // Initialize roles
                var roleInitService = new RoleInitializationService(roleManager, roleInitLogger);
                await roleInitService.InitializeRolesAsync();

                // Seed sample rooms if none exist
                if (!await context.Rooms.AnyAsync())
                {
                    await SeedRoomsAsync(context, extensionLogger);
                }

                extensionLogger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("DatabaseExtensions");
                logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }

            return app;
        }

        private static async Task SeedRoomsAsync(ApplicationDbContext context, ILogger logger)
        {
            var rooms = new List<Room>
            {
                new Room
                {
                    Name = "Conference Room A",
                    Description = "Large conference room with projector and whiteboard",
                    Capacity = 20,
                    Location = "1st Floor, East Wing",
                    Equipment = "Projector, Whiteboard, Video conferencing system, 20 chairs",
                    HourlyRate = 50.00m,
                    IsAvailable = true
                },
                new Room
                {
                    Name = "Meeting Room B",
                    Description = "Medium-sized meeting room for team meetings",
                    Capacity = 10,
                    Location = "2nd Floor, North Wing",
                    Equipment = "TV screen, Whiteboard, 10 chairs, Conference phone",
                    HourlyRate = 30.00m,
                    IsAvailable = true
                },
                new Room
                {
                    Name = "Training Room",
                    Description = "Large training room with presentation equipment",
                    Capacity = 35,
                    Location = "1st Floor, West Wing",
                    Equipment = "Projector, Sound system, 35 chairs, 2 Whiteboards, Flip charts",
                    HourlyRate = 75.00m,
                    IsAvailable = true
                },
                new Room
                {
                    Name = "Executive Boardroom",
                    Description = "Premium boardroom for executive meetings",
                    Capacity = 12,
                    Location = "3rd Floor, Executive Suite",
                    Equipment = "Large TV screen, Premium furniture, Coffee station, Conference phone",
                    HourlyRate = 100.00m,
                    IsAvailable = true
                },
                new Room
                {
                    Name = "Brainstorm Room",
                    Description = "Creative space for brainstorming sessions",
                    Capacity = 8,
                    Location = "2nd Floor, Creative Zone",
                    Equipment = "Whiteboards on all walls, Colorful furniture, Markers, Sticky notes",
                    HourlyRate = 25.00m,
                    IsAvailable = true
                }
            };

            context.Rooms.AddRange(rooms);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} sample rooms", rooms.Count);
        }
    }
}