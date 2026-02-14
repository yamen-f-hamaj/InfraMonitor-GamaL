using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Application.Common.Models;
using InfraMonitor.Domain.Entities;
using InfraMonitor.Domain.Enums;
using Microsoft.Extensions.Options;

namespace InfraMonitor.Infrastructure.Services;

public class ThresholdChecker : IThresholdChecker
{
    private readonly AlertSettings _settings;

    public ThresholdChecker(IOptions<AlertSettings> settings)
    => _settings = settings.Value;

    public IEnumerable<Alert> CheckThresholds(Metric metric)
    {
        var alerts = new List<Alert>();

        foreach (var type in Enum.GetValues<AlertType>())
        {
            var alert = CheckSingleMetric(metric, type);
            if (alert != null)
                alerts.Add(alert);
        }
        return alerts;
    }

    #region  Private Methods

    private Alert? CheckSingleMetric(Metric metric, AlertType type)
    {
        return type switch
        {
            AlertType.CPU when metric.CpuUsage > _settings.CpuThreshold => 
                CreateAlert(metric, AlertType.CPU, metric.CpuUsage, _settings.CpuThreshold),
            
            AlertType.Memory when metric.MemoryUsage > _settings.MemoryThreshold => 
                CreateAlert(metric, AlertType.Memory, metric.MemoryUsage, _settings.MemoryThreshold),
            
            AlertType.ResponseTime when metric.ResponseTime > _settings.ResponseTimeThreshold => 
                CreateAlert(metric, AlertType.ResponseTime, metric.ResponseTime, _settings.ResponseTimeThreshold),
            
            AlertType.Status when metric.Status == ServerStatus.Down => 
                new Alert
                {
                    ServerId = metric.ServerId,
                    MetricType = AlertType.Status.ToString(),
                    MetricValue = 0,
                    Threshold = 1,
                    Status = AlertStatus.Critical.ToString(),
                    CreatedAt = DateTime.UtcNow
                },
            
            _ => null
        };
    }

    private Alert CreateAlert(Metric metric, AlertType type, double value, double threshold)
   => new ()
        {
            ServerId = metric.ServerId,
            MetricType = type.ToString(),
            MetricValue = value,
            Threshold = threshold,
            Status = AlertStatus.Warning.ToString(),
            CreatedAt = DateTime.UtcNow
        };        
    #endregion
}
