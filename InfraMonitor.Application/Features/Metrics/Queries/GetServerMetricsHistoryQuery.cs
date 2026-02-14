using InfraMonitor.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InfraMonitor.Application.Features.Metrics.Queries;

public record GetServerMetricsHistoryQuery(int ServerId, int Limit = 100) : IRequest<MetricsHistoryDto?>;

public class GetServerMetricsHistoryQueryHandler : IRequestHandler<GetServerMetricsHistoryQuery, MetricsHistoryDto?>
{
    private readonly IApplicationDbContext _context;

    public GetServerMetricsHistoryQueryHandler(IApplicationDbContext context)
    => _context = context;


    public async Task<MetricsHistoryDto?> Handle(GetServerMetricsHistoryQuery request, CancellationToken cancellationToken)
    {
        var server = await _context.Servers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.ServerId == request.ServerId, cancellationToken);

        if (server is null)
            return null;

        var metrics = await _context.Metrics
            .AsNoTracking()
            .Where(m => m.ServerId == request.ServerId)
            .OrderByDescending(m => m.Timestamp)
            .Take(request.Limit)
            .OrderBy(m => m.Timestamp) // Return in chronological order for charts
            .Select(m => new MetricDto(
                m.MetricId,
                m.ServerId,
                m.CpuUsage,
                m.MemoryUsage,
                m.DiskUsage,
                m.ResponseTime,
                m.Status,
                m.Timestamp))
            .ToListAsync(cancellationToken);

        return new(server.ServerId, server.Name, metrics);
    }
}
