using InfraMonitor.Application.Features.Servers.Commands;
using InfraMonitor.Application.Features.Servers.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.AspNetCore.OutputCaching;

namespace InfraMonitor.WebAPI.Controllers;

[Authorize]
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ServersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IOutputCacheStore _cacheStore;

    public ServersController(IMediator mediator, IOutputCacheStore cacheStore)
    {
        _mediator = mediator;
        _cacheStore = cacheStore;
    }

    [HttpGet]
    [OutputCache(PolicyName = "ServersList")]
    public async Task<ActionResult<List<ServerDto>>> GetList()
    {
        var result = await _mediator.Send(new GetServersListQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    [OutputCache(VaryByRouteValueNames = new[] { "id" }, Tags = new[] { "servers-list" })]
    public async Task<ActionResult<ServerDetailsDto>> GetDetails(int id)
    {
        var result = await _mediator.Send(new GetServerDetailsQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateServerCommand command)
    {
        var id = await _mediator.Send(command);
        await _cacheStore.EvictByTagAsync("servers-list", default);
        return CreatedAtAction(nameof(GetDetails), new { id }, id);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateServerCommand command)
    {
        if (id != command.ServerId) return BadRequest();
        var result = await _mediator.Send(command);
        if (!result) return NotFound();
        await _cacheStore.EvictByTagAsync("servers-list", default);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteServerCommand(id));
        if (!result) return NotFound();
        await _cacheStore.EvictByTagAsync("servers-list", default);
        return NoContent();
    }
}
