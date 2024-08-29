using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using PmsApi.DataContexts;
using PmsApi.DTOs;
using PmsApi.Utilities;
using Task = PmsApi.Models.Task;

namespace PmsApi.Controllers;

[ApiController]                 //needed to define the controller
[Route("api/tasks"), Authorize]

public class TasksController : ControllerBase
{
    private readonly IUserContextHelper _userContextHelper;
    private readonly PmsapiContext _context;
    private readonly IMapper _mapper;

    public TasksController(PmsapiContext context, IMapper mapper, IUserContextHelper userContextHelper)
    {
        _context = context;
        _mapper = mapper;
        _userContextHelper = userContextHelper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskAllDto>>> GetTasks([FromQuery] string include = "")
    {
        var tasksQuery = QueryHelper.ApplyTaskIncludes(_context.Tasks.AsQueryable(), include);      //dynamic query for including dependend data with helper class

        if (!_userContextHelper.IsAdmin())
        {
            tasksQuery.Where(p => p.AssignedUserId == _userContextHelper.GetUserId());
        }
        var tasks = await tasksQuery.ToListAsync();
        var taskDto = _mapper.Map<IEnumerable<TaskAllDto>>(tasks);

        return Ok(taskDto);
    }

    [HttpGet("{taskId:int}")]
    public async Task<ActionResult<TaskAllDto>> GetTask([FromRoute] int taskId, [FromQuery] string include = "")
    {
        var tasksQuery = QueryHelper.ApplyTaskIncludes(_context.Tasks.AsQueryable(), include);
        if (!_userContextHelper.IsAdmin())
        {
            tasksQuery.Where(p => p.AssignedUserId == _userContextHelper.GetUserId());
        }
        var task = await tasksQuery.FirstAsync(p => p.TaskId == taskId);
        if (task is null)
        {
            return NotFound();
        }
        var taskDto = _mapper.Map<TaskAllDto>(task);
        return Ok(taskDto);
    }

    [HttpGet("{taskId}/attachments")]
    public async Task<ActionResult<IEnumerable<AttachmentWithTaskDto>>> GetTaskAttachments(int taskId)
    {
        var task = await _context.Tasks.Include(y => y.TaskAttachments).Where(x => x.TaskId == taskId).FirstOrDefaultAsync();

        if (task == null)
        {
            return NotFound();
        }

        if (!_userContextHelper.IsAdmin() && task.AssignedUserId != _userContextHelper.GetUserId())
        {
            return Unauthorized();
        }
        var taskAttachments = task.TaskAttachments;
        var taskAttachmentsDto = _mapper.Map<IEnumerable<AttachmentWithTaskDto>>(taskAttachments);

        return Ok(taskAttachmentsDto);
    }

    [HttpPost]
    public async Task<ActionResult> CreateTask(CreateTaskDto taskDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!_userContextHelper.IsAdmin())
        {
            taskDto.AssignedUserId = _userContextHelper.GetUserId();
        }

        var task = _mapper.Map<Task>(taskDto);

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

        if (!_userContextHelper.IsAdmin() && task.AssignedUserId != _userContextHelper.GetUserId())
        {
            return Unauthorized();
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
        if (!_userContextHelper.IsAdmin() && task.AssignedUserId != _userContextHelper.GetUserId())
        {
            return Unauthorized();
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
            return BadRequest("The task has other dependencies. Please delete those first.");
        }
        catch (Exception)
        {
            return StatusCode(500, "An internal error has occoured.");
        }
    }
}