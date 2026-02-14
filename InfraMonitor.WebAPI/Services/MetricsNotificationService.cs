using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Domain.Entities;
using InfraMonitor.WebAPI.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace InfraMonitor.WebAPI.Services;

public class MetricsNotificationService : IMetricsNotificationService
{
    private readonly IHubContext<MetricsHub> _hubContext;
    private readonly IApplicationDbContext _context;
    private readonly IThresholdChecker _thresholdChecker;

    public MetricsNotificationService(
        IHubContext<MetricsHub> hubContext, 
        IApplicationDbContext context, 
        IThresholdChecker thresholdChecker)
    {
        _hubContext = hubContext;
        _context = context;
        _thresholdChecker = thresholdChecker;
    }

    public async Task NotifyMetricReceivedAsync(Metric metric, CancellationToken cancellationToken = default)
    {
        // 1. Broadcast the new metric to the specific server group (Real-time update)
        await _hubContext.Clients.Group($"Server_{metric.ServerId}").SendAsync("ReceiveMetric", new
        {
            metric.MetricId,
            metric.ServerId,
            metric.CpuUsage,
            metric.MemoryUsage,
            metric.DiskUsage,
            metric.ResponseTime,
            metric.Status,
            metric.Timestamp
        }, cancellationToken);

        // 2. Check thresholds and generate alerts
        var alerts = _thresholdChecker.CheckThresholds(metric);

        foreach (var alert in alerts)
        {
            _context.Alerts.Add(alert);
            
            // 3. Notify clients about new alerts (Broadcast to all admins or specific groups)
            await _hubContext.Clients.All.SendAsync("ReceiveAlert", new
            {
                alert.ServerId,
                alert.MetricType,
                alert.MetricValue,
                alert.Threshold,
                alert.Status,
                alert.CreatedAt
            }, cancellationToken);
        }

        if (alerts.Any())
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
