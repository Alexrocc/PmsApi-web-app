namespace PmsApi.DTOs;

public record UserDto
(
    string Id,

    string UserName,

    string FirstName,

    string Lastname,

    string Email,

    string Password,

    string PhoneNumber,

    List<ProjectDto> Projects,

    List<TaskDto> Tasks
);