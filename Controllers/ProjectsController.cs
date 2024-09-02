using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using PmsApi.DataContexts;
using PmsApi.DTOs;
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
        GetUserCredentials(out string userId, out bool isAdmin);

        var projectsDto = await _projectService.GetProjectsAsync(userId, isAdmin, include);

        return Ok(projectsDto);
    }

    [HttpGet("{projectId}/tasks")]
    public async Task<ActionResult<ProjectWithTaskDto>> GetProjectTasks(int projectId)
    {
        GetUserCredentials(out string userId, out bool isAdmin);

        var projectTasks = await _projectService.GetProjectTasksAsync(projectId, userId, isAdmin);
        if (projectTasks == null)
        {
            return NotFound();
        }
        return Ok(projectTasks);
    }

    [HttpGet("{projectId:int}")]
    public async Task<ActionResult<ProjectWithTaskDto>> GetProject([FromRoute] int projectId, [FromQuery] string include = "")
    {
        GetUserCredentials(out string userId, out bool isAdmin);

        var project = await _projectService.GetProjectAsync(projectId, userId, isAdmin, include);
        if (project is null)
        {
            return NotFound($"Project with ID {projectId} not found.");
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

        GetUserCredentials(out string userId, out bool isAdmin);

        try
        {
            var newProjectDto = await _projectService.CreateProjectAsync(projectDto, userId, isAdmin);
            // api/projects/{newId} ---> CreatedAtAction() returns the new URL for the resource
            return CreatedAtAction(nameof(GetProject), new { projectId = newProjectDto.ProjectId }, newProjectDto);
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
        GetUserCredentials(out string userId, out bool isAdmin);

        try
        {
            var result = await _projectService.UpdateProjectAsync(projectId, projectDto, userId, isAdmin);
            if (result is null)
            {
                return NotFound();
            }
            if (result is false)
            {
                return StatusCode(500, "An internal error has occurred.");
            }
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
        GetUserCredentials(out string userId, out bool isAdmin);

        try
        {
            var result = await _projectService.DeleteProjectAsync(projectId, userId, isAdmin);
            if (result is null)
            {
                return NotFound();
            }

            return Ok();
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

    private void GetUserCredentials(out string userId, out bool isAdmin)
    {
        userId = _userContextHelper.GetUserId();
        isAdmin = _userContextHelper.IsAdmin();
    }
}