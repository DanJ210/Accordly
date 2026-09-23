# Accordly — Scaffold Task List

Use this file as your build guide. Work through each step sequentially.
Each step is scoped small enough to paste directly into a coding assistant as a standalone prompt.
Check off each item only after it is implemented and validated. Commits are not required for completion.

> **Stack:** .NET 10 · ASP.NET Core Minimal APIs · Carter · EF Core 10 · SQL Server 2022
> **Test:** MSTest · Moq · Testcontainers.MsSql
> **Frontend:** Vue 3 · TypeScript · Pinia · Vue Router 4 · Tailwind CSS v4 · Tiptap v2
> **Infra:** Docker Compose · MinIO · MailHog

> **Progress note (2026-09-22):** Checked items below are implemented and validated; commit state does not determine completion. The solution uses the .NET 10 CLI's generated `Accordly.slnx` format. `dotnet build Accordly.slnx`, the `Accordly.Unit` test project, the Docker-backed auth lifecycle integration test, and the frontend type check/production build pass. Unchecked items are still missing or only partially implemented.

---

## Phase 1 — Repo & Solution

- [x] **1.1 — Initialize the monorepo**
  Create the root folder structure. Add `.gitignore` covering `bin/`, `obj/`, `.vs/`, `node_modules/`,
  `dist/`, `.env.local`, `appsettings.*.json` (except Development), `*.user`, JetBrains dirs,
  `.DS_Store`, `Thumbs.db`.
  Add `.editorconfig`: UTF-8, LF line endings, 4-space indent for C#, 2-space indent for
  TS/Vue/JSON/YAML, trim trailing whitespace, insert final newline.

- [x] **1.2 — Create the solution file**
  Run `dotnet new sln -n Accordly` at the repo root.
  Create empty class library stubs for all five `src/` projects and all three `tests/` projects
  using `dotnet new classlib` / `dotnet new mstest`.
  Add all eight projects to `Accordly.sln` via `dotnet sln add`.

- [x] **1.3 — Wire project references**
  Add the following project references:
  - `Accordly.Application` → `Accordly.Domain`, `Accordly.Contracts`
  - `Accordly.Infrastructure` → `Accordly.Domain`, `Accordly.Application`
  - `Accordly.Api` → `Accordly.Domain`, `Accordly.Application`, `Accordly.Infrastructure`, `Accordly.Contracts`
  - `Accordly.Unit` → `Accordly.Domain`, `Accordly.Application`
  - `Accordly.Integration` → `Accordly.Api`, `Accordly.Domain`
  Verify `dotnet build` passes with zero errors.

---

## Phase 2 — Domain Layer (`Accordly.Domain`)

- [x] **2.1 — Base entity**
  Create `Common/Entity.cs` with an abstract base class:
  - `Id` (Guid)
  - `CreatedAt` (DateTimeOffset, set in constructor via `DateTimeOffset.UtcNow`)

- [x] **2.2 — Enums**
  Create `Enums/AgreementStatus.cs`: `Draft`, `PendingSignatures`, `Active`, `Expired`, `Terminated`.
  Create `Enums/SignatoryRole.cs`: `Owner`, `Collaborator`, `Signer`, `Viewer`.

- [x] **2.3 — Core entities**
  Create one file per entity in `Entities/`. Each inherits from `Entity`.
  Properties must match the data model exactly:
  - `User`: `Email`, `DisplayName`, `PublicKey` (string), `OrganizationId?` (Guid?)
  - `Organization`: `Name`, `CreatedAt`
  - `Agreement`: `Title`, `Status` (AgreementStatus), `OwnerId`, `OrganizationId?`, `CurrentVersionId`, `UpdatedAt`, `ExpiresAt?`
  - `AgreementVersion`: `AgreementId`, `VersionNumber` (int), `Body` (string), `AuthorId`, `ChangeNote?`
  - `Signatory`: `AgreementId`, `UserId?`, `Email`, `Role` (SignatoryRole), `InviteToken?`, `SignedAt?`, `SignatureValue?`, `SignerIp?`, `VersionSignedId?`
  - `Attachment`: `AgreementId`, `VersionId`, `FileName`, `ContentType`, `StorageKey`, `FileSizeBytes` (long), `Sha256Hash`, `UploadedById`, `UploadedAt`
  - `AuditEvent`: `AgreementId`, `ActorId?`, `EventType`, `Payload?`, `OccurredAt`, `IpAddress?`

- [x] **2.4 — Domain events**
  Create `Events/AgreementStatusChangedEvent.cs`, `Events/AgreementVersionCreatedEvent.cs`,
  `Events/SignatorySignedEvent.cs`.
  Each is a simple `record` with relevant properties. No dispatch logic yet.

