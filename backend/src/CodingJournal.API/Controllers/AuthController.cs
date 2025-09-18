using CodingJournal.Application.Authentication.Actions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CodingJournal.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        var result = await mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        return BadRequest(new
        {
            errors = result.Errors,
            message = "Failed to register user."
        });
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        var result = await mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        return BadRequest(new
        {
            errors = result.Errors,
            message = "Failed to login user."
        });   
    }
}