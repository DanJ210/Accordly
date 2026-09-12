using Accordly.Application.Agreements.Commands.CreateAgreement;

namespace Accordly.Unit.Application;

[TestClass]
public sealed class CreateAgreementCommandValidatorTests
{
    private readonly CreateAgreementCommandValidator validator = new();

    [TestMethod]
    public void Validate_WithEmptyTitle_Fails()
    {
        var command = new CreateAgreementCommand(string.Empty, Guid.NewGuid(), null);

        var result = validator.Validate(command);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(error => error.PropertyName == nameof(CreateAgreementCommand.Title)));
    }

    [TestMethod]
    public void Validate_WithTitleLongerThan250Characters_Fails()
    {
        var command = new CreateAgreementCommand(new string('a', 251), Guid.NewGuid(), null);

        var result = validator.Validate(command);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(error => error.PropertyName == nameof(CreateAgreementCommand.Title)));
    }

    [TestMethod]
    public void Validate_WithValidCommand_Passes()
    {
        var command = new CreateAgreementCommand("Consulting agreement", Guid.NewGuid(), DateTimeOffset.UtcNow.AddDays(30));

        var result = validator.Validate(command);

        Assert.IsTrue(result.IsValid);
    }
}