using InfraMonitor.Domain.Entities;

namespace InfraMonitor.Application.Common.Interfaces;

public interface IMetricCollector
{
    Task CollectAndStoreMetricsAsync(CancellationToken cancellationToken = default);
}
