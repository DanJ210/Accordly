using FluentValidation;

namespace Accordly.Application.Agreements.Commands.InviteSignatory;

/// <summary>Validates signatory invitations.</summary>
public sealed class InviteSignatoryCommandValidator : AbstractValidator<InviteSignatoryCommand>
{
    public InviteSignatoryCommandValidator()
    {
        RuleFor(command => command.Email).NotEmpty().EmailAddress();
        RuleFor(command => command.Role).Must(role => Enum.TryParse<Accordly.Domain.Enums.SignatoryRole>(role, true, out _)).WithMessage("Invalid signatory role.");
    }
}
