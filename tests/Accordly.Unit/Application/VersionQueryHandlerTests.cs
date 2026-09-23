using System.Reflection;
using Accordly.Application.Agreements.Queries.GetVersion;
using Accordly.Application.Agreements.Queries.GetVersionDiff;
using Accordly.Application.Agreements.Queries.ListVersions;
using Accordly.Application.Common.Interfaces;
using Accordly.Domain.Common;
using Accordly.Domain.Entities;
using Moq;

namespace Accordly.Unit.Application;

[TestClass]
public sealed class VersionQueryHandlerTests
{
    private static void SetEntityId(Entity entity, Guid id)
    {
        var property = typeof(Entity).GetProperty(nameof(Entity.Id), BindingFlags.Instance | BindingFlags.Public)!;
        var setter = property.GetSetMethod(true)!;
        setter.Invoke(entity, new object[] { id });
    }

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

    [TestMethod]
    public async Task GetVersionDiff_WithMatchingVersions_ReturnsTransportSafeDiff()
    {
        var agreementId = Guid.NewGuid();
        var requestingUserId = Guid.NewGuid();
        var fromVersionId = Guid.NewGuid();
        var toVersionId = Guid.NewGuid();
        var repository = new Mock<IAgreementRepository>();
        var authorization = new Mock<IAgreementAuthorizationService>();
        var fromVersion = new AgreementVersion { AgreementId = agreementId, VersionNumber = 1, Body = "<p>One</p>", AuthorId = Guid.NewGuid() };
        var toVersion = new AgreementVersion { AgreementId = agreementId, VersionNumber = 2, Body = "<p>Two</p>", AuthorId = Guid.NewGuid() };
        SetEntityId(fromVersion, fromVersionId);
        SetEntityId(toVersion, toVersionId);
        authorization.Setup(service => service.CanReadAsync(agreementId, requestingUserId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        repository.Setup(service => service.GetVersionAsync(agreementId, fromVersionId, It.IsAny<CancellationToken>())).ReturnsAsync(fromVersion);
        repository.Setup(service => service.GetVersionAsync(agreementId, toVersionId, It.IsAny<CancellationToken>())).ReturnsAsync(toVersion);

        var handler = new GetVersionDiffQueryHandler(repository.Object, authorization.Object);

        var response = await handler.Handle(new GetVersionDiffQuery(agreementId, requestingUserId, fromVersionId, toVersionId), CancellationToken.None);

        Assert.IsNotNull(response);
        Assert.AreEqual(agreementId, response.AgreementId);
        Assert.AreEqual(fromVersionId, response.FromVersionId);
        Assert.AreEqual(toVersionId, response.ToVersionId);
        Assert.IsTrue(response.Changes.Count >= 1);
        Assert.IsTrue(response.Changes.All(change => change.Kind is "added" or "removed" or "unchanged"));
    }

    [TestMethod]
    public async Task GetVersionDiff_WithMismatchedVersions_ReturnsNull()
    {
        var agreementId = Guid.NewGuid();
        var requestingUserId = Guid.NewGuid();
        var fromVersionId = Guid.NewGuid();
        var toVersionId = Guid.NewGuid();
        var repository = new Mock<IAgreementRepository>();
        var authorization = new Mock<IAgreementAuthorizationService>();
        var fromVersion = new AgreementVersion { AgreementId = agreementId, VersionNumber = 1, Body = "One", AuthorId = Guid.NewGuid() };
        var mismatchedVersion = new AgreementVersion { AgreementId = Guid.NewGuid(), VersionNumber = 2, Body = "Two", AuthorId = Guid.NewGuid() };
        SetEntityId(fromVersion, fromVersionId);
        SetEntityId(mismatchedVersion, toVersionId);
        authorization.Setup(service => service.CanReadAsync(agreementId, requestingUserId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        repository.Setup(service => service.GetVersionAsync(agreementId, fromVersionId, It.IsAny<CancellationToken>())).ReturnsAsync(fromVersion);
        repository.Setup(service => service.GetVersionAsync(agreementId, toVersionId, It.IsAny<CancellationToken>())).ReturnsAsync(mismatchedVersion);

        var handler = new GetVersionDiffQueryHandler(repository.Object, authorization.Object);

        var response = await handler.Handle(new GetVersionDiffQuery(agreementId, requestingUserId, fromVersionId, toVersionId), CancellationToken.None);

        Assert.IsNull(response);
    }
}
