using PmsApi.DTOs;

namespace PmsApi.Services;

public interface IProjectService
{
    Task<ProjectWithTaskDto?> GetProjectTasksAsync(int ProjectId, string userId, bool IsAdmin);

    Task<IEnumerable<ProjectWithTaskDto>?> GetProjectsAsync(string userId, bool IsAdmin, string include = "");

    Task<ProjectWithTaskDto?> GetProjectAsync(int projectId, string userId, bool isAdmin, string include = "");

    Task<ProjectDto> CreateProjectAsync(CreateProjectDto projectDto, string userId, bool isAdmin);

    Task<bool?> UpdateProjectAsync(int projectId, CreateProjectDto projectDto, string userId, bool isAdmin);

    Task<bool?> DeleteProjectAsync(int projectId, string userId, bool isAdmin);
}