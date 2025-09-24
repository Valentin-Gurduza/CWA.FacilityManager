using Microsoft.AspNetCore.Mvc;
using CWA.FacilityManager.Application.Services.UserManagement;
using CWA.FacilityManager.Application.Interfaces.UserManagement;
using System.Security.Claims;

namespace CWA.FacilityManager.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemValidationController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IRoleManagementService _roleManagementService;
        private readonly IPermissionService _permissionService;
        private readonly RoleInitializationService _roleInitializationService;

        public SystemValidationController(
            IUserManagementService userManagementService,
            IRoleManagementService roleManagementService,
            IPermissionService permissionService,
            RoleInitializationService roleInitializationService)
        {
            _userManagementService = userManagementService;
            _roleManagementService = roleManagementService;
            _permissionService = permissionService;
            _roleInitializationService = roleInitializationService;
        }

        [HttpGet("system-status")]
        public async Task<IActionResult> GetSystemStatus()
        {
            try
            {
                var status = new
                {
                    SystemInitialized = true,
                    Timestamp = DateTime.UtcNow,
                    
                    // Permission statistics
                    Permissions = new
                    {
                        Total = (await _permissionService.GetAllPermissionsAsync()).Count(),
                        Modules = (await _permissionService.GetModulesAsync()).Count(),
                        Resources = (await _permissionService.GetResourcesAsync()).Count(),
                        Actions = (await _permissionService.GetActionsAsync()).Count()
                    },
                    
                    // Role statistics
                    Roles = new
                    {
                        Total = (await _roleManagementService.GetAllRolesAsync()).Count(),
                        Active = (await _roleManagementService.GetActiveRolesAsync()).Count(),
                        System = (await _roleManagementService.GetAllRolesAsync())
                                .Count(r => r.IsSystemRole)
                    },
                    
                    // Role permission assignments
                    RolePermissionAssignments = await _roleInitializationService.GetRolePermissionsSummaryAsync(),
                    
                    // Validation status
                    ValidationResults = new
                    {
                        RolePermissionsValid = await _roleInitializationService.ValidateRolePermissionsAsync(),
                        AllSystemRolesPresent = await ValidateSystemRolesAsync(),
                        AllPermissionsSeeded = await ValidatePermissionsAsync()
                    }
                };

                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpGet("role-permissions/{roleName}")]
        public async Task<IActionResult> GetRolePermissions(string roleName)
        {
            try
            {
                var role = await _roleManagementService.GetRoleByNameAsync(roleName);
                if (role == null)
                {
                    return NotFound($"Role '{roleName}' not found");
                }

                var permissions = await _roleManagementService.GetRolePermissionsAsync(role.Id);
                var groupedPermissions = permissions.GroupBy(p => p.Module)
                    .Select(g => new
                    {
                        Module = g.Key,
                        Permissions = g.Select(p => new
                        {
                            p.Name,
                            p.DisplayName,
                            p.Description,
                            p.Resource,
                            p.Action
                        }).OrderBy(p => p.Resource).ThenBy(p => p.Action)
                    }).OrderBy(g => g.Module);

                return Ok(new
                {
                    Role = new
                    {
                        role.Name,
                        role.Description,
                        role.Priority,
                        role.IsSystemRole,
                        role.RoleType
                    },
                    TotalPermissions = permissions.Count(),
                    PermissionsByModule = groupedPermissions
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpPost("validate-system")]
        public async Task<IActionResult> ValidateSystem()
        {
            try
            {
                var validationResults = new List<object>();

                // Validate role permissions
                var rolePermissionsValid = await _roleInitializationService.ValidateRolePermissionsAsync();
                validationResults.Add(new
                {
                    Test = "Role Permissions Validation",
                    Status = rolePermissionsValid ? "PASS" : "FAIL",
                    Message = rolePermissionsValid ? "All roles have correct permission counts" : "Some roles have incorrect permission assignments"
                });

                // Validate system roles exist
                var systemRolesValid = await ValidateSystemRolesAsync();
                validationResults.Add(new
                {
                    Test = "System Roles Validation",
                    Status = systemRolesValid ? "PASS" : "FAIL",
                    Message = systemRolesValid ? "All system roles are present" : "Some system roles are missing"
                });

                // Validate permissions seeded
                var permissionsValid = await ValidatePermissionsAsync();
                validationResults.Add(new
                {
                    Test = "Permissions Seeding Validation",
                    Status = permissionsValid ? "PASS" : "FAIL",
                    Message = permissionsValid ? "All system permissions are present" : "Some permissions are missing"
                });

                var allTestsPassed = rolePermissionsValid && systemRolesValid && permissionsValid;

                return Ok(new
                {
                    OverallStatus = allTestsPassed ? "PASS" : "FAIL",
                    Message = allTestsPassed ? "System validation completed successfully" : "System validation found issues",
                    ValidationResults = validationResults,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpGet("current-user-info")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> GetCurrentUserInfo()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found");
                }

                var userDto = await _userManagementService.GetUserByIdAsync(userId);
                if (userDto == null)
                {
                    return NotFound("User not found in database");
                }

                var userClaims = User.Claims.Select(c => new { Type = c.Type, Value = c.Value }).ToList();
                var userRoles = await _userManagementService.GetUserRolesAsync(userId);
                var userPermissions = await _userManagementService.GetUserPermissionsAsync(userId);

                var response = new
                {
                    DatabaseInfo = new
                    {
                        userDto.Id,
                        userDto.UserName,
                        userDto.Email,
                        userDto.IsActive,
                        DatabaseRoles = userDto.Roles,
                        DatabasePermissions = userDto.Permissions
                    },
                    ClaimsInfo = new
                    {
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        UserName = User.FindFirstValue(ClaimTypes.Name),
                        Email = User.FindFirstValue(ClaimTypes.Email),
                        IsAuthenticated = User.Identity?.IsAuthenticated,
                        AuthenticationType = User.Identity?.AuthenticationType,
                        Claims = userClaims,
                        RoleChecks = new
                        {
                            IsAdministrator = User.IsInRole("Administrator"),
                            IsSecretary = User.IsInRole("Secretary"),
                            IsRenter = User.IsInRole("Renter")
                        }
                    },
                    ServiceInfo = new
                    {
                        ServiceRoles = userRoles,
                        ServicePermissions = userPermissions
                    },
                    Timestamp = DateTime.UtcNow
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }

        private async Task<bool> ValidateSystemRolesAsync()
        {
            var roles = await _roleManagementService.GetAllRolesAsync();
            var systemRoleNames = new[] { "Administrator", "Secretary", "Renter" };
            
            return systemRoleNames.All(roleName => 
                roles.Any(r => r.Name == roleName && r.IsSystemRole));
        }

        private async Task<bool> ValidatePermissionsAsync()
        {
            var permissions = await _permissionService.GetAllPermissionsAsync();
            
            // Expected minimum permission counts by module
            var expectedModules = new Dictionary<string, int>
            {
                ["UserManagement"] = 14,     // 6+5+3 permissions
                ["FacilityManagement"] = 10, // 4+6 permissions  
                ["EventManagement"] = 7,     // 7 permissions
                ["BookingManagement"] = 7,   // 7 permissions
                ["ReportManagement"] = 3,    // 3 permissions
                ["SystemManagement"] = 4     // 4 permissions
            };

            var modulePermissionCounts = permissions.GroupBy(p => p.Module)
                .ToDictionary(g => g.Key, g => g.Count());

            return expectedModules.All(expected => 
                modulePermissionCounts.ContainsKey(expected.Key) && 
                modulePermissionCounts[expected.Key] >= expected.Value);
        }
    }
}