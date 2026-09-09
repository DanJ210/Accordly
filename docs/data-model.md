# Data Model

All persisted domain entities use Guid identifiers and UTC `DateTimeOffset` timestamps. EF Core maps them to SQL Server.

## User

- `Id`: Guid primary key
- `Email`: up to 320 characters
- `DisplayName`: up to 100 characters
- `PublicKey`: base64 Ed25519 public key
- `OrganizationId`: optional Guid

## Organization

- `Id`: Guid primary key
- `Name`: organization name
- `CreatedAt`: creation timestamp

## Agreement

- `Id`: Guid primary key
- `Title`: required, maximum 250 characters
- `Status`: `Draft`, `PendingSignatures`, `Active`, `Expired`, or `Terminated`; stored as a string
- `OwnerId`: owning user Guid
- `OrganizationId`: optional organization Guid
- `CurrentVersionId`: current version Guid
- `CreatedAt` and `UpdatedAt`: timestamps
- `ExpiresAt`: optional timestamp

## AgreementVersion

- `Id`: Guid primary key
- `AgreementId`: parent agreement Guid
- `VersionNumber`: sequence within an agreement
- `Body`: full Markdown agreement body
- `AuthorId`: author user Guid
- `ChangeNote`: optional note, maximum 500 characters
- `CreatedAt`: immutable creation timestamp

## Signatory

- `Id`: Guid primary key
- `AgreementId`: parent agreement Guid
- `UserId`: optional registered user Guid
- `Email`: signatory email
- `Role`: `Owner`, `Collaborator`, `Signer`, or `Viewer`; stored as a string
- `InviteToken`: optional guest token
- `SignedAt`: optional signature timestamp
- `SignatureValue`: optional base64 Ed25519 signature
- `SignerIp`: optional IP address, maximum 45 characters
- `VersionSignedId`: optional signed version Guid

## Attachment

- `Id`: Guid primary key
- `AgreementId`: parent agreement Guid
- `VersionId`: pinned version Guid
- `FileName`: original name
- `ContentType`: MIME type
- `StorageKey`: S3/MinIO object key
- `FileSizeBytes`: long
- `Sha256Hash`: required 64-character hexadecimal hash
- `UploadedById`: uploader user Guid
- `UploadedAt`: upload timestamp

## AuditEvent

- `Id`: Guid primary key
- `AgreementId`: parent agreement Guid
- `ActorId`: optional user Guid
- `EventType`: maximum 100 characters
- `Payload`: optional JSON stored as `nvarchar(max)`
- `OccurredAt`: occurrence timestamp
- `IpAddress`: optional IP address

## Identity

ASP.NET Core Identity stores authentication users, roles, claims, logins, tokens, and related tables through `ApplicationUser` and `AccordlyDbContext`. The domain `User` record is kept separately for agreement-facing profile data until the identity/domain relationship is completed.

## Domain Events

The domain defines immutable event records without dispatch logic yet:

- `AgreementStatusChangedEvent`: agreement identifier, previous and new status, and occurrence timestamp
- `AgreementVersionCreatedEvent`: agreement, version, and author identifiers, version number, and creation timestamp
- `SignatorySignedEvent`: agreement and signatory identifiers, optional signed version identifier, and signing timestamp
