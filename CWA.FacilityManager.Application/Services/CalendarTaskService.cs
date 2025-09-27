using CWA.FacilityManager.Application.Interfaces;
using CWA.FacilityManager.Domain.Models;
using CWA.FacilityManager.Infrastructure.Contexts;
using CWA.FacilityManager.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using DomainTaskStatus = CWA.FacilityManager.Domain.Models.TaskStatus;

namespace CWA.FacilityManager.Application.Services
{
    public class CalendarTaskService : ICalendarTaskService
    {
        private readonly ApplicationDbContext _context;

        public CalendarTaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CalendarTaskDto>> GetAllTasksAsync()
        {
            var tasks = await _context.CalendarTasks
                .Include(t => t.AssignedUser)
                .OrderBy(t => t.StartDate)
                .ToListAsync();

            return tasks.Select(MapToDto);
        }

        public async Task<IEnumerable<CalendarTaskDto>> GetTasksByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var tasks = await _context.CalendarTasks
                .Include(t => t.AssignedUser)
                .Where(t => t.StartDate <= endDate && t.EndDate >= startDate)
                .OrderBy(t => t.StartDate)
                .ToListAsync();

            return tasks.Select(MapToDto);
        }

        public async Task<IEnumerable<CalendarTaskDto>> GetTasksByUserAsync(string userId)
        {
            var tasks = await _context.CalendarTasks
                .Include(t => t.AssignedUser)
                .Where(t => t.AssignedUserId == userId)
                .OrderBy(t => t.StartDate)
                .ToListAsync();

            return tasks.Select(MapToDto);
        }

        public async Task<CalendarTaskDto?> GetTaskByIdAsync(int id)
        {
            var task = await _context.CalendarTasks
                .Include(t => t.AssignedUser)
                .FirstOrDefaultAsync(t => t.Id == id);

            return task != null ? MapToDto(task) : null;
        }

        public async Task<CalendarTaskDto> CreateTaskAsync(CreateCalendarTaskDto createTaskDto)
        {
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
                AssignedUserId = createTaskDto.AssignedUserId,
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
            var task = await _context.CalendarTasks.FindAsync(id);
            if (task == null)
                return null;

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
            task.AssignedUserId = updateTaskDto.AssignedUserId;
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
            var task = await _context.CalendarTasks.FindAsync(id);
            if (task == null)
                return false;

            _context.CalendarTasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CalendarTaskDto>> GetTasksByCategoryAsync(TaskCategoryDto category)
        {
            var tasks = await _context.CalendarTasks
                .Include(t => t.AssignedUser)
                .Where(t => t.Category == (TaskCategory)category)
                .OrderBy(t => t.StartDate)
                .ToListAsync();

            return tasks.Select(MapToDto);
        }

        public async Task<IEnumerable<CalendarTaskDto>> GetTasksByStatusAsync(TaskStatusDto status)
        {
            var tasks = await _context.CalendarTasks
                .Include(t => t.AssignedUser)
                .Where(t => t.Status == (DomainTaskStatus)status)
                .OrderBy(t => t.StartDate)
                .ToListAsync();

            return tasks.Select(MapToDto);
        }

        public async Task<bool> UpdateTaskStatusAsync(int id, TaskStatusDto status)
        {
            var task = await _context.CalendarTasks.FindAsync(id);
            if (task == null)
                return false;

            task.Status = (DomainTaskStatus)status;
            task.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CalendarTaskDto>> SearchTasksAsync(string searchTerm)
        {
            var tasks = await _context.CalendarTasks
                .Include(t => t.AssignedUser)
                .Where(t => t.Title.Contains(searchTerm) || 
                           (t.Description != null && t.Description.Contains(searchTerm)) ||
                           (t.Tags != null && t.Tags.Contains(searchTerm)))
                .OrderBy(t => t.StartDate)
                .ToListAsync();

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