---

## Phase 3 — Contracts Layer (`Accordly.Contracts`)

- [x] **3.1 — Agreement DTOs**
  Create `Agreements/CreateAgreementRequest.cs` (record: `Title`, `ExpiresAt?`).
  Create `Agreements/AgreementResponse.cs` (record: `Id`, `Title`, `Status`, `OwnerId`, `CreatedAt`, `UpdatedAt`, `ExpiresAt?`).

- [x] **3.2 — Version DTOs**
  Create `Versions/CreateVersionRequest.cs` (record: `Body`, `ChangeNote?`).
  Create `Versions/AgreementVersionResponse.cs` (record: `Id`, `AgreementId`, `VersionNumber`, `Body`, `AuthorId`, `ChangeNote?`, `CreatedAt`).

- [x] **3.3 — Signatory DTOs**
  Create `Signatories/InviteSignatoryRequest.cs` (record: `Email`, `Role`).
  Create `Signatories/SignatoryResponse.cs` (record: `Id`, `Email`, `Role`, `SignedAt?`).
  Create `Signatories/SubmitSignatureRequest.cs` (record: `SignatureValue`).

- [x] **3.4 — Attachment DTOs**
  Create `Attachments/AttachmentResponse.cs` (record: `Id`, `FileName`, `ContentType`, `FileSizeBytes`, `Sha256Hash`, `UploadedAt`).

- [x] **3.5 — Auth DTOs**
  Create `Auth/RegisterRequest.cs` (record: `Email`, `DisplayName`, `Password`).
  Create `Auth/LoginRequest.cs` (record: `Email`, `Password`).
  Create `Auth/AuthResponse.cs` (record: `AccessToken`, `RefreshToken`, `ExpiresAt`).

---

## Phase 4 — Application Layer (`Accordly.Application`)

- [x] **4.1 — NuGet packages**
  Add to `Accordly.Application`: `MediatR`, `FluentValidation`,
  `FluentValidation.DependencyInjectionExtensions`.

- [x] **4.2 — Repository interfaces**
  Create `Common/Interfaces/IAgreementRepository.cs`: `GetByIdAsync`, `GetAllForUserAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`.
  Create `Common/Interfaces/IUserRepository.cs`: `GetByIdAsync`, `GetByEmailAsync`, `AddAsync`.
  Create `Common/Interfaces/IUnitOfWork.cs`: `SaveChangesAsync`.

- [x] **4.3 — Service interfaces**
  Create `Common/Interfaces/IStorageService.cs`: `UploadAsync(string key, Stream content, string contentType)`, `DownloadAsync(string key)`, `DeleteAsync(string key)`.
  Create `Common/Interfaces/IEmailService.cs`: `SendAsync(string to, string subject, string body)`.

- [x] **4.4 — Create Agreement command**
  Create `Agreements/Commands/CreateAgreement/CreateAgreementCommand.cs`
  (MediatR `IRequest<AgreementResponse>`): `Title`, `OwnerId`, `ExpiresAt?`.
  Create `Agreements/Commands/CreateAgreement/CreateAgreementCommandHandler.cs`:
  implement `IRequestHandler<CreateAgreementCommand, AgreementResponse>`.
  Use `IAgreementRepository` and `IUnitOfWork`. Map result to `AgreementResponse`.
  Create `Agreements/Commands/CreateAgreement/CreateAgreementCommandValidator.cs`:
  FluentValidation — `Title` required, max 250 chars.

- [x] **4.5 — Get Agreement query**
  Create `Agreements/Queries/GetAgreement/GetAgreementQuery.cs`
  (MediatR `IRequest<AgreementResponse?>`): `AgreementId`, `RequestingUserId`.
  Create `Agreements/Queries/GetAgreement/GetAgreementQueryHandler.cs`:
  fetch by id, return null if not found.

- [x] **4.6 — Application service registration**
  Create `ApplicationServiceExtensions.cs` with `AddApplication(this IServiceCollection)`.
  Register MediatR scanning `Accordly.Application` assembly.
  Register FluentValidation validators scanning the same assembly.
  Register pipeline behavior: `ValidationBehavior<TRequest, TResponse>` that runs validators
  before the handler and throws `ValidationException` on failure.

---

## Phase 5 — Infrastructure Layer (`Accordly.Infrastructure`)

- [x] **5.1 — NuGet packages**
  Add to `Accordly.Infrastructure`: `Microsoft.EntityFrameworkCore.SqlServer`,
  `Microsoft.EntityFrameworkCore.Tools`, `Microsoft.AspNetCore.Identity.EntityFrameworkCore`,
  `AWSSDK.S3`, `Hangfire.AspNetCore`, `Hangfire.SqlServer`, `QuestPDF`.

