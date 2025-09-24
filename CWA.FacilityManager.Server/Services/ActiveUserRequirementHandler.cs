using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CWA.FacilityManager.Server.Services
{
    public class ActiveUserRequirement : IAuthorizationRequirement
    {
    }

    public class ActiveUserRequirementHandler : AuthorizationHandler<ActiveUserRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ActiveUserRequirement requirement)
        {
            var isActiveClaim = context.User.FindFirst("IsActive");
            
            if (isActiveClaim != null && bool.TryParse(isActiveClaim.Value, out bool isActive) && isActive)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}