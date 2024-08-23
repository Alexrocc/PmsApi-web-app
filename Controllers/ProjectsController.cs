using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PmsApi.DataContexts;
using PmsApi.DTOs;
using PmsApi.Models;

namespace PmsApi.Controllers;

[ApiController]                 //needed to define the controller
[Route("api/projects")]

public class ProjectsController : ControllerBase
{
    private readonly PmsapiContext _context;
    private readonly IMapper _mapper;

    public ProjectsController(PmsapiContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectWithTaskDto>>> GetProjects([FromQuery] string include = "")
    {
        var projectsQuery = _context.Projects.AsQueryable();      //dynamic query for including dependend data

        if (include.Contains("tasks", StringComparison.OrdinalIgnoreCase))
        {
            projectsQuery = projectsQuery.Include(p => p.Tasks);
        }
        if (include.Contains("manager", StringComparison.OrdinalIgnoreCase))
        {
            projectsQuery = projectsQuery.Include(p => p.UsersManager);
        }
        if (include.Contains("category", StringComparison.OrdinalIgnoreCase))
        {
            projectsQuery = projectsQuery.Include(p => p.ProjectCategory);
        }

        var projects = await projectsQuery.ToListAsync();
        var projectsDto = _mapper.Map<IEnumerable<ProjectWithTaskDto>>(projects);

        return Ok(projectsDto);
    }

    [HttpGet("{projectId:int}")]
    public async Task<ActionResult<ProjectWithTaskDto>> GetProject([FromRoute] int projectId, [FromQuery] string include = "")
    {
        var projectsQuery = _context.Projects.AsQueryable();
        if (include.Contains("tasks", StringComparison.OrdinalIgnoreCase))
        {
            projectsQuery = projectsQuery.Include(p => p.Tasks);
        }
        if (include.Contains("manager", StringComparison.OrdinalIgnoreCase))
        {
            projectsQuery = projectsQuery.Include(p => p.UsersManager);
        }
        if (include.Contains("category", StringComparison.OrdinalIgnoreCase))
        {
            projectsQuery = projectsQuery.Include(p => p.ProjectCategory);
        }
        Project? project = await projectsQuery.FirstOrDefaultAsync(p => p.ProjectId == projectId);
        if (project is null)
        {
            return NotFound();
        }
        var projectDto = _mapper.Map<ProjectWithTaskDto>(project);
        return Ok(projectDto);
    }
}