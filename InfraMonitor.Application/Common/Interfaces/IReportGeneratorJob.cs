namespace InfraMonitor.Application.Common.Interfaces;

public interface IReportGeneratorJob
{
    Task GenerateReport(int reportId, CancellationToken cancellationToken);
}
