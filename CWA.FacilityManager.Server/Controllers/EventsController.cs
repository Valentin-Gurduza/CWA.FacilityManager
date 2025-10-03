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
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IAuditLogService _auditLogService;
        private readonly ILogger<EventsController> _logger;

        public EventsController(
            IEventService eventService,
            IAuditLogService auditLogService,
            ILogger<EventsController> logger)
        {
            _eventService = eventService;
            _auditLogService = auditLogService;
            _logger = logger;
        }

        /// <summary>
        /// Get all events
        /// </summary>
        [HttpGet]
        [Authorize(Policy = Policies.SecretaryOrHigher)]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            try
            {
                var events = await _eventService.GetAllEventsAsync();
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving events");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get event by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = Policies.RenterOrHigher)]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            try
            {
                var evt = await _eventService.GetEventByIdAsync(id);
                if (evt == null)
                    return NotFound();

                // Renters can only view their own events unless they are Secretary or Admin
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var isSecretary = User.IsInRole(Roles.Secretary) || User.IsInRole(Roles.Administrator);
                
                if (!isSecretary && evt.CreatedById != userId)
                    return Forbid();

                return Ok(evt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving event {EventId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get events by date range
        /// </summary>
        [HttpGet("date-range")]
        [Authorize(Policy = Policies.RenterOrHigher)]
        public async Task<ActionResult<IEnumerable<Event>>> GetEventsByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                var events = await _eventService.GetEventsByDateRangeAsync(startDate, endDate);
                
                // Renters see only their own events, Secretary/Admin see all
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var isSecretary = User.IsInRole(Roles.Secretary) || User.IsInRole(Roles.Administrator);
                
                if (!isSecretary && userId != null)
                {
                    events = events.Where(e => e.CreatedById == userId);
                }

                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving events by date range");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get events by room
        /// </summary>
        [HttpGet("room/{roomId}")]
        [Authorize(Policy = Policies.RenterOrHigher)]
        public async Task<ActionResult<IEnumerable<Event>>> GetEventsByRoom(int roomId)
        {
            try
            {
                var events = await _eventService.GetEventsByRoomAsync(roomId);
                
                // Renters see only approved events + their own, Secretary/Admin see all
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var isSecretary = User.IsInRole(Roles.Secretary) || User.IsInRole(Roles.Administrator);
                
                if (!isSecretary && userId != null)
                {
                    events = events.Where(e => e.Status == EventStatus.Approved || e.CreatedById == userId);
                }

                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving events for room {RoomId}", roomId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get pending events (Secretary/Admin only)
        /// </summary>
        [HttpGet("pending")]
        [Authorize(Policy = Policies.SecretaryOrHigher)]
        public async Task<ActionResult<IEnumerable<Event>>> GetPendingEvents()
        {
            try
            {
                var events = await _eventService.GetPendingEventsAsync();
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending events");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get user's own events
        /// </summary>
        [HttpGet("my-events")]
        [Authorize(Policy = Policies.RenterOrHigher)]
        public async Task<ActionResult<IEnumerable<Event>>> GetMyEvents()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                    return Unauthorized();

                var events = await _eventService.GetUserEventsAsync(userId);
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user events");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Check room availability
        /// </summary>
        [HttpGet("check-availability")]
        [Authorize(Policy = Policies.RenterOrHigher)]
        public async Task<ActionResult<object>> CheckRoomAvailability(
            [FromQuery] int roomId,
            [FromQuery] DateTime startTime,
            [FromQuery] DateTime endTime,
            [FromQuery] int? excludeEventId = null)
        {
            try
            {
                var isAvailable = await _eventService.IsRoomAvailableAsync(roomId, startTime, endTime, excludeEventId);
                var conflicts = isAvailable ? new List<Event>() : await _eventService.GetConflictingEventsAsync(roomId, startTime, endTime, excludeEventId);

                return Ok(new { isAvailable, conflicts });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking room availability");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create event (reservation request)
        /// </summary>
        [HttpPost]
        [Authorize(Policy = Policies.CanCreateReservations)]
        public async Task<ActionResult<Event>> CreateEvent([FromBody] Event evt)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Set the creator
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                    return Unauthorized();

                evt.CreatedById = userId;

                // Check for conflicts
                var isAvailable = await _eventService.IsRoomAvailableAsync(evt.RoomId, evt.StartDateTime, evt.EndDateTime);
                if (!isAvailable)
                {
                    var conflicts = await _eventService.GetConflictingEventsAsync(evt.RoomId, evt.StartDateTime, evt.EndDateTime);
                    return Conflict(new { message = "Room is not available for the specified time", conflicts });
                }

                var createdEvent = await _eventService.CreateEventAsync(evt);

                // Audit log
                await _auditLogService.LogAsync(
                    userId,
                    "Create",
                    "Event",
                    createdEvent.Id.ToString(),
                    $"Created event: {createdEvent.Title}",
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString()
                );

                return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.Id }, createdEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update event
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = Policies.RenterOrHigher)]
        public async Task<ActionResult<Event>> UpdateEvent(int id, [FromBody] Event evt)
        {
            try
            {
                if (id != evt.Id)
                    return BadRequest("ID mismatch");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check ownership or role
                var existingEvent = await _eventService.GetEventByIdAsync(id);
                if (existingEvent == null)
                    return NotFound();

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var isSecretary = User.IsInRole(Roles.Secretary) || User.IsInRole(Roles.Administrator);
                
                if (!isSecretary && existingEvent.CreatedById != userId)
                    return Forbid();

                // Check for conflicts if room or time changed
                if (existingEvent.RoomId != evt.RoomId || 
                    existingEvent.StartDateTime != evt.StartDateTime || 
                    existingEvent.EndDateTime != evt.EndDateTime)
                {
                    var isAvailable = await _eventService.IsRoomAvailableAsync(evt.RoomId, evt.StartDateTime, evt.EndDateTime, evt.Id);
                    if (!isAvailable)
                    {
                        var conflicts = await _eventService.GetConflictingEventsAsync(evt.RoomId, evt.StartDateTime, evt.EndDateTime, evt.Id);
                        return Conflict(new { message = "Room is not available for the specified time", conflicts });
                    }
                }

                var updatedEvent = await _eventService.UpdateEventAsync(evt);

                // Audit log
                if (userId != null)
                {
                    await _auditLogService.LogAsync(
                        userId,
                        "Update",
                        "Event",
                        updatedEvent.Id.ToString(),
                        $"Updated event: {updatedEvent.Title}",
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );
                }

                return Ok(updatedEvent);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Event not found: {EventId}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event {EventId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Approve event (Secretary/Admin only)
        /// </summary>
        [HttpPost("{id}/approve")]
        [Authorize(Policy = Policies.CanApproveEvents)]
        public async Task<ActionResult> ApproveEvent(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                    return Unauthorized();

                var success = await _eventService.ApproveEventAsync(id, userId);
                if (!success)
                    return NotFound();

                // Audit log
                await _auditLogService.LogAsync(
                    userId,
                    "Approve",
                    "Event",
                    id.ToString(),
                    $"Approved event ID: {id}",
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString()
                );

                return Ok(new { message = "Event approved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving event {EventId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Reject event (Secretary/Admin only)
        /// </summary>
        [HttpPost("{id}/reject")]
        [Authorize(Policy = Policies.CanApproveEvents)]
        public async Task<ActionResult> RejectEvent(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                    return Unauthorized();

                var success = await _eventService.RejectEventAsync(id, userId);
                if (!success)
                    return NotFound();

                // Audit log
                await _auditLogService.LogAsync(
                    userId,
                    "Reject",
                    "Event",
                    id.ToString(),
                    $"Rejected event ID: {id}",
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString()
                );

                return Ok(new { message = "Event rejected successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting event {EventId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Cancel event
        /// </summary>
        [HttpPost("{id}/cancel")]
        [Authorize(Policy = Policies.RenterOrHigher)]
        public async Task<ActionResult> CancelEvent(int id)
        {
            try
            {
                // Check ownership or role
                var existingEvent = await _eventService.GetEventByIdAsync(id);
                if (existingEvent == null)
                    return NotFound();

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var isSecretary = User.IsInRole(Roles.Secretary) || User.IsInRole(Roles.Administrator);
                
                if (!isSecretary && existingEvent.CreatedById != userId)
                    return Forbid();

                var success = await _eventService.CancelEventAsync(id);
                if (!success)
                    return NotFound();

                // Audit log
                if (userId != null)
                {
                    await _auditLogService.LogAsync(
                        userId,
                        "Cancel",
                        "Event",
                        id.ToString(),
                        $"Cancelled event ID: {id}",
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );
                }

                return Ok(new { message = "Event cancelled successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling event {EventId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete event (Admin/Secretary only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.CanManageEvents)]
        public async Task<ActionResult> DeleteEvent(int id)
        {
            try
            {
                var success = await _eventService.DeleteEventAsync(id);
                if (!success)
                    return NotFound();

                // Audit log
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId != null)
                {
                    await _auditLogService.LogAsync(
                        userId,
                        "Delete",
                        "Event",
                        id.ToString(),
                        $"Deleted event ID: {id}",
                        HttpContext.Connection.RemoteIpAddress?.ToString(),
                        Request.Headers["User-Agent"].ToString()
                    );
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting event {EventId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
