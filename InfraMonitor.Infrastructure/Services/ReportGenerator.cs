using System.Text.Json;
using System.Text.Json.Serialization;
using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InfraMonitor.Infrastructure.Services;

public class ReportGenerator : IReportGenerator
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ReportGenerator> _logger;

    public ReportGenerator(IApplicationDbContext context, ILogger<ReportGenerator> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<string> GenerateReportAsync(Report report, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating report {ReportId} for server {ServerId}", report.ReportId, report.ServerId);

        var metrics = await _context.Metrics
            .Where(m => m.ServerId == report.ServerId && m.Timestamp >= report.StartTime && m.Timestamp <= report.EndTime)
            .OrderBy(m => m.Timestamp)
            .ToListAsync(cancellationToken);

        if (!metrics.Any())//I'm using any () because it's more efficient than count () == 0
            _logger.LogWarning("No metrics found for report {ReportId}", report.ReportId);  // Even if empty, create a report file
           
        var reportData = CreateReportData(report, metrics);

        var directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "reports");
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var fileName = $"Report_{report.ServerId}_{report.ReportId}_{DateTime.UtcNow:yyyyMMddHHmmss}.json";
        var filePath = Path.Combine(directory, fileName);

        var options = new JsonSerializerOptions { WriteIndented = true };
        options.Converters.Add(new JsonStringEnumConverter());
        await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(reportData, options), cancellationToken);

        return fileName; // Return relative path or file name
    }

    private static object CreateReportData(Report report, List<Metric> metrics)
    {
        return new
        {
            ReportId = report.ReportId,
            ServerId = report.ServerId,
            GeneratedAt = DateTime.UtcNow,
            MetricsCount = metrics.Count,
            Summary = new
            {
                AvgCpu = metrics.Any() ? metrics.Average(m => m.CpuUsage) : 0,
                AvgMemory = metrics.Any() ? metrics.Average(m => m.MemoryUsage) : 0,
                AvgResponseTime = metrics.Any() ? metrics.Average(m => m.ResponseTime) : 0
            },
            Metrics = metrics.Select(m => new
            {
                m.Timestamp,
                m.CpuUsage,
                m.MemoryUsage,
                m.ResponseTime,
                m.Status
            })
        };
    }
}