- [x] **5.2 — DbContext**
  Create `Persistence/AccordlyDbContext.cs` extending `IdentityDbContext<ApplicationUser>`.
  Add `DbSet<>` for: `Agreement`, `AgreementVersion`, `Signatory`, `Attachment`, `AuditEvent`, `Organization`.
  Override `OnModelCreating` to call `ApplyConfigurationsFromAssembly`.

- [x] **5.3 — Entity configurations**
  Create one `IEntityTypeConfiguration<T>` per entity in `Persistence/Configurations/`:
  - Enums stored as strings using `.HasConversion<string>()`.
  - `AgreementVersion.CreatedAt` immutable (no setter).
  - `AuditEvent.Payload` → `nvarchar(max)`.
  - `Signatory.SignerIp` → `nvarchar(45)`.
  - All Guid PKs use `ValueGeneratedNever()`.

- [x] **5.4 — Repository implementations**
  Create `Persistence/Repositories/AgreementRepository.cs` implementing `IAgreementRepository`.
  Create `Persistence/Repositories/UserRepository.cs` implementing `IUserRepository`.
  Create `Persistence/UnitOfWork.cs` implementing `IUnitOfWork`.

- [x] **5.5 — Storage service**
  Create `Storage/S3StorageService.cs` implementing `IStorageService` using `AWSSDK.S3`.
  Read `Storage:Endpoint`, `Storage:Bucket`, `Storage:AccessKey`, `Storage:SecretKey` from `IConfiguration`.

- [x] **5.6 — Email service**
  Create `Email/SmtpEmailService.cs` implementing `IEmailService` using `System.Net.Mail.SmtpClient`.
  Read `Email:Host` and `Email:Port` from `IConfiguration`.

- [x] **5.7 — Infrastructure service registration**
  Create `InfrastructureServiceExtensions.cs` with `AddInfrastructure(this IServiceCollection, IConfiguration)`.
  Register: `AccordlyDbContext` (SQL Server, `ConnectionStrings:DefaultConnection`),
  all repositories, unit of work, storage service, email service,
  Hangfire (SQL Server, same connection string, schema prefix `Hangfire`).

- [x] **5.8 — Initial EF Core migration**
  Run: `dotnet ef migrations add InitialCreate --project src/Accordly.Infrastructure --startup-project src/Accordly.Api`
  Commit the generated `Migrations/` folder.
  Verify the migration SQL creates all expected tables.

---

## Phase 6 — API Layer (`Accordly.Api`)

- [x] **6.1 — NuGet packages**
  Add to `Accordly.Api`: `Carter`, `MediatR`, `FluentValidation.DependencyInjectionExtensions`,
  `Microsoft.AspNetCore.Authentication.JwtBearer`, `Serilog.AspNetCore`, `Serilog.Sinks.File`,
  `Swashbuckle.AspNetCore`.

- [x] **6.2 — Configuration files**
  Create `appsettings.json` with sections: `Jwt`, `Storage`, `ServerKey`, `Email`, `Hangfire`, `ConnectionStrings`.
  Create `appsettings.Development.json` with local values:
  ```json
  {
    "ConnectionStrings": {
      "DefaultConnection": "Server=localhost,1433;Database=Accordly;User Id=sa;Password=Accordly_Dev1;TrustServerCertificate=True;"
    },
    "Jwt": {
      "Secret": "CHANGE_ME_TO_A_32_CHAR_MIN_SECRET",
      "Issuer": "accordly",
      "Audience": "accordly-client",
      "ExpiryMinutes": 60
    },
    "Storage": {
      "Endpoint": "http://localhost:9000",
      "Bucket": "accordly",
      "AccessKey": "minioadmin",
      "SecretKey": "minioadmin"
    },
    "Email": { "Host": "localhost", "Port": 1025 }
  }
  ```

- [x] **6.3 — Program.cs**
  Wire up in order: Serilog, `AddApplication()`, `AddInfrastructure()`, Carter, JWT Bearer auth,
  ASP.NET Core Identity, SignalR, Swagger (dev only), global exception handler middleware.
  Map: `app.MapCarter()`, `app.MapHub<AgreementsHub>("/hubs/agreements")`,
  Hangfire dashboard at `/hangfire` (dev only).

- [x] **6.4 — Global exception middleware**
  Create `Middleware/ExceptionHandlingMiddleware.cs`.
  `ValidationException` → 400 with field errors.
  `KeyNotFoundException` → 404.
  All others → 500 with generic message (no stack trace in production).

