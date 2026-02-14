using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Domain.Entities;
using InfraMonitor.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InfraMonitor.Infrastructure.BackgroundJobs;
// Added this report job to add other type of jobs ( as per check list)
public class DailyReportSchedulerJob
{
    private readonly IApplicationDbContext _context;
    private readonly IBackgroundJobService _backgroundJob;
    private readonly ILogger<DailyReportSchedulerJob> _logger;

    public DailyReportSchedulerJob(
        IApplicationDbContext context,
        IBackgroundJobService backgroundJob,
        ILogger<DailyReportSchedulerJob> logger)
    {
        _context = context;
        _backgroundJob = backgroundJob;
        _logger = logger;
    }

    public async Task ScheduleDailyReports(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting daily report scheduling...");

        var servers = await _context.Servers.ToListAsync(cancellationToken);
        var startTime = DateTime.UtcNow.AddDays(-1);
        var endTime = DateTime.UtcNow;

        foreach (var server in servers)
        {
            var report = CreateDailyReport(server, startTime, endTime);
            await AddToDbAsync(report, cancellationToken);

            // Queue the  generation job (Fire-and-Forget)
            _backgroundJob.Enqueue<IReportGeneratorJob>(job => job.GenerateReport(report.ReportId, CancellationToken.None));
            
            _logger.LogInformation("Scheduled daily report for Server {ServerName} (ReportId: {ReportId})", server.Name, report.ReportId);
        }

        _logger.LogInformation("Daily report scheduling completed.");
    }
    
    #region  Private Methods

    private static Report CreateDailyReport(Server server, DateTime startTime, DateTime endTime)
    => new ()
    {
        ServerId = server.ServerId,
        ReportName = $"Daily_Report_{server.Name}_{DateTime.UtcNow:yyyyMMdd}",
        StartTime = startTime,
        EndTime = endTime,
        Status = ReportStatus.Pending,
        CreatedAt = DateTime.UtcNow
    };

    private async Task AddToDbAsync(Report report, CancellationToken cancellationToken)
    {
        _context.Reports.Add(report);
        await _context.SaveChangesAsync(cancellationToken);
    }
    #endregion
}
