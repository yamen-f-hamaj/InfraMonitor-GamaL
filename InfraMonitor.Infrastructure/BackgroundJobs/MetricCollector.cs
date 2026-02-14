using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Domain.Entities;
using InfraMonitor.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InfraMonitor.Infrastructure.BackgroundJobs;

public class MetricCollector : IMetricCollector
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<MetricCollector> _logger;
    private readonly IMetricsNotificationService _notificationService;
    private readonly ICacheInvalidator _cacheInvalidator;
    private readonly ISystemMetricsService _systemMetricsService;
    private readonly Random _random = new();

    public MetricCollector(
        IApplicationDbContext context, 
        ILogger<MetricCollector> _logger,
        IMetricsNotificationService notificationService,
        ICacheInvalidator cacheInvalidator,
        ISystemMetricsService systemMetricsService)
    {
        _context = context;
        this._logger = _logger;
        _notificationService = notificationService;
        _cacheInvalidator = cacheInvalidator;
        _systemMetricsService = systemMetricsService;
    }

    public async Task CollectAndStoreMetricsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting metrics collection for all servers...");

        var servers = await _context.Servers.ToListAsync(cancellationToken);

        if (!servers.Any())
        {
            _logger.LogWarning("No servers found for metric collection.");
            return;
        }

        foreach (var server in servers)        
            await ProcessServerMetricsAsync(server, cancellationToken);
        

        await _context.SaveChangesAsync(cancellationToken);
        await _cacheInvalidator.EvictByTagAsync("metrics-latest", cancellationToken);
        _logger.LogInformation("Metrics collection completed.");
    }

#region  Private Mehods 
    private async Task ProcessServerMetricsAsync(Server server, CancellationToken cancellationToken)
    {
        try
        {
            var metric = CreateMetricForServer(server);

            _context.Metrics.Add(metric);
            
            // Real-time notification and threshold check
            await _notificationService.NotifyMetricReceivedAsync(metric, cancellationToken);
            
            // Update server status based on latest metric
            server.Status = metric.Status;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error collecting metrics for server {ServerName} (ID: {ServerId})", server.Name, server.ServerId);
        }
    }

    private Metric CreateMetricForServer(Server server)
    {
        if (IsLocalMachine(server.Ipaddress))
        {
            _logger.LogInformation("Collected REAL metrics for local machine (ID: {ServerId})", server.ServerId);
            return new Metric
            {
                ServerId = server.ServerId,
                CpuUsage = (double)Math.Round(_systemMetricsService.GetCpuUsage(), 2),
                MemoryUsage = (double)Math.Round(_systemMetricsService.GetMemoryUsage(), 2),
                DiskUsage = (double)Math.Round(_systemMetricsService.GetDiskUsage(), 2),
                ResponseTime = 0, // Local is zero latency
                Status = ServerStatus.Up,
                Timestamp = DateTime.UtcNow
            };
        }

        _logger.LogInformation("Generated RANDOM metrics for server {ServerName} (ID: {ServerId})", server.Name, server.ServerId);
        return new Metric
        {
            ServerId = server.ServerId,
            CpuUsage = Math.Round(_random.NextDouble() * 100, 2),
            MemoryUsage = Math.Round(_random.NextDouble() * 100, 2),
            DiskUsage = Math.Round(_random.NextDouble() * 100, 2),
            ResponseTime = _random.Next(1, 500),
            Status = _random.Next(1, 10) > 1 ? ServerStatus.Up : ServerStatus.Down,
            Timestamp = DateTime.UtcNow
        };
    }

    private static bool IsLocalMachine(string? ip)
    {
        if (string.IsNullOrEmpty(ip)) return true; // Assume local if no IP
        var lowerIp = ip.ToLower();
        return lowerIp == "127.0.0.1" || lowerIp == "localhost" || lowerIp == "::1";
    }
#endregion
}
