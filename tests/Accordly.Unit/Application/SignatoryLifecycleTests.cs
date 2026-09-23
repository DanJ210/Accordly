using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Accordly.Application.Agreements.Commands.DeleteSignatory;
using Accordly.Application.Agreements.Commands.InviteSignatory;
using Accordly.Application.Agreements.Commands.SubmitGuestSignature;
using Accordly.Application.Agreements.Commands.SubmitRegisteredSignature;
using Accordly.Application.Agreements.Queries.ListSignatories;
using Accordly.Application.Common.Interfaces;
using Accordly.Domain.Common;
using Accordly.Domain.Entities;
using Accordly.Domain.Enums;
using Moq;

namespace Accordly.Unit.Application;

[TestClass]
public sealed class SignatoryLifecycleTests
{
    private static void SetEntityId(Entity entity, Guid id)
    {
        var property = typeof(Entity).GetProperty(nameof(Entity.Id), BindingFlags.Instance | BindingFlags.Public)!;
        var setter = property.GetSetMethod(true)!;
        setter.Invoke(entity, [id]);
    }

    [TestMethod]
    public async Task InviteSignatory_WithAuthorizedUser_CreatesSignatoryAndHashesToken()
    {
        var agreementId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var repository = new Mock<IAgreementRepository>();
        var authorization = new Mock<IAgreementAuthorizationService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var agreement = new Agreement { Title = "Review", OwnerId = ownerId };
        SetEntityId(agreement, agreementId);
        repository.Setup(service => service.GetByIdAsync(agreementId, It.IsAny<CancellationToken>())).ReturnsAsync(agreement);
        authorization.Setup(service => service.CanMutateAsync(agreementId, ownerId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new InviteSignatoryCommandHandler(repository.Object, authorization.Object, unitOfWork.Object);

        var response = await handler.Handle(new InviteSignatoryCommand(agreementId, ownerId, "guest@example.com", nameof(SignatoryRole.Signer)), CancellationToken.None);

        Assert.IsNotNull(response);
        Assert.AreEqual("guest@example.com", response.Email);
        Assert.AreEqual(nameof(SignatoryRole.Signer), response.Role);
        repository.Verify(service => service.AddSignatoryAsync(It.Is<Signatory>(signatory => signatory.AgreementId == agreementId && signatory.Email == "guest@example.com"), It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(service => service.AddSignatoryAsync(It.Is<Signatory>(signatory => signatory.InviteToken != null && signatory.InviteToken.Length > 0), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(service => service.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task ListSignatories_WithReadAccess_ReturnsVisibleEntries()
    {
        var agreementId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var signatories = new[]
        {
            new Signatory { AgreementId = agreementId, Email = "alice@example.com", Role = SignatoryRole.Signer },
            new Signatory { AgreementId = agreementId, Email = "bob@example.com", Role = SignatoryRole.Viewer }
        };
        foreach (var signatory in signatories)
        {
            SetEntityId(signatory, Guid.NewGuid());
        }

        var repository = new Mock<IAgreementRepository>();
        var authorization = new Mock<IAgreementAuthorizationService>();
        repository.Setup(service => service.GetSignatoriesForAgreementAsync(agreementId, It.IsAny<CancellationToken>())).ReturnsAsync(signatories);
        authorization.Setup(service => service.CanReadAsync(agreementId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new ListSignatoriesQueryHandler(repository.Object, authorization.Object);

        var response = await handler.Handle(new ListSignatoriesQuery(agreementId, userId), CancellationToken.None);

        Assert.IsNotNull(response);
        Assert.AreEqual(2, response.Count);
    }

    [TestMethod]
    public async Task SubmitGuestSignature_WithValidToken_RecordsSignature()
    {
        var agreementId = Guid.NewGuid();
        var currentVersionId = Guid.NewGuid();
        var agreement = new Agreement { Title = "Review", OwnerId = Guid.NewGuid() };
        SetEntityId(agreement, agreementId);
        agreement.CurrentVersionId = currentVersionId;
        var token = "guest-token-123";
        var signatory = new Signatory { AgreementId = agreementId, Email = "guest@example.com", Role = SignatoryRole.Signer, InviteToken = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token))) };
        SetEntityId(signatory, Guid.NewGuid());
        var repository = new Mock<IAgreementRepository>();
        repository.Setup(service => service.GetSignatoryByTokenHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(signatory);
        repository.Setup(service => service.GetByIdAsync(agreementId, It.IsAny<CancellationToken>())).ReturnsAsync(agreement);

        var handler = new SubmitGuestSignatureCommandHandler(repository.Object);

        var response = await handler.Handle(new SubmitGuestSignatureCommand(token, "signature-value", "127.0.0.1"), CancellationToken.None);

        Assert.IsNotNull(response);
        Assert.AreEqual(signatory.Email, response.Email);
        Assert.AreEqual("signature-value", signatory.SignatureValue);
        Assert.IsNotNull(signatory.SignedAt);
        Assert.AreEqual(currentVersionId, signatory.VersionSignedId);
        Assert.IsNull(signatory.InviteToken);
        repository.Verify(service => service.UpdateSignatoryAsync(signatory, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task SubmitRegisteredSignature_WithMatchingUser_RecordsSignature()
    {
        var agreementId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var agreement = new Agreement { Title = "Review", OwnerId = Guid.NewGuid() };
        SetEntityId(agreement, agreementId);
        agreement.CurrentVersionId = Guid.NewGuid();

        var signatory = new Signatory
        {
            AgreementId = agreementId,
            UserId = userId,
            Email = "member@example.com",
            Role = SignatoryRole.Signer
        };
        SetEntityId(signatory, Guid.NewGuid());

        var repository = new Mock<IAgreementRepository>();
        repository.Setup(service => service.GetSignatoryAsync(agreementId, signatory.Id, It.IsAny<CancellationToken>())).ReturnsAsync(signatory);
        repository.Setup(service => service.GetByIdAsync(agreementId, It.IsAny<CancellationToken>())).ReturnsAsync(agreement);

        var handler = new SubmitRegisteredSignatureCommandHandler(repository.Object);

        var response = await handler.Handle(new SubmitRegisteredSignatureCommand(agreementId, signatory.Id, userId, "sig-value", "127.0.0.1"), CancellationToken.None);

        Assert.IsNotNull(response);
        Assert.AreEqual(signatory.Email, response.Email);
        Assert.AreEqual("sig-value", signatory.SignatureValue);
        Assert.AreEqual(agreement.CurrentVersionId, signatory.VersionSignedId);
        Assert.IsNotNull(signatory.SignedAt);
        repository.Verify(service => service.UpdateSignatoryAsync(signatory, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task SubmitGuestSignature_WhenFinalRequiredSignatureIsRecorded_ActivatesAgreement()
    {
        var agreementId = Guid.NewGuid();
        var currentVersionId = Guid.NewGuid();
        var agreement = new Agreement { Title = "Review", OwnerId = Guid.NewGuid() };
        SetEntityId(agreement, agreementId);
        agreement.CurrentVersionId = currentVersionId;
        agreement.TransitionTo(AgreementStatus.PendingSignatures);

        var first = new Signatory { AgreementId = agreementId, Email = "first@example.com", Role = SignatoryRole.Signer, SignedAt = DateTimeOffset.UtcNow, VersionSignedId = currentVersionId };
        var second = new Signatory { AgreementId = agreementId, Email = "second@example.com", Role = SignatoryRole.Signer, InviteToken = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes("guest-token-123"))) };
        SetEntityId(first, Guid.NewGuid());
        SetEntityId(second, Guid.NewGuid());

        var repository = new Mock<IAgreementRepository>();
        repository.Setup(service => service.GetSignatoryByTokenHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(second);
        repository.Setup(service => service.GetByIdAsync(agreementId, It.IsAny<CancellationToken>())).ReturnsAsync(agreement);
        repository.Setup(service => service.GetSignatoriesForAgreementAsync(agreementId, It.IsAny<CancellationToken>())).ReturnsAsync(new[] { first, second });

        var handler = new SubmitGuestSignatureCommandHandler(repository.Object);

        await handler.Handle(new SubmitGuestSignatureCommand("guest-token-123", "signature-value", "127.0.0.1"), CancellationToken.None);

        Assert.AreEqual(AgreementStatus.Active, agreement.Status);
        repository.Verify(service => service.UpdateAsync(agreement, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task DeleteUnsignedSignatory_WithOwnerUser_DeletesRecord()
    {
        var agreementId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var repository = new Mock<IAgreementRepository>();
        var authorization = new Mock<IAgreementAuthorizationService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var signatory = new Signatory { AgreementId = agreementId, Email = "pending@example.com", Role = SignatoryRole.Signer };
        SetEntityId(signatory, Guid.NewGuid());
        repository.Setup(service => service.GetSignatoryAsync(agreementId, signatory.Id, It.IsAny<CancellationToken>())).ReturnsAsync(signatory);
        authorization.Setup(service => service.CanMutateAsync(agreementId, ownerId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = new DeleteSignatoryCommandHandler(repository.Object, authorization.Object, unitOfWork.Object);

        var result = await handler.Handle(new DeleteSignatoryCommand(agreementId, signatory.Id, ownerId), CancellationToken.None);

        Assert.IsTrue(result);
        repository.Verify(service => service.DeleteSignatoryAsync(signatory, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(service => service.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
