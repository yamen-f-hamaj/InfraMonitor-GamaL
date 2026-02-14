using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Domain.Entities;
using InfraMonitor.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InfraMonitor.Application.Features.Reports.Commands;

public record CreateReportCommand(int ServerId, DateTime StartTime, DateTime EndTime) : IRequest<int>;

public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IBackgroundJobService _backgroundJob;

    public CreateReportCommandHandler(IApplicationDbContext context, IBackgroundJobService backgroundJob)
    {
        _context = context;
        _backgroundJob = backgroundJob;
    }

    public async Task<int> Handle(CreateReportCommand request, CancellationToken cancellationToken)
    {
        var server = await _context.Servers.FindAsync(new object[] { request.ServerId }, cancellationToken);
        if (server is null)
            throw new Exception($"Server with ID {request.ServerId} not found.");

        var report = await CreateAndSaveReportAsync(request.ServerId, server.Name, request.StartTime, request.EndTime, cancellationToken);

        //using abstraction
        _backgroundJob.Enqueue<IReportGeneratorJob>(job => job.GenerateReport(report.ReportId, CancellationToken.None));

        return report.ReportId;
    }

    #region private method

    private async Task<Report> CreateAndSaveReportAsync(int serverId,string serverName,DateTime startTime,DateTime endTime, CancellationToken cancellationToken)
    {
        var report = new Report
        {
            ServerId = serverId,
            ReportName = $"Performance_Report_{serverName}_{DateTime.UtcNow:yyyyMMddHHmmss}",
            StartTime = startTime,
            EndTime = endTime,
            Status = ReportStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reports.Add(report);
        await _context.SaveChangesAsync(cancellationToken);

        return report;
    }
    #endregion
}


