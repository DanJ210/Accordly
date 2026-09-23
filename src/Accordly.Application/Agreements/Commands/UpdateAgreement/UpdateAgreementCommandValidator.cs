using Accordly.Domain.Enums;
using FluentValidation;

namespace Accordly.Application.Agreements.Commands.UpdateAgreement;

/// <summary>Validates agreement updates.</summary>
public sealed class UpdateAgreementCommandValidator : AbstractValidator<UpdateAgreementCommand>
{
    /// <summary>Initializes validation rules.</summary>
    public UpdateAgreementCommandValidator()
    {
        RuleFor(command => command.AgreementId).NotEmpty();
        RuleFor(command => command.RequestingUserId).NotEmpty();
        RuleFor(command => command.Title).MaximumLength(250).When(command => command.Title is not null);
        RuleFor(command => command)
            .Must(command => command.Title is not null || command.ExpiresAt is not null || command.Status is not null)
            .WithMessage("At least one agreement field must be provided.");
        RuleFor(command => command.Status)
            .Must(status => status is null || Enum.TryParse<AgreementStatus>(status, true, out _))
            .WithMessage("Status must be a valid agreement lifecycle value.");
    }
}