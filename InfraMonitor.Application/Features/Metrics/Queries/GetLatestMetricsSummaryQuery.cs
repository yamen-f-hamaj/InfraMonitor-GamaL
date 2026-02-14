using InfraMonitor.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InfraMonitor.Application.Features.Metrics.Queries;

public record GetLatestMetricsSummaryQuery() : IRequest<List<MetricDto>>;

public class GetLatestMetricsSummaryQueryHandler : IRequestHandler<GetLatestMetricsSummaryQuery, List<MetricDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLatestMetricsSummaryQueryHandler(IApplicationDbContext context)
    => _context = context;
     
    // Get the latest metric for each server ( no Tracking for performace)
    public async Task<List<MetricDto>> Handle(GetLatestMetricsSummaryQuery request, CancellationToken cancellationToken)
    => await _context.Servers
            .AsNoTracking()
            .Select(s => _context.Metrics
                .Where(m => m.ServerId == s.ServerId)
                .OrderByDescending(m => m.Timestamp)
                .FirstOrDefault())
            .Where(m => m != null)
            .Select(m => new MetricDto(
                m!.MetricId,
                m.ServerId,
                m.CpuUsage,
                m.MemoryUsage,
                m.DiskUsage,
                m.ResponseTime,
                m.Status,
                m.Timestamp))
            .ToListAsync(cancellationToken);
}