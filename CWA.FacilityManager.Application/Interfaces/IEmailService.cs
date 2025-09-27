namespace CWA.FacilityManager.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEventNotificationAsync(string toEmail, string eventTitle, string eventType, DateTime startTime, DateTime endTime, string roomLocation, string? organizer = null);
        Task SendEventUpdateNotificationAsync(string toEmail, string eventTitle, string changes, DateTime startTime, DateTime endTime, string roomLocation);
        Task SendEventReminderAsync(string toEmail, string eventTitle, DateTime startTime, string roomLocation, string? organizer = null);
        Task SendEventCancellationAsync(string toEmail, string eventTitle, DateTime startTime, string roomLocation);
    }
}