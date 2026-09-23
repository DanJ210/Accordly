using Accordly.Application.Agreements.Commands.CreateVersion;
using Accordly.Application.Common.Interfaces;
using Accordly.Domain.Entities;
using Moq;

namespace Accordly.Unit.Application;

[TestClass]
public sealed class CreateVersionCommandHandlerTests
{
    [TestMethod]
    public async Task Handle_WithValidRequest_CreatesNextVersionAndUpdatesAgreement()
    {
        var agreement = new Agreement { Title = "Versioned agreement", OwnerId = Guid.NewGuid() };
        var agreementRepository = new Mock<IAgreementRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        AgreementVersion? addedVersion = null;
        agreementRepository
            .Setup(repository => repository.GetByIdAsync(agreement.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(agreement);
        agreementRepository
            .Setup(repository => repository.GetNextVersionNumberAsync(agreement.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        agreementRepository
            .Setup(repository => repository.AddVersionAsync(It.IsAny<AgreementVersion>(), It.IsAny<CancellationToken>()))
            .Callback<AgreementVersion, CancellationToken>((version, _) => addedVersion = version)
            .Returns(Task.CompletedTask);
        var handler = new CreateVersionCommandHandler(agreementRepository.Object, unitOfWork.Object);

        var response = await handler.Handle(new CreateVersionCommand(agreement.Id, agreement.OwnerId, "<p>First draft</p>", "Initial revision"), CancellationToken.None);

        Assert.IsNotNull(response);
        Assert.IsNotNull(addedVersion);
        Assert.AreEqual(agreement.Id, response.AgreementId);
        Assert.AreEqual(1, response.VersionNumber);
        Assert.AreEqual(agreement.OwnerId, response.AuthorId);
        Assert.AreEqual("<p>First draft</p>", response.Body);
        Assert.AreEqual("Initial revision", response.ChangeNote);
        Assert.AreEqual(agreement.CurrentVersionId, addedVersion.Id);
        Assert.AreEqual(agreement.CurrentVersionId, response.Id);
        agreementRepository.Verify(repository => repository.AddVersionAsync(It.IsAny<AgreementVersion>(), It.IsAny<CancellationToken>()), Times.Once);
        agreementRepository.Verify(repository => repository.UpdateAsync(agreement, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(work => work.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
