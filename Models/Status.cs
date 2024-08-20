namespace PmsApi.Models;

public class Status
{
    public int StatusId { get; set; }
    public string StatusName { get; set; } = "";
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    public virtual ICollection<Models.Task> Tasks { get; set; } = new List<Models.Task>();
}