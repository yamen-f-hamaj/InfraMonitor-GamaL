using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Domain.Entities;
using InfraMonitor.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InfraMonitor.Application.Features.Reports.Queries;

public record ReportDto(int ReportId, string ReportName, int ServerId, string ServerName, DateTime StartTime,
                         DateTime EndTime, string Status, string? FilePath, string? ErrorMessage, DateTime CreatedAt, DateTime? CompletedAt);

public record GetReportsQuery(int? ServerId = null, ReportStatus? Status = null) : IRequest<List<ReportDto>>;

public class GetReportsQueryHandler : IRequestHandler<GetReportsQuery, List<ReportDto>>
{
    private readonly IApplicationDbContext _context;

    public GetReportsQueryHandler(IApplicationDbContext context)

       => _context = context;


    public async Task<List<ReportDto>> Handle(GetReportsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Reports.Include(r => r.Server).AsQueryable();

        if (request.ServerId.HasValue)
            query = query.Where(r => r.ServerId == request.ServerId.Value);


        if (request.Status.HasValue)
            query = query.Where(r => r.Status == request.Status.Value);

        var reports = await query.ToListAsync(cancellationToken);

        return reports.Select(r => new ReportDto(r.ReportId,
            r.ReportName, r.ServerId, r.Server.Name, r.StartTime, r.EndTime, r.Status.ToString(), r.FilePath, r.ErrorMessage, r.CreatedAt, r.CompletedAt
        )).ToList();
    }
}
