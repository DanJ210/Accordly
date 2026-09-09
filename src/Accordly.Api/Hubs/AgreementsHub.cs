using Microsoft.AspNetCore.SignalR;

namespace Accordly.Api.Hubs;

/// <summary>Client events broadcast for agreement collaboration.</summary>
public interface IAgreementsHubClient
{
    /// <summary>Notifies clients that a version was created.</summary>
    Task VersionCreated(object payload);
    /// <summary>Notifies clients that a signatory changed.</summary>
    Task SignatoryUpdated(object payload);
    /// <summary>Notifies clients that status changed.</summary>
    Task AgreementStatusChanged(object payload);
    /// <summary>Notifies clients that an attachment was uploaded.</summary>
    Task AttachmentUploaded(object payload);
}

/// <summary>SignalR hub for agreement updates.</summary>
public sealed class AgreementsHub : Hub<IAgreementsHubClient> { }
