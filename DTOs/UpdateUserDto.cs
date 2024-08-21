using System.ComponentModel.DataAnnotations;

namespace PmsApi.DTOs;

public class UpdateUserDto
{
    [Required(ErrorMessage = "Username is required.")]
    public string UserName { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string Lastname { get; set; } = "";
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    public string Email { get; set; } = "";
    [Required]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Please specify your role.")]
    [Range(1, 4, ErrorMessage = "Value must be between 1 and 4.")]
    public int RoleId { get; set; }
}