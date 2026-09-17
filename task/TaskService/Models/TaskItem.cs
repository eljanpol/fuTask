namespace TaskService.Models;

public enum TaskStatus
{   
    New,
    Inprogres,
    Done
}
public class TaskItem
{
    public Guid Id { get; set; }
    public string Title {get;set;} = string.Empty;
    public string Description {get;set;} = string.Empty;

    public TaskStatus Status {get;set;} = TaskStatus.New;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // public Guid UserId{get;set;}
    // public User? User{get;set;}
}
