namespace InfraMonitor.Application.Features.Alerts.Queries;

public record AlertDto(int AlertId, int ServerId, string ServerName, string MetricType, double MetricValue,
                      double Threshold, string Status, DateTime CreatedAt, DateTime? ResolvedAt);
