using CWA.FacilityManager.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CWA.FacilityManager.Application.Services
{
    public class RoleInitializationService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RoleInitializationService> _logger;

        public RoleInitializationService(RoleManager<IdentityRole> roleManager, ILogger<RoleInitializationService> logger)
        {
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task InitializeRolesAsync()
        {
            var roles = Enum.GetValues<UserRole>().Select(r => r.ToString()).ToArray();

            foreach (var roleName in roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Created role: {RoleName}", roleName);
                    }
                    else
                    {
                        _logger.LogError("Failed to create role {RoleName}: {Errors}", roleName, 
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
        }
    }
}