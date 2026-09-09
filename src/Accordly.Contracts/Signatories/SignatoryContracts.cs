namespace Accordly.Contracts.Signatories;

/// <summary>Request to invite a signatory.</summary>
public sealed record InviteSignatoryRequest(string Email, string Role);
/// <summary>Signatory API response.</summary>
public sealed record SignatoryResponse(Guid Id, string Email, string Role, DateTimeOffset? SignedAt);
/// <summary>Request to submit a signature.</summary>
public sealed record SubmitSignatureRequest(string SignatureValue);
