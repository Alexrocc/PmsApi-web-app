namespace PmsApi.DTOs;

public record TaskAllDto(
     int TaskId,

     string Title,

     string Description,

     int StatusId,

     int PriorityId,

     DateOnly DueDate,

     ProjectDto Project,

     SimpleUserDto AssignedUser,

     IEnumerable<TaskAttachmentDto> TaskAttachments
);