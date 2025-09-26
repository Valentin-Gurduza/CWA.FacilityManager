namespace CWA.FacilityManager.Shared.DTOs
{
    public enum TaskPriorityDto
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }
    
    public enum TaskStatusDto
    {
        Pending = 1,
        InProgress = 2,
        Completed = 3,
        Cancelled = 4,
        OnHold = 5
    }
    
    public enum RecurrenceTypeDto
    {
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Yearly = 4
    }
    
    public enum TaskCategoryDto
    {
        Meeting = 1,
        Training = 2,
        Course = 3
    }
}