using CWA.FacilityManager.Application.Interfaces;
using CWA.FacilityManager.Application.Services;
using CWA.FacilityManager.Domain.Models;
using CWA.FacilityManager.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CWA.FacilityManager.Tests.ServicesTests
{
    public class EventServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly EventService _eventService;

        public EventServiceTests()
        {
            // Create in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _eventService = new EventService(_context);
        }

        [Fact]
        public async Task IsRoomAvailableAsync_NoConflicts_ReturnsTrue()
        {
            // Arrange
            var room = new Room
            {
                Id = 1,
                Name = "Conference Room A",
                Capacity = 20,
                BuildingId = 1,
                IsActive = true
            };
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            var startTime = DateTime.UtcNow.AddHours(2);
            var endTime = startTime.AddHours(1);

            // Act
            var isAvailable = await _eventService.IsRoomAvailableAsync(room.Id, startTime, endTime);

            // Assert
            Assert.True(isAvailable);
        }

        [Fact]
        public async Task IsRoomAvailableAsync_WithOverlappingApprovedEvent_ReturnsFalse()
        {
            // Arrange
            var room = new Room
            {
                Id = 1,
                Name = "Conference Room A",
                Capacity = 20,
                BuildingId = 1,
                IsActive = true
            };
            await _context.Rooms.AddAsync(room);

            var existingEvent = new Event
            {
                RoomId = room.Id,
                Title = "Existing Meeting",
                StartDateTime = DateTime.UtcNow.AddHours(2),
                EndDateTime = DateTime.UtcNow.AddHours(4),
                Status = EventStatus.Approved
            };
            await _context.Events.AddAsync(existingEvent);
            await _context.SaveChangesAsync();

            // Try to book overlapping time
            var startTime = DateTime.UtcNow.AddHours(3); // Overlaps with existing event
            var endTime = startTime.AddHours(2);

            // Act
            var isAvailable = await _eventService.IsRoomAvailableAsync(room.Id, startTime, endTime);

            // Assert
            Assert.False(isAvailable);
        }

        [Fact]
        public async Task GetConflictingEventsAsync_WithOverlappingApprovedEvent_ReturnsConflict()
        {
            // Arrange
            var room = new Room
            {
                Id = 1,
                Name = "Conference Room A",
                Capacity = 20,
                BuildingId = 1,
                IsActive = true
            };
            await _context.Rooms.AddAsync(room);

            var existingEvent = new Event
            {
                RoomId = room.Id,
                Title = "Existing Meeting",
                StartDateTime = DateTime.UtcNow.AddHours(2),
                EndDateTime = DateTime.UtcNow.AddHours(4),
                Status = EventStatus.Approved
            };
            await _context.Events.AddAsync(existingEvent);
            await _context.SaveChangesAsync();

            // Try to book overlapping time
            var startTime = DateTime.UtcNow.AddHours(3); // Overlaps with existing event
            var endTime = startTime.AddHours(2);

            // Act
            var conflicts = await _eventService.GetConflictingEventsAsync(room.Id, startTime, endTime);

            // Assert
            Assert.NotEmpty(conflicts);
            Assert.Contains(conflicts, e => e.Id == existingEvent.Id);
        }

        [Fact]
        public async Task IsRoomAvailableAsync_WithRejectedEvent_ReturnsTrue()
        {
            // Arrange
            var room = new Room
            {
                Id = 1,
                Name = "Conference Room A",
                Capacity = 20,
                BuildingId = 1,
                IsActive = true
            };
            await _context.Rooms.AddAsync(room);

            var rejectedEvent = new Event
            {
                RoomId = room.Id,
                Title = "Rejected Meeting",
                StartDateTime = DateTime.UtcNow.AddHours(2),
                EndDateTime = DateTime.UtcNow.AddHours(4),
                Status = EventStatus.Rejected
            };
            await _context.Events.AddAsync(rejectedEvent);
            await _context.SaveChangesAsync();

            // Try to book overlapping time with rejected event
            var startTime = DateTime.UtcNow.AddHours(3);
            var endTime = startTime.AddHours(2);

            // Act
            var isAvailable = await _eventService.IsRoomAvailableAsync(room.Id, startTime, endTime);

            // Assert
            Assert.True(isAvailable); // Rejected events should not block availability
        }

        [Fact]
        public async Task IsRoomAvailableAsync_WithPendingEvent_ReturnsFalse()
        {
            // Arrange
            var room = new Room
            {
                Id = 1,
                Name = "Conference Room A",
                Capacity = 20,
                BuildingId = 1,
                IsActive = true
            };
            await _context.Rooms.AddAsync(room);

            var pendingEvent = new Event
            {
                RoomId = room.Id,
                Title = "Pending Meeting",
                StartDateTime = DateTime.UtcNow.AddHours(2),
                EndDateTime = DateTime.UtcNow.AddHours(4),
                Status = EventStatus.Pending
            };
            await _context.Events.AddAsync(pendingEvent);
            await _context.SaveChangesAsync();

            // Try to book overlapping time
            var startTime = DateTime.UtcNow.AddHours(3);
            var endTime = startTime.AddHours(2);

            // Act
            var isAvailable = await _eventService.IsRoomAvailableAsync(room.Id, startTime, endTime);

            // Assert
            Assert.False(isAvailable); // Pending events should block availability
        }

        [Fact]
        public async Task IsRoomAvailableAsync_BackToBackEvents_ReturnsTrue()
        {
            // Arrange
            var room = new Room
            {
                Id = 1,
                Name = "Conference Room A",
                Capacity = 20,
                BuildingId = 1,
                IsActive = true
            };
            await _context.Rooms.AddAsync(room);

            var existingEvent = new Event
            {
                RoomId = room.Id,
                Title = "Existing Meeting",
                StartDateTime = DateTime.UtcNow.AddHours(2),
                EndDateTime = DateTime.UtcNow.AddHours(3),
                Status = EventStatus.Approved
            };
            await _context.Events.AddAsync(existingEvent);
            await _context.SaveChangesAsync();

            // Book immediately after existing event
            var startTime = DateTime.UtcNow.AddHours(3); // Exactly when previous ends
            var endTime = startTime.AddHours(1);

            // Act
            var isAvailable = await _eventService.IsRoomAvailableAsync(room.Id, startTime, endTime);

            // Assert
            Assert.True(isAvailable); // Back-to-back should not conflict
        }

        [Fact]
        public async Task GetPendingEventsAsync_ReturnsOnlyPendingEvents()
        {
            // Arrange
            var room = new Room
            {
                Id = 1,
                Name = "Conference Room A",
                Capacity = 20,
                BuildingId = 1,
                IsActive = true
            };
            await _context.Rooms.AddAsync(room);

            var events = new[]
            {
                new Event
                {
                    RoomId = room.Id,
                    Title = "Pending Event 1",
                    StartDateTime = DateTime.UtcNow.AddHours(2),
                    EndDateTime = DateTime.UtcNow.AddHours(3),
                    Status = EventStatus.Pending
                },
                new Event
                {
                    RoomId = room.Id,
                    Title = "Approved Event",
                    StartDateTime = DateTime.UtcNow.AddHours(4),
                    EndDateTime = DateTime.UtcNow.AddHours(5),
                    Status = EventStatus.Approved
                },
                new Event
                {
                    RoomId = room.Id,
                    Title = "Pending Event 2",
                    StartDateTime = DateTime.UtcNow.AddHours(6),
                    EndDateTime = DateTime.UtcNow.AddHours(7),
                    Status = EventStatus.Pending
                }
            };
            await _context.Events.AddRangeAsync(events);
            await _context.SaveChangesAsync();

            // Act
            var pendingEvents = (await _eventService.GetPendingEventsAsync()).ToList();

            // Assert
            Assert.Equal(2, pendingEvents.Count);
            Assert.All(pendingEvents, e => Assert.Equal(EventStatus.Pending, e.Status));
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
