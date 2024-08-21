namespace PmsApi.Models;

public partial class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Password { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int RoleId { get; set; }
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public Role? Role { get; set; }
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}