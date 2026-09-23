using Accordly.Application.Agreements.Queries.GetAgreement;
using Accordly.Application.Common.Interfaces;
using Accordly.Domain.Entities;
using Moq;

namespace Accordly.Unit.Application;

[TestClass]
public sealed class GetAgreementQueryHandlerTests
{
    [TestMethod]
    public async Task Handle_WhenAuthorizationAllowsRead_ReturnsAgreement()
    {
        var agreement = new Agreement { Title = "Consulting agreement", OwnerId = Guid.NewGuid() };
        var agreementRepository = new Mock<IAgreementRepository>();
        var authorizationService = new Mock<IAgreementAuthorizationService>();
        agreementRepository.Setup(repository => repository.GetByIdAsync(agreement.Id, It.IsAny<CancellationToken>())).ReturnsAsync(agreement);
        authorizationService.Setup(service => service.CanReadAsync(agreement.Id, agreement.OwnerId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var handler = new GetAgreementQueryHandler(agreementRepository.Object, authorizationService.Object);

        var result = await handler.Handle(new GetAgreementQuery(agreement.Id, agreement.OwnerId), CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.AreEqual(agreement.Id, result.Id);
        authorizationService.Verify(service => service.CanReadAsync(agreement.Id, agreement.OwnerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task Handle_WhenAuthorizationDeniesRead_ReturnsNull()
    {
        var agreement = new Agreement { Title = "Private agreement", OwnerId = Guid.NewGuid() };
        var requestingUserId = Guid.NewGuid();
        var agreementRepository = new Mock<IAgreementRepository>();
        var authorizationService = new Mock<IAgreementAuthorizationService>();
        agreementRepository.Setup(repository => repository.GetByIdAsync(agreement.Id, It.IsAny<CancellationToken>())).ReturnsAsync(agreement);
        authorizationService.Setup(service => service.CanReadAsync(agreement.Id, requestingUserId, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var handler = new GetAgreementQueryHandler(agreementRepository.Object, authorizationService.Object);

        var result = await handler.Handle(new GetAgreementQuery(agreement.Id, requestingUserId), CancellationToken.None);

        Assert.IsNull(result);
    }
}