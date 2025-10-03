using CWA.FacilityManager.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CWA.FacilityManager.Application.Authorization
{
    /// <summary>
    /// Handler for role-level based authorization
    /// </summary>
    public class RoleLevelHandler : AuthorizationHandler<RoleLevelRequirement>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RoleLevelHandler(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            RoleLevelRequirement requirement)
        {
            if (context.User?.Identity?.IsAuthenticated != true)
            {
                return;
            }

            var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return;
            }

            var user = await _userManager.FindByIdAsync(userIdClaim.Value);
            if (user == null || !user.IsActive)
            {
                return;
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles == null || !userRoles.Any())
            {
                return;
            }

            // Get the highest priority from user's roles
            var maxPriority = 0;
            foreach (var roleName in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null && role.IsActive && role.Priority > maxPriority)
                {
                    maxPriority = role.Priority;
                }
            }

            if (maxPriority >= requirement.MinimumLevel)
            {
                context.Succeed(requirement);
            }
        }
    }
}
