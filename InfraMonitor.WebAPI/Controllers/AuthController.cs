using InfraMonitor.Application.Features.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace InfraMonitor.WebAPI.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator)
    => _mediator = mediator;

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterUserCommand command)
    => Ok(await _mediator.Send(command));

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginUserCommand command)
    => Ok(await _mediator.Send(command));
}
