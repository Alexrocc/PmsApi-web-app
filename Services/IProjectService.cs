using PmsApi.DTOs;

namespace PmsApi.Services;

public interface IProjectService
{
    Task<ProjectWithTaskDto?> GetProjectTasksAsync(int ProjectId, string UserId, bool IsAdmin);
}