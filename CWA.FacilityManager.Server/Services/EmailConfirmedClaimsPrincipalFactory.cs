using System.Security.Claims;
using CWA.FacilityManager.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CWA.FacilityManager.Server.Services
{
    public class EmailConfirmedClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        public EmailConfirmedClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor) : base(userManager, roleManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            
            // Add email confirmation claim
            if (await UserManager.IsEmailConfirmedAsync(user))
            {
                identity.AddClaim(new Claim("EmailConfirmed", "True"));
            }
            else
            {
                identity.AddClaim(new Claim("EmailConfirmed", "False"));
            }

            // Add user active status claim - CRITICAL for preventing deactivated users from accessing the system
            identity.AddClaim(new Claim("IsActive", user.IsActive.ToString()));
            
            // Add user ID for easy access
            identity.AddClaim(new Claim("UserId", user.Id));

            return identity;
        }
    }
}