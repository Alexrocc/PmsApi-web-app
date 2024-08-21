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
    public async Task<ActionResult<List<User>>> GetUsers()
    {
        var userList = await _context.Users.ToListAsync();

        return Ok(userList);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
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
            // api/users/{newId} ---> CreatedAtAction() returns the new URL for the resource
            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
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

    [HttpPatch("{userId:int}")]
    public async Task<ActionResult> UpdateUser([FromRoute] int userId, [FromBody] UpdateUserDto userDto) //[FromBody] is not necessary for POST and PUT calls, since it is implicitly understood by Entity
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

        user.Username = userDto.UserName;
        user.Email = userDto.Email;
        user.FirstName = userDto.FirstName;
        user.LastName = userDto.Lastname;
        user.RoleId = userDto.RoleId;

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
}