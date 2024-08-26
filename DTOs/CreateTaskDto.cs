using System.ComponentModel.DataAnnotations;

namespace PmsApi.DTOs;

public class CreateTaskDto
{

    [Required(ErrorMessage = "Task title is required.")]
    public string Title { get; set; } = "";

    public string? Description { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [Range(1, 4, ErrorMessage = "Value must be between 1 and 4.")]
    public int StatusId { get; set; }

    [Required(ErrorMessage = "Priority is required.")]
    [Range(1, 4, ErrorMessage = "Value must be between 1 and 4.")]
    public int PriorityId { get; set; }

    [Required(ErrorMessage = "Due Date needs to be specified.")]
    public DateOnly DueDate { get; set; }

    [Required(ErrorMessage = "Project Id is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Value cannot be 0.")]
    public int ProjectsId { get; set; }

    [Required(ErrorMessage = "Assigned user is required.")]
    public required UserDto User { get; set; }
}