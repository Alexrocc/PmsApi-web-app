using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using PmsApi.DataContexts;
using PmsApi.DTOs;
using PmsApi.Models;
using PmsApi.Utilities;
using Task = PmsApi.Models.Task;

namespace PmsApi.Controllers;

[ApiController]                 //needed to define the controller
[Route("api/tasks")]

public class TasksController : ControllerBase
{
    private readonly PmsapiContext _context;
    private readonly IMapper _mapper;

    public TasksController(PmsapiContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskAllDto>>> GetTasks([FromQuery] string include = "")
    {
        var tasksQuery = QueryHelper.ApplyTaskIncludes(_context.Tasks.AsQueryable(), include);      //dynamic query for including dependend data with helper class

        // if (include.Contains("project", StringComparison.OrdinalIgnoreCase))
        // {
        //     tasksQuery = tasksQuery.Include(p => p.Project);
        // }
        // if (include.Contains("user", StringComparison.OrdinalIgnoreCase))
        // {
        //     tasksQuery = tasksQuery.Include(p => p.AssignedUser);
        // }
        // if (include.Contains("attachments", StringComparison.OrdinalIgnoreCase))
        // {
        //     tasksQuery = tasksQuery.Include(p => p.TaskAttachments);
        // }

        var tasks = await tasksQuery.ToListAsync();
        var taskDto = _mapper.Map<IEnumerable<TaskAllDto>>(tasks);

        return Ok(taskDto);
    }

    [HttpGet("{Taskid:int}")]
    public async Task<ActionResult<TaskAllDto>> GetTask([FromRoute] int Taskid, [FromQuery] string include = "")
    {
        var tasksQuery = QueryHelper.ApplyTaskIncludes(_context.Tasks.AsQueryable(), include);

        Task? task = await tasksQuery.FirstAsync(p => p.TaskId == Taskid);
        if (task is null)
        {
            return NotFound();
        }
        var taskDto = _mapper.Map<TaskAllDto>(task);
        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult> CreateTask(CreateTaskDto TaskDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var task = _mapper.Map<Task>(TaskDto);

        try
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            var newTaskDto = _mapper.Map<TaskDto>(task);
            // api/projects/{newId} ---> CreatedAtAction() returns the new URL for the resource
            return CreatedAtAction(nameof(GetTask), new { taskId = task.ProjectId }, newTaskDto);
        }
        catch (DbUpdateException ex)
        when (ex.InnerException is MySqlException mySqlException
        && mySqlException.Number == 1062)
        {
            return BadRequest("Task name already taken.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("{taskId:int}")]
    public async Task<ActionResult> UpdateTask([FromRoute] int taskId, [FromBody] CreateTaskDto taskDto) //[FromBody] is not necessary for POST and PUT calls, since it is implicitly understood by Entity
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Task? task = await _context.Tasks.FindAsync(taskId);

        if (task is null)
        {
            return NotFound($"The project with the ID {taskId} could not be found.");
        }

        _mapper.Map(taskDto, task);

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
    public async Task<ActionResult> DeleteTask(int taskId)
    {
        Task? task = await _context.Tasks.FindAsync(taskId);

        if (task == null)
        {
            return NotFound($"No task found with ID {taskId}.");
        }
        try
        {
            _context.Tasks.Remove(task);
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