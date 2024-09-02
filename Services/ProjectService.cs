using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PmsApi.DataContexts;
using PmsApi.DTOs;
using PmsApi.Models;
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

    public async Task<IEnumerable<ProjectWithTaskDto>?> GetProjectsAsync(string userId, bool IsAdmin, string include = "")
    {
        var projectsQuery = QueryHelper.ApplyProjectIncludes(_context.Projects.AsQueryable(), include);

        if (!_userContextHelper.IsAdmin())
        {
            projectsQuery = projectsQuery.Where(p => p.UsersManagerId == userId);
        }

        var projects = await projectsQuery.ToListAsync();
        var projectsDto = _mapper.Map<IEnumerable<ProjectWithTaskDto>>(projects);

        return projectsDto;
    }

    public async Task<ProjectWithTaskDto?> GetProjectAsync(int projectId, string userId, bool isAdmin, string include = "")
    {
        var projectsQuery = QueryHelper.ApplyProjectIncludes(_context.Projects.AsQueryable(), include);
        if (!_userContextHelper.IsAdmin())
        {
            projectsQuery = projectsQuery.Where(p => p.UsersManagerId == userId);
        }

        Project? project = await projectsQuery.FirstOrDefaultAsync(p => p.ProjectId == projectId);

        if (project is null)
        {
            return null;
        }

        var projectDto = _mapper.Map<ProjectWithTaskDto>(project);

        return projectDto;
    }

    public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto projectDto, string userId, bool isAdmin)
    {
        if (!isAdmin)
        {
            projectDto.UsersManagerId = userId;
        }

        var project = _mapper.Map<Project>(projectDto);

        _context.Projects.Add(project);

        await _context.SaveChangesAsync();
        var newProjectDto = _mapper.Map<ProjectDto>(project);
        return newProjectDto;
    }

    public async Task<bool?> UpdateProjectAsync(int projectId, CreateProjectDto projectDto, string userId, bool isAdmin)
    {
        Project? project = await _context.Projects.FindAsync(projectId);

        if (project is null || (!isAdmin && project.UsersManagerId != userId))
        {
            return null;
        }

        _mapper.Map(projectDto, project);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool?> DeleteProjectAsync(int projectId, string userId, bool isAdmin)
    {
        Project? project = await _context.Projects.FindAsync(projectId);

        if (project is null || (!isAdmin && project.UsersManagerId != userId))
        {
            return null;
        }

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return true;
    }
}