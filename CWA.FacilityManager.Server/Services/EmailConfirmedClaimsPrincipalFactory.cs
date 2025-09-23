using System.Security.Claims;
using CWA.FacilityManager.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CWA.FacilityManager.Server.Services
{
    public class EmailConfirmedClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
    {
        public EmailConfirmedClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
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

            return identity;
        }
    }
}