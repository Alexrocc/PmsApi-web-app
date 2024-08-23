using System.ComponentModel.DataAnnotations;

namespace PmsApi.DTOs;

public record CreateProjectDto
(
    [Required(ErrorMessage = "Project name is required.")]
    string ProjectName,

    string? Description,

    [Required(ErrorMessage = "Category is required.")]
    [Range(1, 4, ErrorMessage = "Project category id must be between 1 and 4.")]
    int ProjectCategoriesId,

    [Required(ErrorMessage = "Manager assignment is required.")]
    int UsersManagerId,

    [Required(ErrorMessage = "Status is required.")]
    [Range(1, 4, ErrorMessage = "Status Id must be between 1 and 4.")]
    int StatusId,

    [Required(ErrorMessage = "Priority is required.")]
    [Range(1, 4, ErrorMessage = "Priority Id must be between 1 (low) and 4 (maximum).")]
    int PriorityId,

    [Required(ErrorMessage = "Start date is required.")]
    DateOnly StartDate,

    [Required(ErrorMessage = "End date is required.")]
    DateOnly EndDate
);