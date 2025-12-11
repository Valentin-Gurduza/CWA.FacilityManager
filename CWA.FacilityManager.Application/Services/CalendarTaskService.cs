using CWA.FacilityManager.Application.Interfaces;
using CWA.FacilityManager.Domain.Models;
using CWA.FacilityManager.Infrastructure.Contexts;
using CWA.FacilityManager.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using DomainTaskStatus = CWA.FacilityManager.Domain.Models.TaskStatus;

namespace CWA.FacilityManager.Application.Services
{
    public class CalendarTaskService : ICalendarTaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CalendarTaskService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        private bool IsAdminOrSecretary()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user != null && (user.IsInRole("Administrator") || user.IsInRole("Secretary"));
        }

        public async Task<IEnumerable<CalendarTaskDto>> GetAllTasksAsync()
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return new List<CalendarTaskDto>();
            }

            IQueryable<CalendarTask> query = _context.CalendarTasks.Include(t => t.AssignedUser);

            // Only admin and secretary can see all tasks, regular users only see their own
            if (!IsAdminOrSecretary())
            {
                query = query.Where(t => t.AssignedUserId == currentUserId);
            }

            var tasks = await query.OrderBy(t => t.StartDate).ToListAsync();
            return tasks.Select(MapToDto);
        }

        public async Task<IEnumerable<CalendarTaskDto>> GetTasksByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return new List<CalendarTaskDto>();
            }

            IQueryable<CalendarTask> query = _context.CalendarTasks
                .Include(t => t.AssignedUser)
                .Where(t => t.StartDate <= endDate && t.EndDate >= startDate);

            // Only admin and secretary can see all tasks, regular users only see their own
            if (!IsAdminOrSecretary())
            {
                query = query.Where(t => t.AssignedUserId == currentUserId);
            }

            var tasks = await query.OrderBy(t => t.StartDate).ToListAsync();
            return tasks.Select(MapToDto);
        }

        public async Task<IEnumerable<CalendarTaskDto>> GetTasksByUserAsync(string userId)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return new List<CalendarTaskDto>();
            }

            // Only admin/secretary can view other users' tasks, or users can view their own
            if (!IsAdminOrSecretary() && userId != currentUserId)
            {
                return new List<CalendarTaskDto>();
            }

            var tasks = await _context.CalendarTasks
                .Include(t => t.AssignedUser)
                .Where(t => t.AssignedUserId == userId)
                .OrderBy(t => t.StartDate)
                .ToListAsync();

            return tasks.Select(MapToDto);
        }

        public async Task<CalendarTaskDto?> GetTaskByIdAsync(int id)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return null;
            }

            IQueryable<CalendarTask> query = _context.CalendarTasks.Include(t => t.AssignedUser);

            // Only admin/secretary can see all tasks, regular users only see their own
            if (!IsAdminOrSecretary())
            {
                query = query.Where(t => t.AssignedUserId == currentUserId);
            }

            var task = await query.FirstOrDefaultAsync(t => t.Id == id);
            return task != null ? MapToDto(task) : null;
        }

        public async Task<CalendarTaskDto> CreateTaskAsync(CreateCalendarTaskDto createTaskDto)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            // Normalize DateTime handling to prevent timezone issues
            var startDate = NormalizeDateTimeForStorage(createTaskDto.StartDate, createTaskDto.IsAllDay);
            var endDate = NormalizeDateTimeForStorage(createTaskDto.EndDate, createTaskDto.IsAllDay);

            var task = new CalendarTask
            {
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                StartDate = startDate,
                EndDate = endDate,
                IsAllDay = createTaskDto.IsAllDay,
                Priority = (TaskPriority)createTaskDto.Priority,
                Color = createTaskDto.Color,
                // If no assigned user is specified or user is not admin/secretary, assign to current user
                AssignedUserId = (IsAdminOrSecretary() && !string.IsNullOrEmpty(createTaskDto.AssignedUserId)) 
                    ? createTaskDto.AssignedUserId 
                    : currentUserId,
                FacilityId = createTaskDto.FacilityId,
                Location = createTaskDto.Location,
                IsRecurring = createTaskDto.IsRecurring,
                RecurrenceType = createTaskDto.RecurrenceType.HasValue ? 
                    (RecurrenceType)createTaskDto.RecurrenceType.Value : null,
                RecurrenceInterval = createTaskDto.RecurrenceInterval,
                RecurrenceEndDate = createTaskDto.RecurrenceEndDate,
                Category = (TaskCategory)createTaskDto.Category,
                Tags = createTaskDto.Tags,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.CalendarTasks.Add(task);
            await _context.SaveChangesAsync();

            return await GetTaskByIdAsync(task.Id) ?? throw new InvalidOperationException("Failed to retrieve created task");
        }

        public async Task<CalendarTaskDto?> UpdateTaskAsync(int id, UpdateCalendarTaskDto updateTaskDto)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return null;
            }

            var task = await _context.CalendarTasks.FindAsync(id);
            if (task == null)
                return null;

            // Only admin/secretary can edit any task, regular users can only edit their own
            if (!IsAdminOrSecretary() && task.AssignedUserId != currentUserId)
            {
                return null;
            }

            // Normalize DateTime handling to prevent timezone issues
            var startDate = NormalizeDateTimeForStorage(updateTaskDto.StartDate, updateTaskDto.IsAllDay);
            var endDate = NormalizeDateTimeForStorage(updateTaskDto.EndDate, updateTaskDto.IsAllDay);

            task.Title = updateTaskDto.Title;
            task.Description = updateTaskDto.Description;
            task.StartDate = startDate;
            task.EndDate = endDate;
            task.IsAllDay = updateTaskDto.IsAllDay;
            task.Priority = (TaskPriority)updateTaskDto.Priority;
            task.Status = (DomainTaskStatus)updateTaskDto.Status;
            task.Color = updateTaskDto.Color;
            
            // Only admin/secretary can reassign tasks
            if (IsAdminOrSecretary() && !string.IsNullOrEmpty(updateTaskDto.AssignedUserId))
            {
                task.AssignedUserId = updateTaskDto.AssignedUserId;
            }
            
            task.FacilityId = updateTaskDto.FacilityId;
            task.Location = updateTaskDto.Location;
            task.IsRecurring = updateTaskDto.IsRecurring;
            task.RecurrenceType = updateTaskDto.RecurrenceType.HasValue ? 
                (RecurrenceType)updateTaskDto.RecurrenceType.Value : null;
            task.RecurrenceInterval = updateTaskDto.RecurrenceInterval;
            task.RecurrenceEndDate = updateTaskDto.RecurrenceEndDate;
            task.Category = (TaskCategory)updateTaskDto.Category;
            task.Tags = updateTaskDto.Tags;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetTaskByIdAsync(id);
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return false;
            }

            var task = await _context.CalendarTasks.FindAsync(id);
            if (task == null)
                return false;

            // Only admin/secretary can delete any task, regular users can only delete their own
            if (!IsAdminOrSecretary() && task.AssignedUserId != currentUserId)
            {
                return false;
            }

            _context.CalendarTasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CalendarTaskDto>> GetTasksByCategoryAsync(TaskCategoryDto category)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return new List<CalendarTaskDto>();
            }

            IQueryable<CalendarTask> query = _context.CalendarTasks
                .Include(t => t.AssignedUser)
                .Where(t => t.Category == (TaskCategory)category);

            // Only admin and secretary can see all tasks, regular users only see their own
            if (!IsAdminOrSecretary())
            {
                query = query.Where(t => t.AssignedUserId == currentUserId);
            }

            var tasks = await query.OrderBy(t => t.StartDate).ToListAsync();
            return tasks.Select(MapToDto);
        }

        public async Task<IEnumerable<CalendarTaskDto>> GetTasksByStatusAsync(TaskStatusDto status)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return new List<CalendarTaskDto>();
            }

            IQueryable<CalendarTask> query = _context.CalendarTasks
                .Include(t => t.AssignedUser)
                .Where(t => t.Status == (DomainTaskStatus)status);

            // Only admin and secretary can see all tasks, regular users only see their own
            if (!IsAdminOrSecretary())
            {
                query = query.Where(t => t.AssignedUserId == currentUserId);
            }

            var tasks = await query.OrderBy(t => t.StartDate).ToListAsync();
            return tasks.Select(MapToDto);
        }

        public async Task<bool> UpdateTaskStatusAsync(int id, TaskStatusDto status)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return false;
            }

            var task = await _context.CalendarTasks.FindAsync(id);
            if (task == null)
                return false;

            // Only admin/secretary can update any task, regular users can only update their own
            if (!IsAdminOrSecretary() && task.AssignedUserId != currentUserId)
            {
                return false;
            }

            task.Status = (DomainTaskStatus)status;
            task.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CalendarTaskDto>> SearchTasksAsync(string searchTerm)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return new List<CalendarTaskDto>();
            }

            IQueryable<CalendarTask> query = _context.CalendarTasks
                .Include(t => t.AssignedUser)
                .Where(t => t.Title.Contains(searchTerm) || 
                           (t.Description != null && t.Description.Contains(searchTerm)) ||
                           (t.Tags != null && t.Tags.Contains(searchTerm)));

            // Only admin and secretary can see all tasks, regular users only see their own
            if (!IsAdminOrSecretary())
            {
                query = query.Where(t => t.AssignedUserId == currentUserId);
            }

            var tasks = await query.OrderBy(t => t.StartDate).ToListAsync();
            return tasks.Select(MapToDto);
        }

        private static DateTime NormalizeDateTimeForStorage(DateTime dateTime, bool isAllDay)
        {
            if (isAllDay)
            {
                // For all-day events, store as unspecified kind to avoid timezone conversion
                return DateTime.SpecifyKind(dateTime.Date, DateTimeKind.Unspecified);
            }
            else
            {
                // For timed events, ensure we have a proper kind
                return dateTime.Kind == DateTimeKind.Unspecified 
                    ? DateTime.SpecifyKind(dateTime, DateTimeKind.Local)
                    : dateTime;
            }
        }

        private static CalendarTaskDto MapToDto(CalendarTask task)
        {
            return new CalendarTaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                IsAllDay = task.IsAllDay,
                Priority = (TaskPriorityDto)task.Priority,
                Status = (TaskStatusDto)task.Status,
                Color = task.Color,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                AssignedUserId = task.AssignedUserId,
                AssignedUserName = task.AssignedUser?.UserName,
                FacilityId = task.FacilityId,
                Location = task.Location,
                IsRecurring = task.IsRecurring,
                RecurrenceType = task.RecurrenceType.HasValue ? 
                    (RecurrenceTypeDto)task.RecurrenceType.Value : null,
                RecurrenceInterval = task.RecurrenceInterval,
                RecurrenceEndDate = task.RecurrenceEndDate,
                Category = (TaskCategoryDto)task.Category,
                Tags = task.Tags
            };
        }
    }
}