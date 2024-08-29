using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using PmsApi.DataContexts;
using PmsApi.DTOs;
using PmsApi.Models;

namespace PmsApi.Controllers;

[ApiController]                 //needed to define the controller
[Route("api/statuses"), Authorize(Policy = "IsAdmin")]

public class StatusController : ControllerBase
{
    private readonly PmsapiContext _context;
    private readonly IMapper _mapper;

    public StatusController(PmsapiContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StatusDto>>> GetStatuses()
    {

        var statuses = await _context.Statuses.ToListAsync();
        var statusDtos = _mapper.Map<IEnumerable<StatusDto>>(statuses);

        return Ok(statusDtos);
    }

    [HttpGet("{statusId:int}")]
    public async Task<ActionResult<StatusDto>> GetStatus([FromRoute] int statusId)
    {
        Status? status = await _context.Statuses.FirstAsync(p => p.StatusId == statusId);
        if (status is null)
        {
            return NotFound();
        }
        var statusDto = _mapper.Map<StatusDto>(status);
        return Ok(statusDto);
    }

    [HttpPost]
    public async Task<ActionResult> CreateStatus(CreateStatusDto statusDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var status = _mapper.Map<Status>(statusDto);

        try
        {
            _context.Statuses.Add(status);
            await _context.SaveChangesAsync();
            var newStatusDto = _mapper.Map<CategoryDto>(status);
            // api/projects/{newId} ---> CreatedAtAction() returns the new URL for the resource
            return CreatedAtAction(nameof(GetStatus), new { statusId = status.StatusId }, newStatusDto);
        }
        catch (DbUpdateException ex)
        when (ex.InnerException is MySqlException mySqlException
        && mySqlException.Number == 1062)
        {
            return BadRequest("Status name already exists.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("{statusId:int}")]
    public async Task<ActionResult> UpdateStatus([FromRoute] int statusId, [FromBody] CreateStatusDto statusDto) //[FromBody] is not necessary for POST and PUT calls, since it is implicitly understood by Entity
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Status? status = await _context.Statuses.FindAsync(statusId);

        if (status is null)
        {
            return NotFound($"The category with the ID {statusId} could not be found.");
        }

        _mapper.Map(statusDto, status);

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
            return BadRequest("Status name already taken.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteStatus(int statusId)
    {
        Status? status = await _context.Statuses.FindAsync(statusId);

        if (status == null)
        {
            return NotFound($"No status found with ID {statusId}.");
        }
        try
        {
            _context.Statuses.Remove(status);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex)
        when (ex.InnerException is MySqlException)
        {
            return BadRequest("The status has other dependencies. Please delete those first.");
        }
        catch (Exception)
        {
            return StatusCode(500, "An internal error has occoured.");
        }
    }
}