using InfraMonitor.Domain.Enums;

namespace InfraMonitor.Application.Features.Metrics.Queries;

public record MetricDto(int MetricId, int ServerId, double CpuUsage, double MemoryUsage, double DiskUsage,
                        double ResponseTime, ServerStatus Status, DateTime Timestamp);

public record MetricsHistoryDto(int ServerId, string ServerName, List<MetricDto> History);
