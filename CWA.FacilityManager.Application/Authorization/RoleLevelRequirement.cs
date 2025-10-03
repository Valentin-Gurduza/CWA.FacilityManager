using Microsoft.AspNetCore.Authorization;

namespace CWA.FacilityManager.Application.Authorization
{
    /// <summary>
    /// Requirement for role-based access based on priority level
    /// </summary>
    public class RoleLevelRequirement : IAuthorizationRequirement
    {
        public int MinimumLevel { get; }

        public RoleLevelRequirement(int minimumLevel)
        {
            MinimumLevel = minimumLevel;
        }
    }
}
