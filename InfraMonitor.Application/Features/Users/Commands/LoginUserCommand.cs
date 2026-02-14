using InfraMonitor.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using InfraMonitor.Application.Common.Exceptions;

namespace InfraMonitor.Application.Features.Users.Commands;

public record LoginUserCommand(string Email, string Password) : IRequest<AuthResponse>;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginUserCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid credentials.");


        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponse(user.UserId, user.UserName, user.Email, user.Role.Name, token);
    }
}
