using InfraMonitor.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InfraMonitor.Application.Features.Servers.Queries;

public record GetServerDetailsQuery(int ServerId) : IRequest<ServerDetailsDto?>;

public class GetServerDetailsQueryHandler : IRequestHandler<GetServerDetailsQuery, ServerDetailsDto?>
{
    private readonly IApplicationDbContext _context;

    public GetServerDetailsQueryHandler(IApplicationDbContext context)
    =>
        _context = context;
    

    public async Task<ServerDetailsDto?> Handle(GetServerDetailsQuery request, CancellationToken cancellationToken)
    {
        var server = await _context.Servers
            .Where(s => s.ServerId == request.ServerId)
            .Select(s => new ServerDetailsDto
            {
                ServerId = s.ServerId,
                Name = s.Name,
                Ipaddress = s.Ipaddress,
                Status = s.Status,
                Description = s.Description,
                CreatedAt = s.CreatedAt,
                RecentMetrics = s.Metrics
                    .OrderByDescending(m => m.Timestamp)
                    .Take(10)
                    .Select(m => new MetricDto
                    {
                        MetricId = m.MetricId,
                        CpuUsage = m.CpuUsage,
                        MemoryUsage = m.MemoryUsage,
                        DiskUsage = m.DiskUsage,
                        ResponseTime = m.ResponseTime,
                        Status = m.Status,
                        Timestamp = m.Timestamp
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return server;
    }
}
