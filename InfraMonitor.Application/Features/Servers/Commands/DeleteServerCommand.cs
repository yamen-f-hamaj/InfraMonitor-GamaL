using InfraMonitor.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InfraMonitor.Application.Features.Servers.Commands;

public record DeleteServerCommand(int ServerId) : IRequest<bool>;

public class DeleteServerCommandHandler : IRequestHandler<DeleteServerCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteServerCommandHandler(IApplicationDbContext context)
    
       => _context = context;
    
    //we have to make it as soft delete...
    public async Task<bool> Handle(DeleteServerCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Servers
            .FirstOrDefaultAsync(s => s.ServerId == request.ServerId, cancellationToken);

        if (entity is null) 
            return false;

        _context.Servers.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
