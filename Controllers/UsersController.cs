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


    [HttpGet]
    public List<User> GetUsers()
    {
        return _context.Users.ToList();
    }
}


