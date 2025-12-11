using CWA.FacilityManager.Application.Interfaces;
using CWA.FacilityManager.Domain.Enums;
using CWA.FacilityManager.Domain.Models;
using CWA.FacilityManager.Infrastructure.Contexts;
using CWA.FacilityManager.Shared.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CWA.FacilityManager.Application.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<UserProfileService> _logger;

        public UserProfileService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<UserProfileService> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return null;
                }

                // Get simple role names for backward compatibility
                var roles = await _userManager.GetRolesAsync(user);

                // Get detailed role information including permissions
                var detailedRoles = await _context.UserRoles
                    .Where(ur => ur.UserId == userId && ur.IsActive)
                    .Include(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions.Where(rp => rp.IsActive))
                        .ThenInclude(rp => rp.Permission)
                    .Select(ur => new UserRoleDetailDto
                    {
                        RoleId = ur.RoleId,
                        RoleName = ur.Role.Name!,
                        Description = ur.Role.Description,
                        Priority = ur.Role.Priority,
                        IsSystemRole = ur.Role.IsSystemRole,
                        IsActive = ur.Role.IsActive,
                        RoleType = ur.Role.RoleType.ToString(),
                        AssignedAt = ur.AssignedAt,
                        AssignedBy = ur.AssignedBy,
                        ExpiresAt = ur.ExpiresAt,
                        Notes = ur.Notes,
                        Permissions = ur.Role.RolePermissions
                            .Where(rp => rp.IsActive)
                            .Select(rp => new PermissionDetailDto
                            {
                                Id = rp.Permission.Id,
                                Name = rp.Permission.Name,
                                DisplayName = rp.Permission.DisplayName,
                                Description = rp.Permission.Description,
                                Module = rp.Permission.Module,
                                Resource = rp.Permission.Resource,
                                Action = rp.Permission.Action,
                                IsSystemPermission = rp.Permission.IsSystemPermission
                            }).ToList()
                    })
                    .OrderByDescending(r => r.Priority)
                    .ToListAsync();

                return new UserProfileDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName ?? "User",
                    LastName = user.LastName ?? "",
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber,
                    Bio = user.Bio,
                    Department = user.Department,
                    Position = user.Position,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt,
                    FullName = user.FullName,
                    Roles = roles.ToList(),
                    DetailedRoles = detailedRoles
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> UpdateUserProfileAsync(string userId, UpdateUserProfileDto updateDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return false;
                }

                // Track changes for history
                var changes = new List<string>();
                if (user.FirstName != updateDto.FirstName)
                    changes.Add($"First name changed from '{user.FirstName}' to '{updateDto.FirstName}'");
                if (user.LastName != updateDto.LastName)
                    changes.Add($"Last name changed from '{user.LastName}' to '{updateDto.LastName}'");
                if (user.PhoneNumber != updateDto.PhoneNumber)
                    changes.Add($"Phone number changed");
                if (user.Bio != updateDto.Bio)
                    changes.Add("Bio updated");
                if (user.Department != updateDto.Department)
                    changes.Add($"Department changed to '{updateDto.Department}'");
                if (user.Position != updateDto.Position)
                    changes.Add($"Position changed to '{updateDto.Position}'");

                // Update user properties
                user.FirstName = updateDto.FirstName;
                user.LastName = updateDto.LastName;
                user.PhoneNumber = updateDto.PhoneNumber;
                user.Bio = updateDto.Bio;
                user.Department = updateDto.Department;
                user.Position = updateDto.Position;

                var result = await _userManager.UpdateAsync(user);
                
                if (result.Succeeded && changes.Any())
                {
                    await AddUserHistoryAsync(userId, UserHistoryType.ProfileUpdated, 
                        "Profile information updated", string.Join("; ", changes));
                }

                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile for user {UserId}", userId);
                throw;
            }
        }

        public async Task<List<UserHistoryDto>> GetUserHistoryAsync(string userId, int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                var skip = (pageNumber - 1) * pageSize;

                var history = await _context.UserHistories
                    .Where(h => h.UserId == userId)
                    .OrderByDescending(h => h.CreatedAt)
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(h => new UserHistoryDto
                    {
                        Id = h.Id,
                        HistoryType = h.HistoryType,
                        Description = h.Description,
                        Details = h.Details,
                        RelatedEntityId = h.RelatedEntityId,
                        RelatedEntityType = h.RelatedEntityType,
                        CreatedAt = h.CreatedAt
                    })
                    .ToListAsync();

                return history;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user history for user {UserId}", userId);
                throw;
            }
        }

        public async Task<List<BookingHistoryDto>> GetUserBookingHistoryAsync(string userId, int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                var skip = (pageNumber - 1) * pageSize;

                var bookings = await _context.Bookings
                    .Include(b => b.Room)
                    .Where(b => b.UserId == userId)
                    .OrderByDescending(b => b.StartDate)
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(b => new BookingHistoryDto
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Description = b.Description,
                        StartDate = b.StartDate,
                        EndDate = b.EndDate,
                        Status = b.Status,
                        RoomName = b.Room.Name,
                        AttendeeCount = b.AttendeeCount,
                        TotalCost = b.TotalCost,
                        CreatedAt = b.CreatedAt
                    })
                    .ToListAsync();

                return bookings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user booking history for user {UserId}", userId);
                throw;
            }
        }

        public async Task AddUserHistoryAsync(string userId, UserHistoryType historyType, string description, 
            string? details = null, int? relatedEntityId = null, string? relatedEntityType = null)
        {
            try
            {
                var history = new UserHistory
                {
                    UserId = userId,
                    HistoryType = historyType,
                    Description = description,
                    Details = details,
                    RelatedEntityId = relatedEntityId,
                    RelatedEntityType = relatedEntityType,
                    CreatedAt = DateTime.UtcNow
                };

                _context.UserHistories.Add(history);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user history for user {UserId}", userId);
                // Don't rethrow here as history logging shouldn't break the main functionality
            }
        }
    }
}