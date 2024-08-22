using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PmsApi.DataContexts;
using PmsApi.DTOs;

namespace PmsApi.Controllers;

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
}