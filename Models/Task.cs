namespace PmsApi.Models;

public partial class Task
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int StatusId { get; set; }
    public virtual Status? Status { get; set; }
    public int PriorityId { get; set; }
    public virtual Priority? Priority { get; set; }
    public DateOnly? DueDate { get; set; }
    public int? ProjectsId { get; set; }
    public int? UsersId { get; set; }
    public virtual Project? Projects { get; set; }
    public virtual ICollection<TaskAttachment> TaskAttachments { get; set; } = new List<TaskAttachment>();
    public virtual User? Users { get; set; }
}