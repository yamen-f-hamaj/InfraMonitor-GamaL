using InfraMonitor.Domain.Entities;

namespace InfraMonitor.Application.Common.Interfaces;

public interface IMetricsNotificationService
{
    Task NotifyMetricReceivedAsync(Metric metric, CancellationToken cancellationToken = default);
}