- [x] **6.5 — SignalR hub**
  Create `Hubs/AgreementsHub.cs` extending `Hub`.
  Add strongly-typed interface `IAgreementsHubClient` with methods:
  `VersionCreated`, `SignatoryUpdated`, `AgreementStatusChanged`, `AttachmentUploaded`.
  Hub shell only — no business logic yet.

- [x] **6.6 — Agreements module**
  Create `Modules/AgreementsModule.cs` implementing `ICarterModule`.
  Stub all five endpoints with correct HTTP method and route.
  Wire `POST /api/v1/agreements` → `CreateAgreementCommand` via MediatR.
  Wire `GET /api/v1/agreements/{id}` → `GetAgreementQuery` via MediatR.
  Remaining three return `Results.Ok("not yet implemented")` stubs.

- [x] **6.7 — Remaining route modules (stubs)**
  Create stub `ICarterModule` implementations for:
  `VersionsModule`, `SignatoriesModule`, `AttachmentsModule`, `ExportModule`, `AuthModule`, `AuditModule`.
  Each registers routes with the correct HTTP method and path but returns `Results.Ok("stub")`.
  Public routes (`/sign/{token}`, all `/auth/*`) must NOT have `RequireAuthorization()`.

- [x] **6.8a — Auth contracts and endpoint semantics**
  Add `RefreshTokenRequest` with a required `RefreshToken` value for refresh and logout.
  Keep `AuthResponse` as the access-token, refresh-token, and access-token-expiry response.
  Define endpoint behavior before implementation:
  - `POST /auth/register` returns `201 Created` with `AuthResponse`.
  - `POST /auth/login` returns `200 OK` with `AuthResponse`.
  - `POST /auth/refresh` returns `200 OK` with a rotated `AuthResponse`.
  - `POST /auth/logout` returns `204 No Content`; it is idempotent for an already-revoked token.
  - Duplicate registration and Identity password failures return `400`; invalid credentials and invalid,
    expired, or revoked refresh tokens return `401` without revealing which credential failed.

- [x] **6.8b — Persisted refresh-token model and migration**
  Add an Identity persistence model tied to `ApplicationUser` with: `Id`, `UserId`, `TokenHash`,
  `CreatedAt`, `ExpiresAt`, `RevokedAt?`, and `ReplacedByTokenId?`.
  Store only a SHA-256 hash of each cryptographically random token. Configure required lengths,
  indexes, foreign keys, and cascade behavior in EF Core. Add and inspect a migration that creates
  the refresh-token table without changing the domain agreement model.

- [x] **6.8c — Token service**
  Create `Services/TokenService.cs` and register it as scoped. Generate signed JWTs from the configured
  issuer, audience, secret, and expiry. Include stable user-id, email, and display-name claims; use the
  same user-id claim consumed by authenticated API routes. Generate 256-bit refresh tokens with a
  separately configured lifetime. Implement persisted issue, validation, single-use rotation, and
  revocation operations with cancellation-token support.

- [x] **6.8d — Identity and auth service registration**
  Complete Identity registration for `UserManager<ApplicationUser>` and `SignInManager<ApplicationUser>`
  without enabling cookie authentication. Configure unique emails and an explicit password policy.
  Validate JWT and refresh-token configuration at startup so missing or weak signing secrets fail fast
  outside the documented local-development setup.

- [x] **6.8e — Registration and login endpoints**
  Replace the register and login stubs. Registration creates `ApplicationUser` and the domain `User`
  with the same Guid and normalized email/display name, and must not leave either record orphaned if the
  operation fails. Do not generate and discard a private signing key on the server: initialize the current
  domain `PublicKey` as empty and document key enrollment as separate work unless the registration contract
  is deliberately extended for a client-generated public key. Login validates the password through Identity
  without disclosing account existence. Both endpoints issue an access/refresh token pair.

- [x] **6.8f — Refresh and logout endpoints**
  Replace the refresh and logout stubs. Refresh atomically revokes the presented token and links it to
  its replacement before returning a new pair. Logout revokes the presented refresh token; existing
  access tokens remain valid only until their short expiry. Keep all four `/auth/*` routes public so a
  caller with an expired access token can still rotate or revoke a refresh token.

- [x] **6.8g — Auth tests and documentation validation**
  Add focused tests for JWT claims/expiry, refresh-token hashing, rotation/replay rejection, revocation,
  duplicate registration, invalid login, and successful register/login/refresh/logout responses.
  Verify persisted Identity and domain users share an ID. Update `docs/api.md`, `docs/data-model.md`,
  `docs/architecture.md`, and configuration documentation with the implemented contracts, persistence
  shape, security behavior, and settings. Run the narrow auth tests, then `dotnet build Accordly.slnx`.

