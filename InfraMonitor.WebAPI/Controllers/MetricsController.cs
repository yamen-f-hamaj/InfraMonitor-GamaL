using InfraMonitor.Application.Features.Metrics.Queries;
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
public class MetricsController : ControllerBase
{
    private readonly IMediator _mediator;

    public MetricsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("latest")]
    [OutputCache(PolicyName = "LatestMetrics")]
    public async Task<ActionResult<List<MetricDto>>> GetLatestSummary()
    {
        return Ok(await _mediator.Send(new GetLatestMetricsSummaryQuery()));
    }

    [HttpGet("history/{serverId}")]
    public async Task<ActionResult<MetricsHistoryDto>> GetHistory(int serverId, [FromQuery] int limit = 100)
    {
        var result = await _mediator.Send(new GetServerMetricsHistoryQuery(serverId, limit));
        if (result == null) return NotFound();
        return Ok(result);
    }
}
