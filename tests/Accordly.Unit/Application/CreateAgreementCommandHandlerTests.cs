using Accordly.Application.Agreements.Commands.CreateAgreement;
using Accordly.Application.Common.Interfaces;
using Accordly.Domain.Entities;
using Accordly.Domain.Enums;
using Moq;

namespace Accordly.Unit.Application;

[TestClass]
public sealed class CreateAgreementCommandHandlerTests
{
    [TestMethod]
    public async Task Handle_WithValidCommand_PersistsAgreementAndReturnsResponse()
    {
        var agreementRepository = new Mock<IAgreementRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        Agreement? addedAgreement = null;
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var ownerId = Guid.NewGuid();
        var expiresAt = DateTimeOffset.UtcNow.AddDays(30);
        var command = new CreateAgreementCommand("Consulting agreement", ownerId, expiresAt);

        agreementRepository
            .Setup(repository => repository.AddAsync(It.IsAny<Agreement>(), cancellationToken))
            .Callback<Agreement, CancellationToken>((agreement, _) => addedAgreement = agreement)
            .Returns(Task.CompletedTask);
        unitOfWork
            .Setup(work => work.SaveChangesAsync(cancellationToken))
            .ReturnsAsync(1);
        var handler = new CreateAgreementCommandHandler(agreementRepository.Object, unitOfWork.Object);

        var response = await handler.Handle(command, cancellationToken);

        Assert.IsNotNull(addedAgreement);
        Assert.AreNotEqual(Guid.Empty, response.Id);
        Assert.AreEqual(addedAgreement.Id, response.Id);
        Assert.AreEqual(command.Title, response.Title);
        Assert.AreEqual(AgreementStatus.Draft.ToString(), response.Status);
        Assert.AreEqual(ownerId, response.OwnerId);
        Assert.AreEqual(expiresAt, response.ExpiresAt);
        Assert.AreEqual(addedAgreement.CreatedAt, response.CreatedAt);
        Assert.AreEqual(addedAgreement.UpdatedAt, response.UpdatedAt);
        agreementRepository.Verify(repository => repository.AddAsync(addedAgreement, cancellationToken), Times.Once);
        unitOfWork.Verify(work => work.SaveChangesAsync(cancellationToken), Times.Once);
    }
}