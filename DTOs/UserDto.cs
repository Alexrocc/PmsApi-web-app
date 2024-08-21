namespace PmsApi.DTOs;

public class UserDto
{
    public string UserName { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string Lastname { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public int RoleId { get; set; }
}