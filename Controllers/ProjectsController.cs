using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
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

    [HttpPost]
    public async Task<ActionResult> CreateProject(CreateProjectDto projectDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var project = _mapper.Map<Project>(projectDto);

        try
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            var newProjectDto = _mapper.Map<ProjectDto>(project);
            // api/projects/{newId} ---> CreatedAtAction() returns the new URL for the resource
            return CreatedAtAction(nameof(GetProject), new { projectId = project.ProjectId }, newProjectDto);
        }
        catch (DbUpdateException ex)
        when (ex.InnerException is MySqlException mySqlException
        && mySqlException.Number == 1062)
        {
            return BadRequest("Project name already taken.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

}