namespace PmsApi.Models;

public class Status
{
    public int StatusId { get; set; }
    public string StatusName { get; set; } = "";
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<Models.Task> Tasks { get; set; } = new List<Models.Task>();
}