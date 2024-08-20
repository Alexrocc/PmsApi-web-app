using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PmsApi.DataContexts;
using PmsApi.Models;

namespace PmsApi.Controllers;


[ApiController]                 //needed to define the controller
[Route("api/users")]
public class UsersController : ControllerBase
{

    private readonly PmsapiContext _context;
    public UsersController(PmsapiContext context)
    {
        _context = context;
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
}


