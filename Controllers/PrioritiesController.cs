using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using PmsApi.DataContexts;
using PmsApi.DTOs;
using PmsApi.Models;

namespace PmsApi.Controllers;

[ApiController]                 //needed to define the controller
[Route("api/priorities")]

public class PrioritiesController : ControllerBase
{
    private readonly PmsapiContext _context;
    private readonly IMapper _mapper;

    public PrioritiesController(PmsapiContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PriorityDto>>> GetPriorities()
    {
        var priorities = await _context.Priorities.ToListAsync();
        var priorityDtos = _mapper.Map<IEnumerable<PriorityDto>>(priorities);

        return Ok(priorityDtos);
    }

    [HttpGet("{priorityId:int}")]
    public async Task<ActionResult<PriorityDto>> GetPriority([FromRoute] int priorityId)
    {
        Priority? priority = await _context.Priorities.FirstAsync(p => p.PriorityId == priorityId);
        if (priority is null)
        {
            return NotFound();
        }
        var priorityDto = _mapper.Map<PriorityDto>(priority);
        return Ok(priorityDto);
    }

    [HttpPost]
    public async Task<ActionResult> CreatePriority(CreatePriorityDto priorityDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var priority = _mapper.Map<Priority>(priorityDto);

        try
        {
            _context.Priorities.Add(priority);
            await _context.SaveChangesAsync();
            var newPriorityDto = _mapper.Map<PriorityDto>(priority);
            // api/projects/{newId} ---> CreatedAtAction() returns the new URL for the resource
            return CreatedAtAction(nameof(GetPriority), new { priorityId = priority.PriorityId }, newPriorityDto);
        }
        catch (DbUpdateException ex)
        when (ex.InnerException is MySqlException mySqlException
        && mySqlException.Number == 1062)
        {
            return BadRequest("Priority name already exists.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("{priorityId:int}")]
    public async Task<ActionResult> UpdatePriority([FromRoute] int priorityId, [FromBody] CreatePriorityDto priorityDto) //[FromBody] is not necessary for POST and PUT calls, since it is implicitly understood by Entity
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Priority? priority = await _context.Priorities.FindAsync(priorityId);

        if (priority is null)
        {
            return NotFound($"The category with the ID {priorityId} could not be found.");
        }

        _mapper.Map(priorityDto, priority);

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
            return BadRequest("Priority name already taken.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete]
    public async Task<ActionResult> DeletePriority(int priorityId)
    {
        Priority? priority = await _context.Priorities.FindAsync(priorityId);

        if (priority == null)
        {
            return NotFound($"No priority found with ID {priorityId}.");
        }
        try
        {
            _context.Priorities.Remove(priority);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex)
        when (ex.InnerException is MySqlException)
        {
            return BadRequest("The priority has other dependencies. Please delete those first.");
        }
        catch (Exception)
        {
            return StatusCode(500, "An internal error has occoured.");
        }
    }
}