- [x] **6.9a — Authenticated user identity resolution**
  Add one API-owned helper or service that extracts the stable user ID emitted by `TokenService` from
  authenticated claims. Reject a missing or malformed claim consistently. Remove `Guid.Empty`, query-string
  owner IDs, and request-controlled user IDs from authorization and ownership decisions. Implemented with
  `ICurrentUserService`, `401 Unauthorized` handling, agreement route propagation, and focused unit tests.

- [ ] **6.9b — Agreement authorization policy** *(partial: shared policy and detail-read enforcement implemented; mutation enforcement awaits mutation handlers)*
  Define owner, collaborator, signer, and viewer access for agreement reads and mutations. Enforce the policy
  in application handlers or a shared authorization service rather than duplicating it in Carter lambdas.
  Return a non-disclosing response for users who cannot access an agreement.

- [x] **6.10a — Agreement list and detail queries**
  Implement authenticated agreement listing and detail retrieval. Scope list results to agreements visible
  to the acting user, map entities to Contracts, and include the current-version data required by the detail UI.
  The list query scopes owners and registered collaborator, signer, and viewer memberships; detail reads return
  the current immutable version when present. Focused query-handler tests pass.

- [x] **6.10b — Agreement creation**
  Create agreements with the authenticated user as owner, return `201 Created` with a resource-specific
  `Location`, and preserve owner access through the agreement's owner membership (`OwnerId`). The command,
  validator, handler, and focused unit tests are implemented and passing.

- [ ] **6.10c — Agreement update and deletion** *(partial: update/delete handlers, validation, authorization, and focused tests implemented; audit recording remains outstanding)*
  Replaced the PATCH and DELETE stubs with validated title, expiry, and lifecycle changes plus owner-only deletion.
  Status-transition rules and immutable versions are preserved. Complete the item when update/delete audit events
  are recorded and covered by tests.

- [x] **6.11a — Version creation workflow**
  Added repository/application support for creating the next immutable `AgreementVersion` transactionally,
  assigning its per-agreement version number, setting `CurrentVersionId`, and creating the documented event.

- [x] **6.11b — Version read and list endpoints**
  Implemented authorized version listing and retrieval with Contract responses. The handlers verify access and
  match the version to the agreement ID so a version cannot be read through a different agreement route.

- [x] **6.11c — Version diff endpoint**
  Implemented an authenticated `/agreements/{id}/versions/diff?from=&to=` query that validates both version IDs,
  ensures both versions belong to the same accessible agreement, and returns a transport-safe diff response for the
  frontend viewer. Includes tests for successful diffing and missing/mismatched version pairs.

- [x] **6.12a — Signatory management**
  Implemented authorized signatory listing, invitation, and removal of unsigned signatories. Role values are
  validated, guest invite tokens are generated securely, and tokens are stored as SHA-256 hashes rather than raw tokens.
  The flow is covered by focused unit tests for invite, list, and delete semantics.

- [x] **6.12b — Registered-user signing**
  Implemented signing for authenticated signatories against the agreement's current version. The command persists
  the signature value, timestamp, signer identity, version ID, and IP address where available, and rejects replay by
  refusing already-signed signatories. Focused unit coverage validates the signed-version record flow.

- [x] **6.12c — Guest signing lifecycle**
  Implemented public token resolution and submission with one-time token enforcement. The app resolves a hashed
  guest token, records the signature, clears the single-use token, and prevents replay by checking signed state.
  The flow is covered by focused unit tests for valid guest-signature submission.

- [ ] **6.12d — Signature-driven status and notifications** *(partial implementation: the agreement now activates when all required signer records for the current version are signed; audit, SignalR, and notification email remain outstanding)*
  Activate an agreement only when all required signatures for the same version are collected. Record audit events,
  publish `SignatoryUpdated` and `AgreementStatusChanged`, and send configured notification email where applicable.

- [ ] **6.13a — Attachment upload and integrity**
  Implement authorized multipart upload for a specific agreement version. Stream to `IStorageService`, compute and
  persist SHA-256, validate metadata and size limits, and compensate for storage/database failures.

- [ ] **6.13b — Attachment list and download**
  Implement authorized metadata listing and streamed download with safe content headers. Verify the attachment
  belongs to the routed agreement and do not expose storage keys.

- [ ] **6.13c — Attachment deletion**
  Implement authorized deletion with storage/database consistency and audit recording. Define whether attachments
  on a signed or active version may be removed, then enforce that rule in application code.

- [ ] **6.14a — Structured JSON export**
  Export the accessible agreement's versions, signatures, attachment manifest, and audit metadata using explicit
  export Contracts. Keep persistence entities and sensitive token/storage fields out of the response.

- [ ] **6.14b — Court-ready PDF export**
  Generate the documented PDF bundle with QuestPDF, including agreement content, version history, signature and
  attachment manifests, and verifiability metadata. Stream it with stable download headers and focused tests.

