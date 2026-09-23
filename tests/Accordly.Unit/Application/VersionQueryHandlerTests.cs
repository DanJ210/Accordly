using Accordly.Application.Agreements.Queries.GetVersion;
using Accordly.Application.Agreements.Queries.ListVersions;
using Accordly.Application.Common.Interfaces;
using Accordly.Domain.Entities;
using Moq;

namespace Accordly.Unit.Application;

[TestClass]
public sealed class VersionQueryHandlerTests
{
    [TestMethod]
    public async Task GetVersion_WithAuthorizedAccess_ReturnsVersionForAgreement()
    {
        var agreementId = Guid.NewGuid();
        var requestingUserId = Guid.NewGuid();
        var version = new AgreementVersion { AgreementId = agreementId, VersionNumber = 2, Body = "Second", AuthorId = Guid.NewGuid(), ChangeNote = "Update" };
        var repository = new Mock<IAgreementRepository>();
        var authorization = new Mock<IAgreementAuthorizationService>();
        repository.Setup(service => service.GetVersionAsync(agreementId, version.Id, It.IsAny<CancellationToken>())).ReturnsAsync(version);
        authorization.Setup(service => service.CanReadAsync(agreementId, requestingUserId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new GetVersionQueryHandler(repository.Object, authorization.Object);

        var response = await handler.Handle(new GetVersionQuery(agreementId, requestingUserId, version.Id), CancellationToken.None);

        Assert.IsNotNull(response);
        Assert.AreEqual(version.Id, response.Id);
        Assert.AreEqual(agreementId, response.AgreementId);
        Assert.AreEqual(version.VersionNumber, response.VersionNumber);
    }

    [TestMethod]
    public async Task ListVersions_WithUnauthorizedUser_ReturnsNull()
    {
        var agreementId = Guid.NewGuid();
        var requestingUserId = Guid.NewGuid();
        var repository = new Mock<IAgreementRepository>();
        var authorization = new Mock<IAgreementAuthorizationService>();
        authorization.Setup(service => service.CanReadAsync(agreementId, requestingUserId, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var handler = new ListVersionsQueryHandler(repository.Object, authorization.Object);

        var response = await handler.Handle(new ListVersionsQuery(agreementId, requestingUserId), CancellationToken.None);

        Assert.IsNull(response);
        repository.Verify(service => service.GetVersionsForAgreementAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
