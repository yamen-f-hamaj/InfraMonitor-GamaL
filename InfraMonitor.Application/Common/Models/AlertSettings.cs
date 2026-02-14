namespace InfraMonitor.Application.Common.Models;

public class AlertSettings
{
    public const string SectionName = "AlertSettings";
    public double CpuThreshold { get; set; }
    public double MemoryThreshold { get; set; }
    public double ResponseTimeThreshold { get; set; }
}
