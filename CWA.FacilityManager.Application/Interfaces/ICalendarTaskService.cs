using CWA.FacilityManager.Shared.DTOs;

namespace CWA.FacilityManager.Application.Interfaces
{
    public interface ICalendarTaskService
    {
        Task<IEnumerable<CalendarTaskDto>> GetAllTasksAsync();
        Task<IEnumerable<CalendarTaskDto>> GetTasksByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<CalendarTaskDto>> GetTasksByUserAsync(string userId);
        Task<CalendarTaskDto?> GetTaskByIdAsync(int id);
        Task<CalendarTaskDto> CreateTaskAsync(CreateCalendarTaskDto createTaskDto);
        Task<CalendarTaskDto?> UpdateTaskAsync(int id, UpdateCalendarTaskDto updateTaskDto);
        Task<bool> DeleteTaskAsync(int id);
        Task<IEnumerable<CalendarTaskDto>> GetTasksByCategoryAsync(TaskCategoryDto category);
        Task<IEnumerable<CalendarTaskDto>> GetTasksByStatusAsync(TaskStatusDto status);
        Task<bool> UpdateTaskStatusAsync(int id, TaskStatusDto status);
        Task<IEnumerable<CalendarTaskDto>> SearchTasksAsync(string searchTerm);
    }
}