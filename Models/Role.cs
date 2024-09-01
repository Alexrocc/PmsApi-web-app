using Microsoft.AspNetCore.Identity;

namespace PmsApi.Models;

public class Role : IdentityRole
{
    public ICollection<User> Users { get; set; } = new List<User>();
}