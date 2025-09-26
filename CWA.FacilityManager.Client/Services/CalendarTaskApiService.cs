using CWA.FacilityManager.Shared.DTOs;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CWA.FacilityManager.Client.Services
{
    public interface ICalendarTaskApiService
    {
        Task<IEnumerable<CalendarTaskDto>?> GetAllTasksAsync();
        Task<IEnumerable<CalendarTaskDto>?> GetTasksByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<CalendarTaskDto>?> GetTasksByUserAsync(string userId);
        Task<CalendarTaskDto?> GetTaskByIdAsync(int id);
        Task<CalendarTaskDto?> CreateTaskAsync(CreateCalendarTaskDto createTaskDto);
        Task<CalendarTaskDto?> UpdateTaskAsync(int id, UpdateCalendarTaskDto updateTaskDto);
        Task<bool> DeleteTaskAsync(int id);
        Task<IEnumerable<CalendarTaskDto>?> GetTasksByCategoryAsync(TaskCategoryDto category);
        Task<IEnumerable<CalendarTaskDto>?> GetTasksByStatusAsync(TaskStatusDto status);
        Task<bool> UpdateTaskStatusAsync(int id, TaskStatusDto status);
        Task<IEnumerable<CalendarTaskDto>?> SearchTasksAsync(string searchTerm);
    }

    public class CalendarTaskApiService : ICalendarTaskApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string BaseUrl = "api/calendartask";

        public CalendarTaskApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            _jsonOptions.Converters.Add(new JsonStringEnumConverter());
            // Temporarily remove custom DateTime converter to test
            // _jsonOptions.Converters.Add(new CalendarDateTimeConverter());
        }

        public async Task<IEnumerable<CalendarTaskDto>?> GetAllTasksAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<CalendarTaskDto>>(BaseUrl, _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllTasksAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<CalendarTaskDto>?> GetTasksByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var url = $"{BaseUrl}/date-range?startDate={startDate:yyyy-MM-ddTHH:mm:ss}&endDate={endDate:yyyy-MM-ddTHH:mm:ss}";
                return await _httpClient.GetFromJsonAsync<IEnumerable<CalendarTaskDto>>(url, _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTasksByDateRangeAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<CalendarTaskDto>?> GetTasksByUserAsync(string userId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<CalendarTaskDto>>($"{BaseUrl}/user/{userId}", _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTasksByUserAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<CalendarTaskDto?> GetTaskByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<CalendarTaskDto>($"{BaseUrl}/{id}", _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTaskByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<CalendarTaskDto?> CreateTaskAsync(CreateCalendarTaskDto createTaskDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, createTaskDto, _jsonOptions);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CalendarTaskDto>(_jsonOptions);
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateTaskAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<CalendarTaskDto?> UpdateTaskAsync(int id, UpdateCalendarTaskDto updateTaskDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", updateTaskDto, _jsonOptions);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CalendarTaskDto>(_jsonOptions);
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateTaskAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteTaskAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<CalendarTaskDto>?> GetTasksByCategoryAsync(TaskCategoryDto category)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<CalendarTaskDto>>($"{BaseUrl}/category/{category}", _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTasksByCategoryAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<CalendarTaskDto>?> GetTasksByStatusAsync(TaskStatusDto status)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<CalendarTaskDto>>($"{BaseUrl}/status/{status}", _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTasksByStatusAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateTaskStatusAsync(int id, TaskStatusDto status)
        {
            try
            {
                var response = await _httpClient.PatchAsJsonAsync($"{BaseUrl}/{id}/status", status, _jsonOptions);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateTaskStatusAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<CalendarTaskDto>?> SearchTasksAsync(string searchTerm)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<CalendarTaskDto>>($"{BaseUrl}/search?searchTerm={Uri.EscapeDataString(searchTerm)}", _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SearchTasksAsync: {ex.Message}");
                return null;
            }
        }
    }
}