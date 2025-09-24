using Microsoft.AspNetCore.Components;
using System.Security.Claims;

namespace CWA.FacilityManager.Server.Services
{
    public interface IRoleBasedRedirectService
    {
        Task<string> GetValidReturnUrlAsync(string? returnUrl, ClaimsPrincipal user);
        bool IsAdminOnlyPage(string url);
        bool UserHasAdminAccess(ClaimsPrincipal user);
    }

    public class RoleBasedRedirectService : IRoleBasedRedirectService
    {
        private readonly ILogger<RoleBasedRedirectService> _logger;

        // Define admin-only pages
        private readonly HashSet<string> _adminOnlyPages = new(StringComparer.OrdinalIgnoreCase)
        {
            "/admin",
            "/admin/users",
            "/admin/users/create",
            "/admin/roles",
            "/admin/permissions"
        };

        public RoleBasedRedirectService(ILogger<RoleBasedRedirectService> logger)
        {
            _logger = logger;
        }

        public async Task<string> GetValidReturnUrlAsync(string? returnUrl, ClaimsPrincipal user)
        {
            // If no return URL specified, redirect to home
            if (string.IsNullOrEmpty(returnUrl))
            {
                return "/";
            }

            try
            {
                // Convert to relative path for comparison
                var uri = new Uri(returnUrl, UriKind.RelativeOrAbsolute);
                string relativePath;
                
                if (uri.IsAbsoluteUri)
                {
                    relativePath = uri.AbsolutePath;
                }
                else
                {
                    relativePath = returnUrl;
                }

                // Normalize the path (remove query parameters and fragments for checking)
                var pathOnly = relativePath.Split('?', '#')[0];

                _logger.LogDebug("Checking access for path: {Path} for user: {UserName}", pathOnly, user.Identity?.Name);

                // Check if this is an admin-only page
                if (IsAdminOnlyPage(pathOnly))
                {
                    if (!UserHasAdminAccess(user))
                    {
                        _logger.LogInformation("User {UserName} attempted to access admin page {Path}, redirecting to home", 
                            user.Identity?.Name, pathOnly);
                        return "/";
                    }
                }

                // Check for other role-specific pages can be added here
                // For example, if you have manager-only or secretary-only pages

                // If user has access or it's a general page, return the original URL
                return returnUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating return URL: {ReturnUrl}", returnUrl);
                return "/";
            }
        }

        public bool IsAdminOnlyPage(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            // Normalize the URL
            var normalizedUrl = url.TrimEnd('/').ToLowerInvariant();
            
            // Check exact matches
            if (_adminOnlyPages.Contains(normalizedUrl))
                return true;

            // Check if it starts with admin paths
            return normalizedUrl.StartsWith("/admin/users/") || 
                   normalizedUrl.StartsWith("/admin/roles/") ||
                   normalizedUrl.StartsWith("/admin/permissions/");
        }

        public bool UserHasAdminAccess(ClaimsPrincipal user)
        {
            if (user?.Identity?.IsAuthenticated != true)
                return false;

            // Check if user is active
            var isActiveClaim = user.FindFirst("IsActive");
            if (isActiveClaim == null || !bool.TryParse(isActiveClaim.Value, out bool isActive) || !isActive)
            {
                return false;
            }

            // Check if user has Administrator or Secretary role
            return user.IsInRole("Administrator") || user.IsInRole("Secretary");
        }
    }
}