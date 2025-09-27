using AutoMapper;
using CWA.FacilityManager.Application.DTOs.UserManagement;
using CWA.FacilityManager.Application.Interfaces.UserManagement;
using CWA.FacilityManager.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CWA.FacilityManager.Application.Services.UserManagement
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserManagementService> _logger;
        private readonly IEmailSender<ApplicationUser>? _emailSender;

        public UserManagementService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IMapper mapper,
            ILogger<UserManagementService> logger,
            IEmailSender<ApplicationUser>? emailSender = null)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _logger = logger;
            _emailSender = emailSender;
        }

        public async Task<UserDto?> GetUserByIdAsync(string userId)
        {
            try
            {
                var user = await _userManager.Users
                    .Where(u => u.Id == userId)
                    .Include(u => u.UserRoles.Where(ur => ur.IsActive))
                    .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions.Where(rp => rp.IsActive))
                    .ThenInclude(rp => rp.Permission)
                    .FirstOrDefaultAsync();

                if (user == null) return null;

                var userDto = _mapper.Map<UserDto>(user);
                
                // Map roles and permissions from the already loaded data
                userDto.Roles = user.UserRoles
                    .Where(ur => ur.IsActive && ur.Role != null)
                    .Select(ur => ur.Role.Name!)
                    .ToList();
                
                userDto.Permissions = user.UserRoles
                    .Where(ur => ur.IsActive && ur.Role != null)
                    .SelectMany(ur => ur.Role.RolePermissions.Where(rp => rp.IsActive && rp.Permission != null))
                    .Select(rp => rp.Permission.PolicyName)
                    .Distinct()
                    .ToList();

                return userDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {UserId}", userId);
                throw;
            }
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                return user != null ? await GetUserByIdAsync(user.Id) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email: {Email}", email);
                throw;
            }
        }

        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                return user != null ? await GetUserByIdAsync(user.Id) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by username: {Username}", username);
                throw;
            }
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userManager.Users
                    .Include(u => u.UserRoles.Where(ur => ur.IsActive))
                        .ThenInclude(ur => ur.Role)
                    .OrderBy(u => u.UserName)
                    .ToListAsync();

                var userDtos = new List<UserDto>();
                foreach (var user in users)
                {
                    var userDto = _mapper.Map<UserDto>(user);
                    userDto.Roles = user.UserRoles
                        .Where(ur => ur.IsActive && ur.Role != null)
                        .Select(ur => ur.Role.Name!)
                        .ToList();
                    userDtos.Add(userDto);
                }

                return userDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                throw;
            }
        }

        public async Task<IEnumerable<UserDto>> GetActiveUsersAsync()
        {
            try
            {
                var users = await _userManager.Users
                    .Where(u => u.IsActive)
                    .Include(u => u.UserRoles.Where(ur => ur.IsActive))
                        .ThenInclude(ur => ur.Role)
                    .OrderBy(u => u.UserName)
                    .ToListAsync();

                var userDtos = new List<UserDto>();
                foreach (var user in users)
                {
                    var userDto = _mapper.Map<UserDto>(user);
                    userDto.Roles = user.UserRoles
                        .Where(ur => ur.IsActive && ur.Role != null)
                        .Select(ur => ur.Role.Name!)
                        .ToList();
                    userDtos.Add(userDto);
                }

                return userDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active users");
                throw;
            }
        }

        public async Task<IEnumerable<UserDto>> GetInactiveUsersAsync()
        {
            try
            {
                var users = await _userManager.Users
                    .Where(u => !u.IsActive)
                    .Include(u => u.UserRoles.Where(ur => ur.IsActive))
                        .ThenInclude(ur => ur.Role)
                    .OrderBy(u => u.UserName)
                    .ToListAsync();

                var userDtos = new List<UserDto>();
                foreach (var user in users)
                {
                    var userDto = _mapper.Map<UserDto>(user);
                    userDto.Roles = user.UserRoles
                        .Where(ur => ur.IsActive && ur.Role != null)
                        .Select(ur => ur.Role.Name!)
                        .ToList();
                    userDtos.Add(userDto);
                }

                return userDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inactive users");
                throw;
            }
        }

        public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string roleName)
        {
            try
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
                var userDtos = new List<UserDto>();

                foreach (var user in usersInRole)
                {
                    var userDto = await GetUserByIdAsync(user.Id);
                    if (userDto != null)
                        userDtos.Add(userDto);
                }

                return userDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users by role: {RoleName}", roleName);
                throw;
            }
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto, string createdBy)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = createUserDto.UserName,
                    Email = createUserDto.Email,
                    FirstName = createUserDto.FirstName,
                    LastName = createUserDto.LastName,
                    PhoneNumber = createUserDto.PhoneNumber,
                    Department = createUserDto.Department,
                    JobTitle = createUserDto.JobTitle,
                    CreatedBy = createdBy,
                    EmailConfirmed = !createUserDto.SendEmailConfirmation // If not sending email, auto-confirm
                };

                var result = await _userManager.CreateAsync(user, createUserDto.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to create user: {errors}");
                }

                // Send email confirmation if requested and email service is available
                if (createUserDto.SendEmailConfirmation && _emailSender != null)
                {
                    try
                    {
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        // Create confirmation link (in production, this should be your domain)
                        var confirmationLink = $"https://localhost:5076/Account/ConfirmEmail?userId={user.Id}&code={Uri.EscapeDataString(token)}";
                        
                        await _emailSender.SendConfirmationLinkAsync(user, user.Email!, confirmationLink);
                        _logger.LogInformation("Email confirmation sent to {Email}", user.Email);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to send email confirmation to {Email}, auto-confirming for development", user.Email);
                        // Auto-confirm email to prevent user from being locked out
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmResult = await _userManager.ConfirmEmailAsync(user, token);
                        if (confirmResult.Succeeded)
                        {
                            _logger.LogInformation("Email automatically confirmed due to email sending failure: {Email}", user.Email);
                        }
                    }
                }
                else if (!createUserDto.SendEmailConfirmation)
                {
                    // Auto-confirm if email confirmation is not requested
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmResult = await _userManager.ConfirmEmailAsync(user, token);
                    if (confirmResult.Succeeded)
                    {
                        _logger.LogInformation("Email automatically confirmed (no confirmation requested): {Email}", user.Email);
                    }
                }

                // Assign roles
                if (createUserDto.RoleIds.Any())
                {
                    foreach (var roleId in createUserDto.RoleIds)
                    {
                        await AssignRoleToUserAsync(user.Id, roleId, createdBy);
                    }
                }

                _logger.LogInformation("User created successfully: {UserId} by {CreatedBy}", user.Id, createdBy);
                return (await GetUserByIdAsync(user.Id))!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user: {Email}", createUserDto.Email);
                throw;
            }
        }

        public async Task<UserDto> UpdateUserAsync(UpdateUserDto updateUserDto, string modifiedBy)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(updateUserDto.Id);
                if (user == null)
                    throw new ArgumentException($"User with ID {updateUserDto.Id} not found");

                user.UserName = updateUserDto.UserName;
                user.Email = updateUserDto.Email;
                user.FirstName = updateUserDto.FirstName;
                user.LastName = updateUserDto.LastName;
                user.PhoneNumber = updateUserDto.PhoneNumber;
                user.Department = updateUserDto.Department;
                user.JobTitle = updateUserDto.JobTitle;
                user.IsActive = updateUserDto.IsActive;
                user.ModifiedBy = modifiedBy;
                user.LastModifiedAt = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to update user: {errors}");
                }

                _logger.LogInformation("User updated successfully: {UserId} by {ModifiedBy}", user.Id, modifiedBy);
                return (await GetUserByIdAsync(user.Id))!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {UserId}", updateUserDto.Id);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(string userId, string deletedBy)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;

                // Soft delete by deactivating
                user.IsActive = false;
                user.ModifiedBy = deletedBy;
                user.LastModifiedAt = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                
                _logger.LogInformation("User deleted successfully: {UserId} by {DeletedBy}", userId, deletedBy);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ActivateUserAsync(string userId, string modifiedBy)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;

                user.IsActive = true;
                user.ModifiedBy = modifiedBy;
                user.LastModifiedAt = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                
                _logger.LogInformation("User activated successfully: {UserId} by {ModifiedBy}", userId, modifiedBy);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> DeactivateUserAsync(string userId, string modifiedBy)
        {
            return await DeleteUserAsync(userId, modifiedBy);
        }

        // Role management methods
        public async Task<bool> AssignRoleToUserAsync(string userId, string roleId, string assignedBy, DateTime? expiresAt = null, string? notes = null)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                var role = await _roleManager.FindByIdAsync(roleId);
                
                if (user == null || role == null) return false;

                var result = await _userManager.AddToRoleAsync(user, role.Name!);
                
                _logger.LogInformation("Role {RoleId} assigned to user {UserId} by {AssignedBy}", roleId, userId, assignedBy);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role {RoleId} to user {UserId}", roleId, userId);
                throw;
            }
        }

        public async Task<bool> RemoveRoleFromUserAsync(string userId, string roleId, string removedBy)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                var role = await _roleManager.FindByIdAsync(roleId);
                
                if (user == null || role == null) return false;

                var result = await _userManager.RemoveFromRoleAsync(user, role.Name!);
                
                _logger.LogInformation("Role {RoleId} removed from user {UserId} by {RemovedBy}", roleId, userId, removedBy);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing role {RoleId} from user {UserId}", roleId, userId);
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return Enumerable.Empty<string>();

                return await _userManager.GetRolesAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user roles: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<UserDto>> GetUsersInRoleAsync(string roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null) return Enumerable.Empty<UserDto>();

                return await GetUsersByRoleAsync(role.Name!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users in role: {RoleId}", roleId);
                throw;
            }
        }

        // Permission methods
        public async Task<IEnumerable<string>> GetUserPermissionsAsync(string userId)
        {
            try
            {
                var user = await _userManager.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null) return Enumerable.Empty<string>();

                return user.UserRoles
                    .Where(ur => ur.IsActive)
                    .SelectMany(ur => ur.Role.RolePermissions.Where(rp => rp.IsActive))
                    .Select(rp => rp.Permission.PolicyName)
                    .Distinct();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user permissions: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> UserHasPermissionAsync(string userId, string permission)
        {
            try
            {
                var userPermissions = await GetUserPermissionsAsync(userId);
                return userPermissions.Contains(permission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking user permission: {UserId}, {Permission}", userId, permission);
                throw;
            }
        }

        public async Task<bool> UserHasAnyPermissionAsync(string userId, params string[] permissions)
        {
            try
            {
                var userPermissions = await GetUserPermissionsAsync(userId);
                return permissions.Any(p => userPermissions.Contains(p));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking user any permissions: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> UserHasAllPermissionsAsync(string userId, params string[] permissions)
        {
            try
            {
                var userPermissions = await GetUserPermissionsAsync(userId);
                return permissions.All(p => userPermissions.Contains(p));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking user all permissions: {UserId}", userId);
                throw;
            }
        }

        // Authentication helper methods
        public async Task<bool> CheckPasswordAsync(string userId, string password)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;

                return await _userManager.CheckPasswordAsync(user, password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking password for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;

                var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ResetPasswordAsync(string userId, string newPassword, string resetBy)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
                
                if (result.Succeeded)
                {
                    user.ModifiedBy = resetBy;
                    user.LastModifiedAt = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);
                }

                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> LockUserAsync(string userId, DateTime? lockoutEnd, string lockedBy)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;

                var result = await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);
                
                if (result.Succeeded)
                {
                    user.ModifiedBy = lockedBy;
                    user.LastModifiedAt = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);
                }

                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error locking user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> UnlockUserAsync(string userId, string unlockedBy)
        {
            return await LockUserAsync(userId, null, unlockedBy);
        }

        // Profile management
        public async Task<bool> UpdateUserProfileAsync(string userId, string? firstName, string? lastName, string? department, string? jobTitle, string? profilePictureUrl)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;

                user.FirstName = firstName;
                user.LastName = lastName;
                user.Department = department;
                user.JobTitle = jobTitle;
                user.ProfilePictureUrl = profilePictureUrl;
                user.LastModifiedAt = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> UpdateLastLoginAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;

                user.LastLoginAt = DateTime.UtcNow;
                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating last login for user: {UserId}", userId);
                throw;
            }
        }

        // Search and filtering
        public async Task<IEnumerable<UserDto>> SearchUsersAsync(string searchTerm)
        {
            try
            {
                var users = await _userManager.Users
                    .Where(u => u.UserName!.Contains(searchTerm) || 
                               u.Email!.Contains(searchTerm) ||
                               u.FirstName!.Contains(searchTerm) ||
                               u.LastName!.Contains(searchTerm))
                    .Include(u => u.UserRoles.Where(ur => ur.IsActive))
                        .ThenInclude(ur => ur.Role)
                    .OrderBy(u => u.UserName)
                    .ToListAsync();

                var userDtos = new List<UserDto>();
                foreach (var user in users)
                {
                    var userDto = _mapper.Map<UserDto>(user);
                    userDto.Roles = user.UserRoles
                        .Where(ur => ur.IsActive && ur.Role != null)
                        .Select(ur => ur.Role.Name!)
                        .ToList();
                    userDtos.Add(userDto);
                }

                return userDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching users: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<UserDto>> GetUsersPaginatedAsync(int page, int pageSize, string? searchTerm = null, string? sortBy = null, bool sortDescending = false)
        {
            try
            {
                var query = _userManager.Users.AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(u => u.UserName!.Contains(searchTerm) || 
                                           u.Email!.Contains(searchTerm) ||
                                           u.FirstName!.Contains(searchTerm) ||
                                           u.LastName!.Contains(searchTerm));
                }

                // Apply sorting
                if (!string.IsNullOrEmpty(sortBy))
                {
                    switch (sortBy.ToLower())
                    {
                        case "username":
                            query = sortDescending ? query.OrderByDescending(u => u.UserName) : query.OrderBy(u => u.UserName);
                            break;
                        case "email":
                            query = sortDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email);
                            break;
                        case "firstname":
                            query = sortDescending ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName);
                            break;
                        case "lastname":
                            query = sortDescending ? query.OrderByDescending(u => u.LastName) : query.OrderBy(u => u.LastName);
                            break;
                        case "createdat":
                            query = sortDescending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt);
                            break;
                        default:
                            query = query.OrderBy(u => u.UserName);
                            break;
                    }
                }
                else
                {
                    query = query.OrderBy(u => u.UserName);
                }

                var users = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Include(u => u.UserRoles.Where(ur => ur.IsActive))
                        .ThenInclude(ur => ur.Role)
                    .ToListAsync();

                var userDtos = new List<UserDto>();
                foreach (var user in users)
                {
                    var userDto = _mapper.Map<UserDto>(user);
                    userDto.Roles = user.UserRoles
                        .Where(ur => ur.IsActive && ur.Role != null)
                        .Select(ur => ur.Role.Name!)
                        .ToList();
                    userDtos.Add(userDto);
                }

                return userDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated users");
                throw;
            }
        }

        public async Task<int> GetUsersCountAsync(string? searchTerm = null)
        {
            try
            {
                var query = _userManager.Users.AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(u => u.UserName!.Contains(searchTerm) || 
                                           u.Email!.Contains(searchTerm) ||
                                           u.FirstName!.Contains(searchTerm) ||
                                           u.LastName!.Contains(searchTerm));
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users count");
                throw;
            }
        }
    }
}