using FluentValidation;

namespace Accordly.Application.Agreements.Commands.CreateVersion;

/// <summary>Validates agreement version creation.</summary>
public sealed class CreateVersionCommandValidator : AbstractValidator<CreateVersionCommand>
{
    /// <summary>Initializes validation rules.</summary>
    public CreateVersionCommandValidator()
    {
        RuleFor(command => command.AgreementId).NotEmpty();
        RuleFor(command => command.AuthorId).NotEmpty();
        RuleFor(command => command.Body).NotEmpty();
        RuleFor(command => command.ChangeNote).MaximumLength(500);
    }
}
