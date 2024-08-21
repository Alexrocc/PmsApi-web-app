namespace PmsApi.DTOs;

public record UserDto
(
    int UserId,

    string UserName,

    string FirstName,

    string Lastname,

    string Email,

    string Password,

    int RoleId,

    List<ProjectDto> Projects,

    List<TaskDto> Tasks
);