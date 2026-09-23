using Accordly.Application.Agreements.Commands.DeleteAgreement;
using Accordly.Application.Agreements.Commands.UpdateAgreement;
using Accordly.Application.Common.Interfaces;
using Accordly.Domain.Entities;
using Accordly.Domain.Enums;
using FluentValidation.TestHelper;
using Moq;

namespace Accordly.Unit.Application;

[TestClass]
public sealed class UpdateDeleteAgreementCommandHandlerTests
{
    [TestMethod]
    public async Task Update_WithMutationAccess_UpdatesMetadataAndStatus()
    {
        var agreement = new Agreement { Title = "Original", OwnerId = Guid.NewGuid() };
        var repository = new Mock<IAgreementRepository>();
        var authorization = new Mock<IAgreementAuthorizationService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        authorization.Setup(service => service.CanMutateAsync(agreement.Id, agreement.OwnerId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        repository.Setup(service => service.GetByIdAsync(agreement.Id, It.IsAny<CancellationToken>())).ReturnsAsync(agreement);
        var expiresAt = DateTimeOffset.UtcNow.AddDays(30);
        var handler = new UpdateAgreementCommandHandler(repository.Object, authorization.Object, unitOfWork.Object);

        var response = await handler.Handle(new UpdateAgreementCommand(agreement.Id, agreement.OwnerId, "Updated", expiresAt, nameof(AgreementStatus.PendingSignatures)), CancellationToken.None);

        Assert.IsNotNull(response);
        Assert.AreEqual("Updated", agreement.Title);
        Assert.AreEqual(expiresAt, agreement.ExpiresAt);
        Assert.AreEqual(AgreementStatus.PendingSignatures.ToString(), response.Status);
        repository.Verify(service => service.UpdateAsync(agreement, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(service => service.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task Update_WithInvalidLifecycleTransition_ThrowsAndDoesNotPersist()
    {
        var agreement = new Agreement { OwnerId = Guid.NewGuid() };
        var repository = new Mock<IAgreementRepository>();
        var authorization = new Mock<IAgreementAuthorizationService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        authorization.Setup(service => service.CanMutateAsync(agreement.Id, agreement.OwnerId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        repository.Setup(service => service.GetByIdAsync(agreement.Id, It.IsAny<CancellationToken>())).ReturnsAsync(agreement);
        var handler = new UpdateAgreementCommandHandler(repository.Object, authorization.Object, unitOfWork.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(new UpdateAgreementCommand(agreement.Id, agreement.OwnerId, null, null, nameof(AgreementStatus.Active)), CancellationToken.None));

        repository.Verify(service => service.UpdateAsync(It.IsAny<Agreement>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(service => service.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task Delete_WithNonOwner_DoesNotDelete()
    {
        var agreement = new Agreement { OwnerId = Guid.NewGuid() };
        var repository = new Mock<IAgreementRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        repository.Setup(service => service.GetByIdAsync(agreement.Id, It.IsAny<CancellationToken>())).ReturnsAsync(agreement);
        var handler = new DeleteAgreementCommandHandler(repository.Object, unitOfWork.Object);

        var deleted = await handler.Handle(new DeleteAgreementCommand(agreement.Id, Guid.NewGuid()), CancellationToken.None);

        Assert.IsFalse(deleted);
        repository.Verify(service => service.DeleteAsync(It.IsAny<Agreement>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(service => service.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public void UpdateValidator_RejectsEmptyPatchAndInvalidStatus()
    {
        var validator = new UpdateAgreementCommandValidator();

        var emptyResult = validator.TestValidate(new UpdateAgreementCommand(Guid.NewGuid(), Guid.NewGuid(), null, null, null));
        var invalidStatusResult = validator.TestValidate(new UpdateAgreementCommand(Guid.NewGuid(), Guid.NewGuid(), null, null, "Unknown"));

        emptyResult.ShouldHaveValidationErrorFor(command => command);
        invalidStatusResult.ShouldHaveValidationErrorFor(command => command.Status);
    }
}