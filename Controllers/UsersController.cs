using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using PmsApi.DataContexts;
using PmsApi.DTOs;
using PmsApi.Models;

namespace PmsApi.Controllers;

[ApiController]                 //needed to define the controller
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly PmsapiContext _context;
    private readonly IMapper _mapper;
    public UsersController(PmsapiContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetUsers([FromQuery] string include = "")
    {
        var usersQuery = _context.Users.AsQueryable();      //dynamic query for including dependend data
        if (include.Contains("projects", StringComparison.OrdinalIgnoreCase))
        {
            usersQuery = usersQuery.Include(u => u.Projects);
        }
        if (include.Contains("tasks", StringComparison.OrdinalIgnoreCase))
        {
            usersQuery = usersQuery.Include(u => u.Tasks);
        }

        var users = await usersQuery.ToListAsync();
        var usersDto = _mapper.Map<IEnumerable<UserDto>>(users);

        return Ok(usersDto);
    }

    [HttpGet("{userId:string}")]
    public async Task<ActionResult<User>> GetUser(string userId)
    {
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult> CreateUser(CreateUserDto userDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = _mapper.Map<User>(userDto);

        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            var newUserDto = _mapper.Map<User>(userDto);
            // api/users/{newId} ---> CreatedAtAction() returns the new URL for the resource
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, newUserDto);
        }
        catch (DbUpdateException ex)
        when (ex.InnerException is MySqlException mySqlException
        && mySqlException.Number == 1062)
        {
            return BadRequest("Email already exists.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPatch("{userId:string}")]
    public async Task<ActionResult> UpdateUser([FromRoute] string userId, [FromBody] UpdateUserDto userDto) //[FromBody] is not necessary for POST and PUT calls, since it is implicitly understood by Entity
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        User? user = await _context.Users.FindAsync(userId);

        if (user is null)
        {
            return NotFound($"The user with the id {userId} could not be found.");
        }

        _mapper.Map(userDto, user);

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
            return BadRequest("Email already exists.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteUser(int userId)
    {
        User? user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound($"Could not find the user with ID {userId}.");
        }
        try
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex)
        when (ex.InnerException is MySqlException)
        {
            return BadRequest("The user has other records. Please delete assigned tasks.");
        }
        catch (Exception)
        {
            return StatusCode(500, "An internal error has occoured.");
        }
    }
}