- [ ] **6.14c — Server signing and key validation**
  Validate configured server signing-key material at startup and digitally sign exports. Document key rotation and
  verification behavior without exposing private material.

- [ ] **6.15a — Append-only audit recording**
  Add an application abstraction for recording meaningful agreement, version, signature, attachment, and export
  activity. Prevent ordinary update/delete paths for audit rows and capture actor/IP metadata consistently.

- [ ] **6.15b — Audit query endpoint**
  Implement an authorized, stable, paginated audit endpoint with explicit ordering and Contract responses.
  Add tests for access control, page boundaries, and append-only ordering.

- [ ] **6.15c — SignalR publication and authorization**
  Publish `VersionCreated`, `SignatoryUpdated`, `AgreementStatusChanged`, and `AttachmentUploaded` after committed
  state changes. Authorize agreement-group membership and avoid broadcasting sensitive data globally.

---

## Phase 7 — Test Projects

- [x] **7.1 — Unit: AgreementStatus transition tests**
  Create `Domain/AgreementStatusTests.cs` in `Accordly.Unit`.
  Use `[TestClass]` and `[TestMethod]`. Assert valid and invalid status transitions
  (e.g., cannot move from `Active` back to `Draft`).

- [x] **7.2 — Unit: CreateAgreementCommandValidator tests**
  Create `Application/CreateAgreementCommandValidatorTests.cs`.
  Test: empty title fails, title over 250 chars fails, valid command passes.

- [x] **7.3 — Unit: CreateAgreementCommandHandler tests**
  Create `Application/CreateAgreementCommandHandlerTests.cs`.
  Mock `IAgreementRepository` and `IUnitOfWork` with Moq.
  Assert handler calls `AddAsync` and `SaveChangesAsync` and returns a populated `AgreementResponse`.

- [ ] **7.4a — Agreement integration fixture**
  Reuse `AccordlyWebApplicationFactory` and the class-scoped SQL Server Testcontainer pattern from the auth lifecycle
  tests. Add helpers that create an authenticated Identity/domain user pair without bypassing production JWT validation.

- [ ] **7.4b — Agreement authentication boundary**
  Assert unauthenticated agreement requests return `401`. Verify client-supplied owner/user identifiers cannot
  override the authenticated user identity.

- [ ] **7.4c — Authenticated agreement creation**
  POST `/api/v1/agreements`, assert `201 Created`, a resource-specific `Location`, and a valid `AgreementResponse`.
  Verify the database owner ID matches the JWT subject and all initial agreement state is persisted.

- [ ] **7.4d — Agreement detail authorization**
  Verify an owner or permitted participant can retrieve agreement detail and an unrelated user receives the chosen
  non-disclosing response. Assert both status codes and response bodies.

- [ ] **7.5a — E2E project convention**
  Keep `Accordly.E2E/README.md` accurate, remove the generated passing placeholder, and add one explicitly ignored
  MSTest placeholder that names the first planned Playwright workflow.

- [ ] **7.5b — Playwright E2E infrastructure**
  Add browser installation and application-lifecycle setup only when the first real E2E workflow is implemented.
  Document required API/frontend/database prerequisites and keep E2E execution separate from fast unit tests.

---

## Phase 8 — Docker Compose & Infrastructure

- [x] **8.1 — docker-compose.yml**
  Three services on shared `accordly-net` bridge network:
  - `sqlserver`: `mcr.microsoft.com/mssql/server:2022-latest`, port `1433:1433`,
    env `ACCEPT_EULA=Y`, `SA_PASSWORD=Accordly_Dev1`, `MSSQL_PID=Developer`, volume `sqldata`.
  - `minio`: `minio/minio:latest`, ports `9000:9000` and `9001:9001`,
    command `server /data --console-address ":9001"`,
    env `MINIO_ROOT_USER=minioadmin`, `MINIO_ROOT_PASSWORD=minioadmin`, volume `miniodata`.
  - `mailhog`: `mailhog/mailhog:latest`, ports `1025:1025` and `8025:8025`.
  Named volumes: `sqldata`, `miniodata`.

- [x] **8.2 — Root documentation stack alignment**
  `README.md` describes SQL Server 2022, SQL Server data types and connection strings,
  and MSTest with `Testcontainers.MsSql` consistently with the implemented stack.

---

## Phase 9 — Frontend Bootstrap

- [x] **9.1 — Vite + Vue 3 scaffold**
  Run: `pnpm create vite frontend --template vue-ts`
  Install runtime deps:
  ```
  pnpm add vue-router@4 pinia axios @microsoft/signalr @tiptap/vue-3 @tiptap/starter-kit diff2html pdfjs-dist
  ```
  Install dev deps:
  ```
  pnpm add -D tailwindcss @tailwindcss/vite vitest @vue/test-utils @testing-library/vue playwright
  ```

