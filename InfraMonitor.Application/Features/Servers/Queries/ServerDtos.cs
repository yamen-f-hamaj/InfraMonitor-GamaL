using InfraMonitor.Domain.Enums;

namespace InfraMonitor.Application.Features.Servers.Queries;

public class ServerDto
{
    public int ServerId { get; set; }
    public string Name { get; set; } = null!;
    public string? Ipaddress { get; set; }
    public ServerStatus Status { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ServerDetailsDto : ServerDto
{
    public List<MetricDto> RecentMetrics { get; set; } = new();
}

public class MetricDto
{
    public int MetricId { get; set; }
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double DiskUsage { get; set; }
    public double ResponseTime { get; set; }
    public ServerStatus Status { get; set; }
    public DateTime Timestamp { get; set; }
}
