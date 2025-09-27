using CWA.FacilityManager.Application.Interfaces;
using CWA.FacilityManager.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CWA.FacilityManager.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly ILogger<UserProfileController> _logger;

        public UserProfileController(IUserProfileService userProfileService, ILogger<UserProfileController> logger)
        {
            _userProfileService = userProfileService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var profile = await _userProfileService.GetUserProfileAsync(userId);
                if (profile == null)
                {
                    return NotFound();
                }

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile");
                return StatusCode(500, "An error occurred while retrieving the profile");
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateProfile([FromBody] UpdateUserProfileDto updateDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _userProfileService.UpdateUserProfileAsync(userId, updateDto);
                if (!success)
                {
                    return NotFound();
                }

                return Ok(new { message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                return StatusCode(500, "An error occurred while updating the profile");
            }
        }

        [HttpGet("history")]
        public async Task<ActionResult<List<UserHistoryDto>>> GetHistory([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                if (pageSize > 100) pageSize = 100; // Limit page size

                var history = await _userProfileService.GetUserHistoryAsync(userId, pageNumber, pageSize);
                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user history");
                return StatusCode(500, "An error occurred while retrieving the history");
            }
        }

        [HttpGet("booking-history")]
        public async Task<ActionResult<List<BookingHistoryDto>>> GetBookingHistory([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                if (pageSize > 100) pageSize = 100; // Limit page size

                var bookingHistory = await _userProfileService.GetUserBookingHistoryAsync(userId, pageNumber, pageSize);
                return Ok(bookingHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user booking history");
                return StatusCode(500, "An error occurred while retrieving the booking history");
            }
        }
    }
}