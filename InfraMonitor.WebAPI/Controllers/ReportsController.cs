using InfraMonitor.Application.Features.Reports.Commands;
using InfraMonitor.Application.Features.Reports.Queries;
using InfraMonitor.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace InfraMonitor.WebAPI.Controllers;

[Authorize]
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<int>> CreateReport([FromBody] CreateReportCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    [HttpGet]
    public async Task<ActionResult<List<ReportDto>>> GetReports([FromQuery] int? serverId, [FromQuery] ReportStatus? status)
    {
        return Ok(await _mediator.Send(new GetReportsQuery(serverId, status)));
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(int id)
    {
        try
        {
            var fileDto = await _mediator.Send(new GetReportFileQuery(id));
            return File(fileDto.FileContent, fileDto.ContentType, fileDto.FileName);
        }
        catch (FileNotFoundException)
        {
            return NotFound("Report file not found.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
