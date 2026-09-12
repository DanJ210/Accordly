using Accordly.Domain.Entities;
using Accordly.Domain.Enums;

namespace Accordly.Unit.Domain;

[TestClass]
public sealed class AgreementStatusTests
{
    [TestMethod]
    public void TransitionTo_ThroughSigningWorkflow_UpdatesStatus()
    {
        var agreement = new Agreement();

        agreement.TransitionTo(AgreementStatus.PendingSignatures);
        agreement.TransitionTo(AgreementStatus.Active);

        Assert.AreEqual(AgreementStatus.Active, agreement.Status);
    }

    [TestMethod]
    [DataRow(AgreementStatus.Expired)]
    [DataRow(AgreementStatus.Terminated)]
    public void TransitionTo_FromActiveToFinalStatus_UpdatesStatus(AgreementStatus finalStatus)
    {
        var agreement = CreateActiveAgreement();

        agreement.TransitionTo(finalStatus);

        Assert.AreEqual(finalStatus, agreement.Status);
    }

    [TestMethod]
    public void TransitionTo_FromActiveBackToDraft_Throws()
    {
        var agreement = CreateActiveAgreement();

        var exception = Assert.ThrowsExactly<InvalidOperationException>(
            () => agreement.TransitionTo(AgreementStatus.Draft));

        StringAssert.Contains(exception.Message, "Active to Draft");
        Assert.AreEqual(AgreementStatus.Active, agreement.Status);
    }

    [TestMethod]
    public void TransitionTo_FromFinalStatus_Throws()
    {
        var agreement = CreateActiveAgreement();
        agreement.TransitionTo(AgreementStatus.Expired);

        Assert.ThrowsExactly<InvalidOperationException>(
            () => agreement.TransitionTo(AgreementStatus.Terminated));
        Assert.AreEqual(AgreementStatus.Expired, agreement.Status);
    }

    private static Agreement CreateActiveAgreement()
    {
        var agreement = new Agreement();
        agreement.TransitionTo(AgreementStatus.PendingSignatures);
        agreement.TransitionTo(AgreementStatus.Active);
        return agreement;
    }
}