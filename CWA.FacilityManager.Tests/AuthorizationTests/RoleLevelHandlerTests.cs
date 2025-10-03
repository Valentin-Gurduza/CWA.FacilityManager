using CWA.FacilityManager.Application.Authorization;
using CWA.FacilityManager.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Security.Claims;
using Xunit;

namespace CWA.FacilityManager.Tests.AuthorizationTests
{
    public class RoleLevelHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<RoleManager<ApplicationRole>> _mockRoleManager;
        private readonly RoleLevelHandler _handler;

        public RoleLevelHandlerTests()
        {
            // Setup UserManager mock
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object, null, null, null, null, null, null, null, null);

            // Setup RoleManager mock
            var roleStore = new Mock<IRoleStore<ApplicationRole>>();
            _mockRoleManager = new Mock<RoleManager<ApplicationRole>>(
                roleStore.Object, null, null, null, null);

            _handler = new RoleLevelHandler(_mockUserManager.Object, _mockRoleManager.Object);
        }

        [Fact]
        public async Task HandleRequirementAsync_UserNotAuthenticated_Fails()
        {
            // Arrange
            var requirement = new RoleLevelRequirement(RoleLevels.Renter);
            var user = new ClaimsPrincipal();
            var context = new AuthorizationHandlerContext(
                new[] { requirement },
                user,
                null);

            // Act
            await _handler.HandleAsync(context);

            // Assert
            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_AdministratorRole_MeetsAdministratorRequirement()
        {
            // Arrange
            var requirement = new RoleLevelRequirement(RoleLevels.Administrator);
            var userId = "test-user-id";
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, "admin@test.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            var testUser = new ApplicationUser { Id = userId };
            var adminRole = new ApplicationRole 
            { 
                Id = "admin-role-id",
                Name = "Administrator", 
                Priority = 100 
            };

            _mockUserManager.Setup(x => x.GetUserAsync(user))
                .ReturnsAsync(testUser);
            _mockUserManager.Setup(x => x.GetRolesAsync(testUser))
                .ReturnsAsync(new List<string> { "Administrator" });
            _mockRoleManager.Setup(x => x.FindByNameAsync("Administrator"))
                .ReturnsAsync(adminRole);

            var context = new AuthorizationHandlerContext(
                new[] { requirement },
                user,
                null);

            // Act
            await _handler.HandleAsync(context);

            // Assert
            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_SecretaryRole_MeetsSecretaryRequirement()
        {
            // Arrange
            var requirement = new RoleLevelRequirement(RoleLevels.Secretary);
            var userId = "test-user-id";
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, "secretary@test.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            var testUser = new ApplicationUser { Id = userId };
            var secretaryRole = new ApplicationRole 
            { 
                Id = "secretary-role-id",
                Name = "Secretary", 
                Priority = 50 
            };

            _mockUserManager.Setup(x => x.GetUserAsync(user))
                .ReturnsAsync(testUser);
            _mockUserManager.Setup(x => x.GetRolesAsync(testUser))
                .ReturnsAsync(new List<string> { "Secretary" });
            _mockRoleManager.Setup(x => x.FindByNameAsync("Secretary"))
                .ReturnsAsync(secretaryRole);

            var context = new AuthorizationHandlerContext(
                new[] { requirement },
                user,
                null);

            // Act
            await _handler.HandleAsync(context);

            // Assert
            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_RenterRole_FailsSecretaryRequirement()
        {
            // Arrange
            var requirement = new RoleLevelRequirement(RoleLevels.Secretary);
            var userId = "test-user-id";
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, "renter@test.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            var testUser = new ApplicationUser { Id = userId };
            var renterRole = new ApplicationRole 
            { 
                Id = "renter-role-id",
                Name = "Renter", 
                Priority = 10 
            };

            _mockUserManager.Setup(x => x.GetUserAsync(user))
                .ReturnsAsync(testUser);
            _mockUserManager.Setup(x => x.GetRolesAsync(testUser))
                .ReturnsAsync(new List<string> { "Renter" });
            _mockRoleManager.Setup(x => x.FindByNameAsync("Renter"))
                .ReturnsAsync(renterRole);

            var context = new AuthorizationHandlerContext(
                new[] { requirement },
                user,
                null);

            // Act
            await _handler.HandleAsync(context);

            // Assert
            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_AdministratorRole_MeetsSecretaryRequirement()
        {
            // Arrange - Administrator (level 100) should meet Secretary requirement (level 50)
            var requirement = new RoleLevelRequirement(RoleLevels.Secretary);
            var userId = "test-user-id";
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, "admin@test.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            var testUser = new ApplicationUser { Id = userId };
            var adminRole = new ApplicationRole 
            { 
                Id = "admin-role-id",
                Name = "Administrator", 
                Priority = 100 
            };

            _mockUserManager.Setup(x => x.GetUserAsync(user))
                .ReturnsAsync(testUser);
            _mockUserManager.Setup(x => x.GetRolesAsync(testUser))
                .ReturnsAsync(new List<string> { "Administrator" });
            _mockRoleManager.Setup(x => x.FindByNameAsync("Administrator"))
                .ReturnsAsync(adminRole);

            var context = new AuthorizationHandlerContext(
                new[] { requirement },
                user,
                null);

            // Act
            await _handler.HandleAsync(context);

            // Assert
            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_SecretaryRole_MeetsRenterRequirement()
        {
            // Arrange - Secretary (level 50) should meet Renter requirement (level 10)
            var requirement = new RoleLevelRequirement(RoleLevels.Renter);
            var userId = "test-user-id";
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, "secretary@test.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            var testUser = new ApplicationUser { Id = userId };
            var secretaryRole = new ApplicationRole 
            { 
                Id = "secretary-role-id",
                Name = "Secretary", 
                Priority = 50 
            };

            _mockUserManager.Setup(x => x.GetUserAsync(user))
                .ReturnsAsync(testUser);
            _mockUserManager.Setup(x => x.GetRolesAsync(testUser))
                .ReturnsAsync(new List<string> { "Secretary" });
            _mockRoleManager.Setup(x => x.FindByNameAsync("Secretary"))
                .ReturnsAsync(secretaryRole);

            var context = new AuthorizationHandlerContext(
                new[] { requirement },
                user,
                null);

            // Act
            await _handler.HandleAsync(context);

            // Assert
            Assert.True(context.HasSucceeded);
        }
    }
}
