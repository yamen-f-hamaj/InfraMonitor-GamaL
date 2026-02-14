using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InfraMonitor.Application.Features.Servers.Commands;

public record UpdateServerCommand(int ServerId, string Name, string? Ipaddress, string? Description, ServerStatus Status) : IRequest<bool>;

public class UpdateServerCommandHandler : IRequestHandler<UpdateServerCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateServerCommandHandler(IApplicationDbContext context)
    
        =>_context = context;
    

    public async Task<bool> Handle(UpdateServerCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Servers
            .FirstOrDefaultAsync(s => s.ServerId == request.ServerId, cancellationToken);

        if (entity is null) 
            return false;

        entity.Name = request.Name;
        entity.Ipaddress = request.Ipaddress;
        entity.Description = request.Description;
        entity.Status = request.Status;

        await _context.SaveChangesAsync(cancellationToken);//will execute update , better than attach and save

        return true;
    }
}
