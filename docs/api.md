# API Reference

All API routes are prefixed with `/api/v1`.

Unless marked public, routes require a JWT bearer token. The API uses Carter modules and returns JSON contracts from `Accordly.Contracts`.

Authenticated agreement routes resolve the acting user from the JWT `NameIdentifier` claim emitted by
`TokenService`. Missing, malformed, empty, or unauthenticated identity claims return `401 Unauthorized`;
client-supplied owner or user identifiers are not used for ownership decisions.

Agreement detail reads use the persisted membership policy: owners, collaborators, signers, and viewers may read;
owners and collaborators may mutate. Missing and unauthorized agreement details are returned as the same `404 Not Found`
result. Missing or unauthorized agreement updates and deletes are also returned as `404 Not Found`.

## Agreements

| Method | Route | Access | Purpose |
| --- | --- | --- | --- |
| GET | `/agreements` | Authenticated | List agreements for the current user |
| POST | `/agreements` | Authenticated | Create an agreement; returns `201 Created` |
| GET | `/agreements/{id}` | Authenticated | Read agreement details and current version |
| PATCH | `/agreements/{id}` | Authenticated | Update title, status, or expiry; owners and collaborators may mutate |
| DELETE | `/agreements/{id}` | Authenticated | Remove an agreement for its owner; returns `204 No Content` |

PATCH accepts an optional `title`, `expiresAt`, and `status` body. At least one field is required;
title is limited to 250 characters and status values must follow the lifecycle transition rules.
Agreement versions are not changed by metadata or lifecycle updates.

## Versions

| Method | Route | Access | Purpose |
| --- | --- | --- | --- |
| GET | `/agreements/{id}/versions` | Authenticated | List versions |
| POST | `/agreements/{id}/versions` | Owner or collaborator | Create an immutable version; returns `201 Created` |
| GET | `/agreements/{id}/versions/{versionId}` | Authenticated | Read one version |
| GET | `/agreements/{id}/versions/diff?from=&to=` | Authenticated | Compare two versions |

Version creation uses the authenticated caller as its author and returns the same non-disclosing `404 Not Found`
result for missing or inaccessible agreements. The diff route validates both version IDs, requires the versions to belong to the same accessible agreement,
returns a `404 Not Found` for missing or mismatched versions, and returns a response with the source/target version
metadata and a transport-safe list of field-level changes.

## Signatories

| Method | Route | Access | Purpose |
| --- | --- | --- | --- |
| GET | `/agreements/{id}/signatories` | Authenticated | List signatories |
| POST | `/agreements/{id}/signatories` | Authenticated | Invite a signatory |
| DELETE | `/agreements/{id}/signatories/{sigId}` | Authenticated | Remove an unsigned signatory |
| POST | `/agreements/{id}/signatories/{sigId}/sign` | Authenticated | Submit a signature |
| GET | `/sign/{token}` | Public | Resolve a guest invite |
| POST | `/sign/{token}` | Public | Submit a guest signature |

## Attachments

| Method | Route | Access | Purpose |
| --- | --- | --- | --- |
| GET | `/agreements/{id}/attachments` | Authenticated | List attachments |
| POST | `/agreements/{id}/attachments` | Authenticated | Upload multipart content |
| GET | `/agreements/{id}/attachments/{attachId}` | Authenticated | Download an attachment |
| DELETE | `/agreements/{id}/attachments/{attachId}` | Authenticated | Delete an attachment |

## Export and Audit

| Method | Route | Access | Purpose |
| --- | --- | --- | --- |
| GET | `/agreements/{id}/export/pdf` | Authenticated | Generate a court-ready PDF bundle |
| GET | `/agreements/{id}/export/json` | Authenticated | Export the agreement record as JSON |
| GET | `/agreements/{id}/audit` | Authenticated | Read the agreement audit log |

## Authentication

| Method | Route | Access | Purpose |
| --- | --- | --- | --- |
| POST | `/auth/register` | Public | Register an account and receive an access/refresh token pair |
| POST | `/auth/login` | Public | Issue an access and refresh token |
| POST | `/auth/refresh` | Public | Rotates a refresh token; request body requires a `refreshToken` string |
| POST | `/auth/logout` | Public | Revoke a refresh token; request body requires a `refreshToken` string |

Registration and login persist only the SHA-256 hash of the returned refresh token. Refresh tokens are
single-use: a successful refresh revokes the submitted token, records its replacement, and returns a new
access/refresh pair. Missing, expired, revoked, unknown, or replayed refresh tokens return `401 Unauthorized`.
Logout returns `204 No Content` and is idempotent for an unknown or already-revoked token.

## SignalR

The hub is available at `/hubs/agreements`.

Server-to-client events:

- `VersionCreated`
- `SignatoryUpdated`
- `AgreementStatusChanged`
- `AttachmentUploaded`

## Error Handling

- Validation errors return HTTP 400 with field-level errors.
- Missing resources return HTTP 404.
- Unexpected errors return HTTP 500. Production responses must not expose stack traces.
