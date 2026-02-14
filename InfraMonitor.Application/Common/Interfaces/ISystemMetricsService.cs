namespace InfraMonitor.Application.Common.Interfaces;

public interface ISystemMetricsService
{
    float GetCpuUsage();
    float GetMemoryUsage();
    float GetDiskUsage();
}
