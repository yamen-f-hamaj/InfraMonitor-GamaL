using InfraMonitor.Domain.Entities;

namespace InfraMonitor.Application.Common.Interfaces;

public interface IReportGenerator
{
    Task<string> GenerateReportAsync(Report report, CancellationToken cancellationToken);
}
