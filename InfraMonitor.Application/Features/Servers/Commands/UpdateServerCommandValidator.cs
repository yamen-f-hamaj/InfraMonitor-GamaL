using FluentValidation;

namespace InfraMonitor.Application.Features.Servers.Commands;

public class UpdateServerCommandValidator : AbstractValidator<UpdateServerCommand>
{
    public UpdateServerCommandValidator()
    {
        RuleFor(v => v.ServerId)
            .NotEmpty();

        RuleFor(v => v.Name)
            .MaximumLength(100)
            .NotEmpty();

        RuleFor(v => v.Ipaddress)
            .MaximumLength(50)
            .Matches(@"^((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)\.){3}(25[0-5]|(2[0-4]|1\d|[1-9]|)\d)$")//rejex
            .WithMessage("Invalid IP Address format.")
            .When(v => !string.IsNullOrEmpty(v.Ipaddress));

        RuleFor(v => v.Status)
            .IsInEnum();
            
        RuleFor(v => v.Description)
            .MaximumLength(250);
    }
}