- [x] **9.2 — Config files**
  `vite.config.ts`: `@` alias → `src/`, Tailwind CSS Vite plugin, inline Vitest config.
  `tsconfig.json`: strict mode, `@` path alias.
  `tailwind.config.ts`: content paths `["./src/**/*.{vue,ts}"]`.
  `.env.example`: `VITE_API_BASE_URL`, `VITE_SIGNALR_HUB_URL`.

- [x] **9.3a — Base router and authentication guard**
  Register `/`, `/agreements`, `/agreements/:id`, `/sign/:token`, `/login`, and `/register` with authenticated
  route metadata where required. Redirect unauthenticated users to `/login` through the auth store.

- [ ] **9.3b — Agreement editor route**
  Add `/agreements/:id/edit` with authentication metadata when `AgreementEditorPage` exists. Preserve the intended
  destination during login so an authenticated user can return to the editor route.

- [x] **9.4 — Pinia stores**
  `src/stores/auth.ts`: state `user`, `token`. Actions `login()`, `logout()`, `register()`.
  Persist `token` to `localStorage`.
  `src/stores/agreements.ts`: state `agreements[]`, `currentAgreement`.
  Actions `fetchAll()`, `fetchById(id)`, `create(payload)`, `update(id, payload)`.

- [x] **9.5 — Composables**
  `src/composables/useApi.ts`: Axios instance with `baseURL` from `VITE_API_BASE_URL`.
  Request interceptor: attach `Authorization: Bearer <token>` from auth store.
  Response interceptor: redirect to `/login` on 401.
  `src/composables/useSignalR.ts`: connect to `VITE_SIGNALR_HUB_URL/hubs/agreements`
  using JWT from auth store. Expose `on(event, handler)` and `off(event, handler)`. Auto-reconnect.

- [x] **9.6 — Layouts**
  `src/layouts/DefaultLayout.vue`: top navbar (Accordly wordmark, nav links, user avatar dropdown
  with Logout). `<slot />` for page content.
  `src/layouts/GuestLayout.vue`: centered card layout, no navigation.

- [x] **9.7 — App shell**
  `src/main.ts`: create app, install Pinia and Router, mount to `#app`.
  `src/App.vue`: wrap `<RouterView />` in DefaultLayout or GuestLayout based on route meta.

---

## Phase 10 — Frontend Features

- [ ] **10.1a — Agreement list presentation**
  Complete `AgreementList.vue` with Title, Status, Created, and Actions columns. Use distinct badge styles for
  Draft, PendingSignatures, Active, Expired, and Terminated, and link each agreement to its detail route.

- [ ] **10.1b — Agreement detail presentation**
  Complete `AgreementDetail.vue` with title, status, expiry, and current version. Provide named composition slots
  for signatories, attachments, and export actions without nesting page sections in decorative cards.

- [ ] **10.1c — Agreements composable**
  Add `composables/useAgreements.ts` around store operations with stable loading, error, retry, and cancellation
  behavior. Re-export the components and composable from the feature barrel.

- [x] **10.2a — Signatory list presentation**
  `SignatoryList.vue` displays email, role, and signed/pending state with the signature timestamp when available.

- [ ] **10.2b — Guest agreement presentation**
  Display the resolved, read-only agreement title, body, version, and signer identity context without exposing the
  guest token beyond the route/API request.

- [ ] **10.2c — Guest signature submission**
  Make `GuestSignForm.vue` submit to `POST /api/v1/sign/:token`, prevent duplicate submission, and show success,
  invalid/expired token, validation, and retryable failure states. Re-export the completed feature surface.

- [x] **10.3a — Attachment list presentation**
  `AttachmentList.vue` displays file name, size, upload date, and a download action.

- [ ] **10.3b — Attachment selection and drag-and-drop**
  Add accessible file selection and drag-and-drop behavior with selected-file metadata, size/type validation,
  replacement, and removal before upload.

- [ ] **10.3c — Attachment upload integration**
  POST multipart data to `/api/v1/agreements/:id/attachments`, report progress, prevent duplicate submission,
  refresh the attachment list on success, and surface validation/network failures. Re-export the completed feature.

- [x] **10.4 — Export feature**
  `ExportButton.vue`: button calls `GET /api/v1/agreements/:id/export/pdf`,
  triggers browser file download.
  `index.ts` barrel.

---

## Phase 11 — Frontend Pages

- [ ] **11.1a — Dashboard identity and recent agreements**
  Greet the authenticated user by display name and render the five most recently updated accessible agreements
  through `AgreementList`, including loading, empty, and failure states.

