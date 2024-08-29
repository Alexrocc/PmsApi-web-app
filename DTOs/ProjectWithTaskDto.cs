namespace PmsApi.DTOs;

public record ProjectWithTaskDto(
     int ProjectId,

     string ProjectName,

     string Description,

     DateOnly StartDate,

     DateOnly EndDate,

     int ProjectCategoriesId,

     string UsersManagerId,

     ICollection<TaskDto>? Tasks,

     ManagerDto? UsersManager,

     CategoryDto? ProjectCategory
);