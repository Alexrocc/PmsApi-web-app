using System.ComponentModel.DataAnnotations;

namespace PmsApi.DTOs;

public class CreateTaskDto
{

    [Required(ErrorMessage = "Task title is required.")]
    public string Title { get; set; } = "";

    public string? Description { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    public int StatusId { get; set; }

    [Required(ErrorMessage = "Priority is required.")]
    public int PriorityId { get; set; }

    [Required(ErrorMessage = "Due Date needs to be specified.")]
    public DateOnly DueDate { get; set; }

    [Required(ErrorMessage = "Project Id is required.")]
    public int ProjectsId { get; set; }

    [Required(ErrorMessage = "Assigned user is required.")]
    public required UserDto User { get; set; }
}