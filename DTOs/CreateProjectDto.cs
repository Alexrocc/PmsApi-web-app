using System.ComponentModel.DataAnnotations;

namespace PmsApi.DTOs;

public class CreateProjectDto
{
    [Required(ErrorMessage = "Project name is required.")]
    public required string ProjectName { get; set; }

    public string? Description { get; set; }

    [Required(ErrorMessage = "Category is required.")]
    [Range(1, 4, ErrorMessage = "Project category id must be between 1 and 4.")]
    public int ProjectCategoriesId { get; set; }

    [Required(ErrorMessage = "Manager assignment is required.")]
    public required string UsersManagerId { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [Range(1, 4, ErrorMessage = "Status Id must be between 1 and 4.")]
    public int StatusId { get; set; }

    [Required(ErrorMessage = "Priority is required.")]
    [Range(1, 4, ErrorMessage = "Priority Id must be between 1 (low) and 4 (maximum).")]
    public int PriorityId { get; set; }

    [Required(ErrorMessage = "Start date is required.")]
    public DateOnly StartDate { get; set; }

    [Required(ErrorMessage = "End date is required.")]
    public DateOnly EndDate { get; set; }
}