using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User_microservices_example.Data.Models;
using User_microservices_example.Services;

namespace User_microservices_example.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController(UserService userService): ControllerBase
{
    [HttpGet]
    [Route("user/{userId}")]
    public async Task<IActionResult> GetUser(string userId)
    {
        return Ok(await userService.GetUserByUserId(userId));
    }
    
}