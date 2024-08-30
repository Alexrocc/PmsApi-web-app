using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PmsApi.DataContexts;
using PmsApi.DTOs;
using PmsApi.Utilities;

namespace PmsApi.Services;

public class ProjectService : IProjectService
{
    private readonly IUserContextHelper _userContextHelper;
    private readonly PmsapiContext _context;
    private readonly IMapper _mapper;

    public ProjectService(IUserContextHelper userContextHelper, PmsapiContext context, IMapper mapper)
    {
        _userContextHelper = userContextHelper;
        _context = context;
        _mapper = mapper;
    }
    public async Task<ProjectWithTaskDto?> GetProjectTasksAsync(int projectId, string userId, bool isAdmin)
    {
        var projectsQuery = _context.Projects.AsQueryable();

        if (!isAdmin)
        {
            projectsQuery = projectsQuery.Where(p => p.UsersManagerId == userId);
        }

        var project = await projectsQuery.Include(p => p.Tasks).FirstOrDefaultAsync(p => p.ProjectId == projectId);

        if (project == null)
        {
            return null;
        }

        var projectDto = _mapper.Map<ProjectWithTaskDto>(project);

        return projectDto;
    }
}