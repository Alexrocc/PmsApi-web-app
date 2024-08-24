using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using PmsApi.DataContexts;
using PmsApi.DTOs;
using PmsApi.Models;

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
        var tasksQuery = _context.Tasks.AsQueryable();      //dynamic query for including dependend data

        if (include.Contains("project", StringComparison.OrdinalIgnoreCase))
        {
            tasksQuery = tasksQuery.Include(p => p.Project);
        }
        if (include.Contains("user", StringComparison.OrdinalIgnoreCase))
        {
            tasksQuery = tasksQuery.Include(p => p.AssignedUser);
        }
        if (include.Contains("attachments", StringComparison.OrdinalIgnoreCase))
        {
            tasksQuery = tasksQuery.Include(p => p.TaskAttachments);
        }

        var tasks = await tasksQuery.ToListAsync();
        var taskDto = _mapper.Map<IEnumerable<TaskAllDto>>(tasks);

        return Ok(taskDto);
    }
}