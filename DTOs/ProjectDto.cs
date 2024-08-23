namespace PmsApi.DTOs;

public record ProjectDto
(
     int ProjectId,

     string ProjectName,

     string Description,

     DateOnly StartDate,

     DateOnly EndDate,

     int ProjectCategoriesId,

     int UsersManagerId
);