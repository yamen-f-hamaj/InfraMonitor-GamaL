using InfraMonitor.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InfraMonitor.Application.Features.Reports.Queries;

public record ReportFileDto(byte[] FileContent, string FileName, string ContentType);

public record GetReportFileQuery(int ReportId) : IRequest<ReportFileDto>;

public class GetReportFileQueryHandler : IRequestHandler<GetReportFileQuery, ReportFileDto>
{
    private readonly IApplicationDbContext _context;

    public GetReportFileQueryHandler(IApplicationDbContext context)

        => _context = context;


    public async Task<ReportFileDto> Handle(GetReportFileQuery request, CancellationToken cancellationToken)
    {
        var report = await _context.Reports.FindAsync(new object[] { request.ReportId }, cancellationToken);

        if (report is null)
            throw new Exception("Report not found.");


        if (report.Status != Domain.Enums.ReportStatus.Completed || string.IsNullOrEmpty(report.FilePath))
            throw new Exception("Report is not ready or failed.");


        // Assuming file is stored relative to wwwroot/reports as per generator
        var directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "reports");
        var fullPath = Path.Combine(directory, report.FilePath); // report.FilePath is just fileName? Let's check generator. Generator returns fileName.

        if (!File.Exists(fullPath))
        {
            // Fallback if full path was stored directly
            if (File.Exists(report.FilePath))
            {
                fullPath = report.FilePath;
            }
            else
            {
                throw new FileNotFoundException("Report file not found on server.", report.FilePath);
            }
        }

        var content = await File.ReadAllBytesAsync(fullPath, cancellationToken);
        var fileName = report.FilePath; // Or generated name

        return new ReportFileDto(content, fileName, "application/json");
    }
}
