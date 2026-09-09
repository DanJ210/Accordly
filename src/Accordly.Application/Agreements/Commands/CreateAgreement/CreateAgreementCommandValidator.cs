using FluentValidation;

namespace Accordly.Application.Agreements.Commands.CreateAgreement;

/// <summary>Validates agreement creation.</summary>
public sealed class CreateAgreementCommandValidator : AbstractValidator<CreateAgreementCommand>
{
    /// <summary>Initializes validation rules.</summary>
    public CreateAgreementCommandValidator()
    {
        RuleFor(command => command.Title).NotEmpty().MaximumLength(250);
        RuleFor(command => command.OwnerId).NotEmpty();
    }
}
