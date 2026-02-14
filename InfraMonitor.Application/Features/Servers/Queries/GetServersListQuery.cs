using InfraMonitor.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InfraMonitor.Application.Features.Servers.Queries;

public record GetServersListQuery : IRequest<List<ServerDto>>;

public class GetServersListQueryHandler : IRequestHandler<GetServersListQuery, List<ServerDto>>
{
    private readonly IApplicationDbContext _context;

    public GetServersListQueryHandler(IApplicationDbContext context)
    => _context = context;


    public async Task<List<ServerDto>> Handle(GetServersListQuery request, CancellationToken cancellationToken)
    =>
         await _context.Servers
            .Select(s => new ServerDto
            {
                ServerId = s.ServerId,
                Name = s.Name,
                Ipaddress = s.Ipaddress,
                Status = s.Status,
                Description = s.Description,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync(cancellationToken);
}
