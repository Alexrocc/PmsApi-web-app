namespace PmsApi.DTOs;

public record TaskDto(

     int TaskId,

     string Title,

     string? Description,

     int StatusId,

     int PriorityId,

     DateOnly DueDate,

     int ProjectsId,

     string AssignedUserId,

     UserDto User
);