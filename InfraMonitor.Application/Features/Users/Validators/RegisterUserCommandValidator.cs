using FluentValidation;
using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Application.Features.Users.Commands;
using Microsoft.EntityFrameworkCore;

namespace InfraMonitor.Application.Features.Users.Validators;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private readonly IApplicationDbContext _context;

    public RegisterUserCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(50).WithMessage("Username must not exceed 50 characters.")
            .MustAsync(BeUniqueUserName).WithMessage("The specified username is already taken.");

        RuleFor(v => v.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters.")
            .MustAsync(BeUniqueEmail).WithMessage("The specified email is already taken.");

        RuleFor(v => v.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
    }

    public async Task<bool> BeUniqueUserName(string userName, CancellationToken cancellationToken)

        => !await _context.Users
            .AnyAsync(u => u.UserName == userName, cancellationToken);


    public async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)

        => !await _context.Users
            .AnyAsync(u => u.Email == email, cancellationToken);

}
