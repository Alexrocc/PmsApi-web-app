using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PmsApi.DataContexts;
using PmsApi.DTOs;
using PmsApi.Utilities;

namespace PmsApi.Services;

public class ProjectService : IProjectService
{
    private readonly PmsapiContext _context;
    private readonly IMapper _mapper;
    private readonly IUserContextHelper _userContextHelper;

    public ProjectService(PmsapiContext context, IMapper mapper, IUserContextHelper userContextHelper)
    {
        _context = context;
        _mapper = mapper;
        _userContextHelper = userContextHelper;
    }
    public async Task<ProjectWithTaskDto?> GetProjectTasksAsync(int projectId, string userId, bool isAdmin)
    {
        var projectsQuery = _context.Projects.AsQueryable();

        if (!isAdmin)
        {
            projectsQuery = projectsQuery.Where(p => p.UsersManagerId == userId);
        }

        var project = await projectsQuery.Include(p => p.Tasks)
        .FirstOrDefaultAsync(p => p.ProjectId == projectId);

        if (project == null)
        {
            return null;
        }

        var projectDto = _mapper.Map<ProjectWithTaskDto>(project);

        return projectDto;
    }
}