using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Domain.Entities;
using InfraMonitor.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InfraMonitor.Infrastructure.BackgroundJobs;

public class ReportGenerationJob : IReportGeneratorJob
{
    private readonly IApplicationDbContext _context;
    private readonly IReportGenerator _reportGenerator;
    private readonly IEmailService _emailService;
    private readonly ILogger<ReportGenerationJob> _logger;

    public ReportGenerationJob(
        IApplicationDbContext context,
        IReportGenerator reportGenerator,
        IEmailService emailService,
        ILogger<ReportGenerationJob> logger)
    {
        _context = context;
        _reportGenerator = reportGenerator;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task GenerateReport(int reportId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting report generation for ReportId: {ReportId}", reportId);

        var report = await _context.Reports
            .Include(r => r.Server)
            .FirstOrDefaultAsync(r => r.ReportId == reportId, cancellationToken);

        if (report == null)
        {
            _logger.LogError("Report {ReportId} not found.", reportId);
            return;
        }

        try
        {
            report.Status = ReportStatus.Processing;
            await _context.SaveChangesAsync(cancellationToken);

            var filePath = await _reportGenerator.GenerateReportAsync(report, cancellationToken);

            report.Status = ReportStatus.Completed;
            report.FilePath = filePath;
            report.CompletedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync(cancellationToken);

            await _emailService.SendEmailAsync("admin@inframonitor.com", 
                "Report Generated", 
                $"Your report {report.ReportName} is ready. File: {filePath}", 
                cancellationToken);

            _logger.LogInformation("Report {ReportId} completed successfully.", reportId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating report {ReportId}", reportId);
            
            report.Status = ReportStatus.Failed;
            report.ErrorMessage = ex.Message;
            report.CompletedAt = DateTime.UtcNow; // Mark as completed (failed)
            
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
