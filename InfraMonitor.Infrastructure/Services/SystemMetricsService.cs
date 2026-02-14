using System.Diagnostics;
using InfraMonitor.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace InfraMonitor.Infrastructure.Services;


public class SystemMetricsService : ISystemMetricsService, IDisposable
{
    private PerformanceCounter? _cpuCounter;
    private PerformanceCounter? _ramCounter;
    private PerformanceCounter? _diskCounter;
    private readonly ILogger<SystemMetricsService> _logger;

    public SystemMetricsService(ILogger<SystemMetricsService> logger)
    {
        _logger = logger;
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
                _diskCounter = new PerformanceCounter("LogicalDisk", "% Free Space", "_Total");
                
                // Initial call to "prime" the counters (CPU often returns 0 on first call)
                _cpuCounter.NextValue();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Windows Performance Counters.");
            }
        }
        else
        {
            _logger.LogWarning("Performance Counters are only supported on Windows. SystemMetricsService will return 0.");
        }
    }

    [SupportedOSPlatform("windows")]
    public float GetCpuUsage()
    {
        try
        {
            return _cpuCounter?.NextValue() ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading CPU counter.");
            return 0;
        }
    }

    [SupportedOSPlatform("windows")]
    public float GetMemoryUsage()
    {
        try
        {
            return _ramCounter?.NextValue() ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading RAM counter.");
            return 0;
        }
    }

    [SupportedOSPlatform("windows")]
    public float GetDiskUsage()
    {
        try
        {
            // The user wanted "% Free Space", we might want to return "Usage" (100 - Free) if we want usage percentage
            float freeSpace = _diskCounter?.NextValue() ?? 100;
            return 100 - freeSpace; 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading Disk counter.");
            return 0;
        }
    }

    public void Dispose()
    {
        _cpuCounter?.Dispose();
        _ramCounter?.Dispose();
        _diskCounter?.Dispose();
    }
}
