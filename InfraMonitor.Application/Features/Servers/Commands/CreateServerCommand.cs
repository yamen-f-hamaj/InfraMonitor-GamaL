using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Domain.Entities;
using InfraMonitor.Domain.Enums;
using MediatR;

namespace InfraMonitor.Application.Features.Servers.Commands;

public record CreateServerCommand(string Name, string? Ipaddress, string? Description) : IRequest<int>;

public class CreateServerCommandHandler : IRequestHandler<CreateServerCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateServerCommandHandler(IApplicationDbContext context)

    => _context = context;


    public async Task<int> Handle(CreateServerCommand request, CancellationToken cancellationToken)
    {
        var entity = new Server
        {
            Name = request.Name,
            Ipaddress = request.Ipaddress,
            Description = request.Description,
            Status = ServerStatus.Up, // make Default status as up
            CreatedAt = DateTime.UtcNow
        };

        _context.Servers.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.ServerId;
    }
}
