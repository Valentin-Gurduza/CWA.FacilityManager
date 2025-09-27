using CWA.FacilityManager.Domain.Models;
using CWA.FacilityManager.Shared.DTOs;

namespace CWA.FacilityManager.Application.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileDto?> GetUserProfileAsync(string userId);
        Task<bool> UpdateUserProfileAsync(string userId, UpdateUserProfileDto updateDto);
        Task<List<UserHistoryDto>> GetUserHistoryAsync(string userId, int pageNumber = 1, int pageSize = 50);
        Task<List<BookingHistoryDto>> GetUserBookingHistoryAsync(string userId, int pageNumber = 1, int pageSize = 50);
        Task AddUserHistoryAsync(string userId, Domain.Enums.UserHistoryType historyType, string description, string? details = null, int? relatedEntityId = null, string? relatedEntityType = null);
    }
}