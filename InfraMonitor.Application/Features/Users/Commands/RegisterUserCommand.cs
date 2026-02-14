using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InfraMonitor.Application.Features.Users.Commands;

public record RegisterUserCommand(string UserName, string Email, string Password) : IRequest<AuthResponse>;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IBackgroundJobService _backgroundJob;

    public RegisterUserCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator
        , IBackgroundJobService backgroundJob)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _backgroundJob = backgroundJob;
    }

    public async Task<AuthResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var isFirstUser = !await _context.Users.AnyAsync(cancellationToken);
        var roleName = isFirstUser ? "Admin" : "User";//just for testing ( in real we have to have user CURD)

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken);
        if (role is null)
        {
            role = new Role { Name = roleName, Description = $"{roleName} Role" };
            _context.Roles.Add(role);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            RoleId = role.RoleId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        // Load role for token generation
        user.Role = role;

        var token = _jwtTokenGenerator.GenerateToken(user);

        // Schedule Welcome Email (Delayed Job)
        _backgroundJob.Schedule<IEmailService>(
            email => email.SendEmailAsync(user.Email, "Welcome to InfraMonitor!", $"Hi {user.UserName}, welcome aboard!", CancellationToken.None),
            TimeSpan.FromMinutes(1)); // Delay by 1 minute

        return new AuthResponse(user.UserId, user.UserName, user.Email, role.Name, token);
    }
}
