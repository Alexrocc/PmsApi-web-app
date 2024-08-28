using Microsoft.AspNetCore.Identity;

namespace PmsApi.Models;

public partial class User : IdentityUser
{
    public string? RoleId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}