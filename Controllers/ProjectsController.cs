using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using PmsApi.DataContexts;
using PmsApi.DTOs;
using PmsApi.Models;
using PmsApi.Services;
using PmsApi.Utilities;

namespace PmsApi.Controllers;

[ApiController]                 //needed to define the controller
[Route("api/projects"), Authorize]

public class ProjectsController : ControllerBase
{
    private readonly IUserContextHelper _userContextHelper;
    private readonly IProjectService _projectService;
    private readonly PmsapiContext _context;
    private readonly IMapper _mapper;

    public ProjectsController(PmsapiContext context, IMapper mapper, IUserContextHelper userContextHelper, IProjectService projectService)
    {
        _context = context;
        _mapper = mapper;
        _userContextHelper = userContextHelper;
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectWithTaskDto>>> GetProjects([FromQuery] string include = "")
    {


        var projectsQuery = QueryHelper.ApplyProjectIncludes(_context.Projects.AsQueryable(), include);

        if (!_userContextHelper.IsAdmin())
        {
            projectsQuery.Where(p => p.UsersManagerId == _userContextHelper.GetUserId());
        }

        var projects = await projectsQuery.ToListAsync();
        var projectsDto = _mapper.Map<IEnumerable<ProjectWithTaskDto>>(projects);

        return Ok(projectsDto);
    }

    [HttpGet("{projectId}/tasks")]
    public async Task<ActionResult<ProjectWithTaskDto?>> GetProjectTasks(int projectId)
    {
        var userId = _userContextHelper.GetUserId();
        var isAdmin = _userContextHelper.IsAdmin();

        var project = await _projectService.GetProjectTasksAsync(projectId, userId, isAdmin);
        if (project == null)
        {
            return NotFound();

        }
        if (project.Tasks == null || project.Tasks.Count == 0)
        {
            return NotFound($"There are currently no tasks assigned to the project with id {projectId}.");
        }
        var projectTasks = project.Tasks.ToList();
        return Ok();
    }

    [HttpGet("{projectId:int}")]
    public async Task<ActionResult<ProjectWithTaskDto>> GetProject([FromRoute] int projectId, [FromQuery] string include = "")
    {
        var projectsQuery = QueryHelper.ApplyProjectIncludes(_context.Projects.AsQueryable(), include);

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

        if (!_userContextHelper.IsAdmin())
        {
            projectDto.UsersManagerId = _userContextHelper.GetUserId();
        }

        var project = _mapper.Map<Project>(projectDto);

        _context.Projects.Add(project);
        try
        {
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

    [HttpPut("{projectId:int}")]
    public async Task<ActionResult> UpdateProject([FromRoute] int projectId, [FromBody] CreateProjectDto projectDto) //[FromBody] is not necessary for POST and PUT calls, since it is implicitly understood by Entity
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Project? project = await _context.Projects.FindAsync(projectId);

        if (project is null)
        {
            return NotFound($"The project with the ID {projectId} could not be found.");
        }
        if (!_userContextHelper.IsAdmin() && project.UsersManagerId != _userContextHelper.GetUserId())
        {
            return Unauthorized();
        }

        _mapper.Map(projectDto, project);

        // project.ProjectName = projectDto.ProjectName;
        // project.Description = projectDto.Description;
        // project.UsersManagerId = projectDto.UsersManagerId;
        // project.ProjectCategoriesId = projectDto.ProjectCategoriesId;
        // project.StatusId = projectDto.StatusId;
        // project.PriorityId = projectDto.PriorityId;
        // project.StartDate = projectDto.StartDate;
        // project.EndDate = projectDto.EndDate;

        try
        {
            await _context.SaveChangesAsync();
            // api/users/{newId} ---> CreatedAtAction() returns the new URL for the resource
            return Ok();
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

    [HttpDelete]
    public async Task<ActionResult> DeleteProject(int projectId)
    {
        Project? project = await _context.Projects.FindAsync(projectId);

        if (project == null)
        {
            return NotFound($"No project found with ID {projectId}.");
        }
        if (!_userContextHelper.IsAdmin() && project.UsersManagerId != _userContextHelper.GetUserId())
        {
            return Unauthorized();
        }
        try
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex)
        when (ex.InnerException is MySqlException)
        {
            return BadRequest("The project has other dependencies. Please delete those first.");
        }
        catch (Exception)
        {
            return StatusCode(500, "An internal error has occoured.");
        }
    }
}