- [ ] **11.1b — Dashboard create action**
  Add a clear "Create Agreement" action that navigates to the agreement creation workflow.

- [ ] **11.2a — Paginated agreements page**
  Render the complete accessible agreement list with stable pagination, loading, empty, retry, and responsive states.
  Keep page state in the URL when practical.

- [ ] **11.2b — Agreement creation modal**
  Add an accessible modal with required title and optional expiry, field validation, submit/error states, and focus
  restoration. Call `create()` and navigate to the returned detail route only after success.

- [ ] **11.3a — Agreement detail composition**
  Fetch by route ID and compose `AgreementDetail`, `SignatoryList`, `AttachmentList`, `AttachmentUpload`, and
  `ExportButton`. Add an Edit action to `/agreements/:id/edit` plus loading, not-found, and access-denied states.

- [ ] **11.3b — Agreement detail child-data loading**
  Load signatories, attachments, and version summary through feature-owned API/state boundaries. Reconcile child
  loading and failure states without discarding successfully loaded agreement data.

- [ ] **11.3c — Agreement detail SignalR updates**
  Subscribe to agreement-scoped events, update or refetch the affected state without duplication, and unregister
  handlers when the route changes or unmounts. Cover reconnect behavior.

- [ ] **11.4a — Agreement version API workflow**
  Complete and validate backend version creation/read behavior from Phase 6.11 before connecting the editor.

- [ ] **11.4b — Tiptap agreement editor**
  Add `AgreementEditorPage` with StarterKit, load the current immutable version into editable state, and preserve
  unsaved content across ordinary component updates. Provide accessible editing and change-note controls.

- [ ] **11.4c — Save-version interaction**
  POST the editor body and optional change note to `/api/v1/agreements/:id/versions`, prevent duplicate saves,
  show validation/network errors, and navigate to detail only after a successful new-version response.

- [ ] **11.5a — Guest token resolution page**
  Resolve `GET /api/v1/sign/:token` and render distinct loading, invalid, expired, revoked, already-used, and success
  states inside `GuestLayout` without entering authenticated application navigation.

- [ ] **11.5b — Guest signing composition**
  Pass the resolved agreement and token-bound signer context into `GuestSignForm`, handle submission completion,
  and prevent stale agreement content from being signed.

- [ ] **11.6a — Login page**
  Add email/password validation, dispatch `login()`, show field/general errors without disclosing account existence,
  and redirect to the preserved authenticated destination on success.

- [ ] **11.6b — Registration page**
  Add email/display-name/password validation, dispatch `register()`, display Identity validation errors safely,
  and follow the implemented registration response flow rather than assuming a second login is required.

---

## Phase 12 — Frontend Tests

- [ ] **12.0 — Vitest infrastructure**
  Add `test` and optional watch scripts to `frontend/package.json`, ensure the `jsdom` environment is directly
  available, and create shared setup for DOM cleanup, Pinia, router, localStorage, and HTTP mocks. Verify an empty
  or smoke suite runs through `pnpm test` before adding behavioral tests.

- [ ] **12.1 — useApi composable tests**
  `tests/composables/useApi.spec.ts`
  Mock Axios. Assert Bearer header is attached when token exists.
  Assert 401 response triggers redirect to `/login` without creating redirect loops on public auth routes.

- [ ] **12.2 — Auth store tests**
  `tests/stores/auth.spec.ts`
  Assert `login()` sets `token` in state and writes to `localStorage`.
  Assert `logout()` clears both.
  Assert `register()` calls the correct API endpoint and handles the implemented `AuthResponse` semantics.

- [ ] **12.3 — Router authorization tests**
  Verify protected routes redirect without a token, public auth/guest routes remain accessible, and the preserved
  destination is restored after login.

- [ ] **12.4 — Agreement workflow component tests**
  Cover agreement creation validation/navigation, detail loading states, attachment upload progress/failure, guest
  signing terminal states, and SignalR handler cleanup as those workflows are implemented.

---

## Completion Checklist

Before marking the scaffold complete, verify all of the following:

- [ ] `dotnet build` — zero errors, zero warnings
- [ ] `dotnet test` — all MSTest tests pass (unit + integration)
- [ ] `docker compose up -d` — all three services start cleanly
- [x] `dotnet ef database update` — migration applies against local SQL Server container
- [ ] API starts and Swagger UI loads at `https://localhost:5001/swagger`
- [ ] `pnpm exec vue-tsc --noEmit` — passes with strict mode
- [ ] `pnpm test` — all Vitest tests pass
- [ ] Frontend dev server starts at `http://localhost:5173` and login page renders
