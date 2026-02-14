using InfraMonitor.Domain.Entities;

namespace InfraMonitor.Application.Common.Interfaces;

public interface IThresholdChecker
{
    IEnumerable<Alert> CheckThresholds(Metric metric);
}
