using InfraMonitor.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InfraMonitor.Application.Features.Alerts.Queries;

public record GetAlertsQuery(bool? OnlyActive = null) : IRequest<List<AlertDto>>;

public class GetAlertsQueryHandler : IRequestHandler<GetAlertsQuery, List<AlertDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAlertsQueryHandler(IApplicationDbContext context)
    => _context = context;
    

    public async Task<List<AlertDto>> Handle(GetAlertsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Alerts
            .Include(a => a.Server)
            .AsNoTracking();

        if (request.OnlyActive == true)
            query = query.Where(a => a.ResolvedAt == null);
         
        return await query
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AlertDto(
                a.AlertId,
                a.ServerId,
                a.Server.Name,
                a.MetricType,
                a.MetricValue,
                a.Threshold,
                a.Status,
                a.CreatedAt,
                a.ResolvedAt))
            .ToListAsync(cancellationToken);
    }
}
