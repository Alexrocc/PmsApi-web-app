namespace PmsApi.Models;

public partial class Task
{
    public int TaskId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int StatusId { get; set; }
    public Status? Status { get; set; }
    public int PriorityId { get; set; }
    public Priority? Priority { get; set; }
    public DateOnly? DueDate { get; set; }
    public int? ProjectId { get; set; }
    public string? AssignedUserId { get; set; }
    public Project? Project { get; set; }
    public IEnumerable<TaskAttachment>? TaskAttachments { get; set; }
    public User? AssignedUser { get; set; }
}