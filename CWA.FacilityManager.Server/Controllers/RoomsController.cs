using CWA.FacilityManager.Application.Authorization;
using CWA.FacilityManager.Application.Interfaces;
using CWA.FacilityManager.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CWA.FacilityManager.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IAuditLogService _auditLogService;
        private readonly ILogger<RoomsController> _logger;

        public RoomsController(
            IRoomService roomService,
            IAuditLogService auditLogService,
            ILogger<RoomsController> logger)
        {
            _roomService = roomService;
            _auditLogService = auditLogService;
            _logger = logger;
        }

        /// <summary>
        /// Get all rooms
        /// </summary>
        [HttpGet]
        [Authorize(Policy = Policies.RenterOrHigher)]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
        {
            try
            {
                var rooms = await _roomService.GetAllRoomsAsync();
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rooms");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get room by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = Policies.RenterOrHigher)]
        public async Task<ActionResult<Room>> GetRoom(int id)
        {
            try
            {
                var room = await _roomService.GetRoomByIdAsync(id);
                if (room == null)
                    return NotFound();

                return Ok(room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving room {RoomId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get rooms by building
        /// </summary>
        [HttpGet("building/{buildingId}")]
        [Authorize(Policy = Policies.RenterOrHigher)]
        public async Task<ActionResult<IEnumerable<Room>>> GetRoomsByBuilding(int buildingId)
        {
            try
            {
                var rooms = await _roomService.GetRoomsByBuildingAsync(buildingId);
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rooms for building {BuildingId}", buildingId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get available rooms for a time slot
        /// </summary>
        [HttpGet("available")]
        [Authorize(Policy = Policies.RenterOrHigher)]
        public async Task<ActionResult<IEnumerable<Room>>> GetAvailableRooms(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int? capacity = null)
        {
            try
            {
                var rooms = await _roomService.GetAvailableRoomsAsync(startDate, endDate, capacity);
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available rooms");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create a new room (Admin/Secretary only)
        /// </summary>
        [HttpPost]
        [Authorize(Policy = Policies.CanManageRooms)]
        public async Task<ActionResult<Room>> CreateRoom([FromBody] Room room)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdRoom = await _roomService.CreateRoomAsync(room);

                // Audit log
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId != null)
                {
                    await _auditLogService.LogAsync(
                        userId,
                        "Create",
                        "Room",
                        createdRoom.Id.ToString(),
                        $"Created room: {createdRoom.Name}",
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );
                }

                return CreatedAtAction(nameof(GetRoom), new { id = createdRoom.Id }, createdRoom);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating room");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update a room (Admin/Secretary only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = Policies.CanManageRooms)]
        public async Task<ActionResult<Room>> UpdateRoom(int id, [FromBody] Room room)
        {
            try
            {
                if (id != room.Id)
                    return BadRequest("ID mismatch");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updatedRoom = await _roomService.UpdateRoomAsync(room);

                // Audit log
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId != null)
                {
                    await _auditLogService.LogAsync(
                        userId,
                        "Update",
                        "Room",
                        updatedRoom.Id.ToString(),
                        $"Updated room: {updatedRoom.Name}",
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );
                }

                return Ok(updatedRoom);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Room not found: {RoomId}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating room {RoomId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete a room (Admin/Secretary only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.CanManageRooms)]
        public async Task<ActionResult> DeleteRoom(int id)
        {
            try
            {
                var success = await _roomService.DeleteRoomAsync(id);
                if (!success)
                    return NotFound();

                // Audit log
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId != null)
                {
                    await _auditLogService.LogAsync(
                        userId,
                        "Delete",
                        "Room",
                        id.ToString(),
                        $"Deleted room ID: {id}",
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting room {RoomId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
