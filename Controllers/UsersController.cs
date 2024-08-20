using Microsoft.AspNetCore.Mvc;
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


    [HttpGet("{id:int}")]
    public IActionResult GetUser(int id)
    {
        User? user = _context.Users.FirstOrDefault(u => u.UserId == id);
        if (user is null)
        {
            return NotFound();
        }
        return Ok(user);
    }
}


