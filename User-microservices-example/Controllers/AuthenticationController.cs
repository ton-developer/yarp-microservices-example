using Microsoft.AspNetCore.Mvc;
using User_microservices_example.Data.Models;
using User_microservices_example.Services;

namespace User_microservices_example.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController(AuthService authService, ILogger<AuthenticationController> logger)
    : ControllerBase
{

    [HttpPost]
    [Route("CreateRoles")]
    public async Task<IActionResult> CreateRoles()
    {
        await authService.CreateRoles();

        return Ok();
    }
    
    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid payload");
            var (status, message) = await authService.Login(model);
            if (status == 0)
                return BadRequest(message);
            return Ok(message);
        }
        catch(Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
    
    [HttpPost]
    [Route("Register")]

    public async Task<IActionResult> Register(RegistrationModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid payload");
            var (status, message) = await authService.Registration(model, UserRoles.User);
            if (status == 0)
            {
                return BadRequest(message);
            }
            return CreatedAtAction(nameof(Register), model);

        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}