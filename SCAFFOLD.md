# Accordly — Scaffold Task List

Use this file as your build guide. Work through each step sequentially.
Each step is scoped small enough to paste directly into a coding assistant as a standalone prompt.
Check off each item as it is completed and committed.

> **Stack:** .NET 10 · ASP.NET Core Minimal APIs · Carter · EF Core 10 · SQL Server 2022
> **Test:** MSTest · Moq · Testcontainers.MsSql
> **Frontend:** Vue 3 · TypeScript · Pinia · Vue Router 4 · Tailwind CSS v4 · Tiptap v2
> **Infra:** Docker Compose · MinIO · MailHog

> **Progress note (2026-09-14):** Checked items below are implemented in the working tree but have not been committed. The solution uses the .NET 10 CLI's generated `Accordly.slnx` format. `dotnet build Accordly.slnx`, the `Accordly.Unit` test project, and the frontend type check/production build pass. Unchecked items are still missing or only partially implemented. Phase 6.8 has been reviewed and split into dependency-ordered, independently verifiable tasks; its existing JWT and Identity foundation remains partial.

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

- [ ] **6.8e — Registration and login endpoints**
  Replace the register and login stubs. Registration creates `ApplicationUser` and the domain `User`
  with the same Guid and normalized email/display name, and must not leave either record orphaned if the
  operation fails. Do not generate and discard a private signing key on the server: initialize the current
  domain `PublicKey` as empty and document key enrollment as separate work unless the registration contract
  is deliberately extended for a client-generated public key. Login validates the password through Identity
  without disclosing account existence. Both endpoints issue an access/refresh token pair.

- [ ] **6.8f — Refresh and logout endpoints**
  Replace the refresh and logout stubs. Refresh atomically revokes the presented token and links it to
  its replacement before returning a new pair. Logout revokes the presented refresh token; existing
  access tokens remain valid only until their short expiry. Keep all four `/auth/*` routes public so a
  caller with an expired access token can still rotate or revoke a refresh token.

- [ ] **6.8g — Auth tests and documentation validation**
  Add focused tests for JWT claims/expiry, refresh-token hashing, rotation/replay rejection, revocation,
  duplicate registration, invalid login, and successful register/login/refresh/logout responses.
  Verify persisted Identity and domain users share an ID. Update `docs/api.md`, `docs/data-model.md`,
  `docs/architecture.md`, and configuration documentation with the implemented contracts, persistence
  shape, security behavior, and settings. Run the narrow auth tests, then `dotnet build Accordly.slnx`.

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

- [ ] **7.4 — Integration: POST /agreements returns 201**
  Add `Testcontainers.MsSql` and `Microsoft.AspNetCore.Mvc.Testing` to `Accordly.Integration`.
  Create `AccordlyWebApplicationFactory.cs` extending `WebApplicationFactory<Program>`.
  Override `ConfigureWebHost` to replace `ConnectionStrings:DefaultConnection` with the
  Testcontainers SQL Server connection string.
  Create `Agreements/CreateAgreementTests.cs`:
  - `[ClassInitialize]` starts the SQL Server container and applies migrations.
  - `[ClassCleanup]` disposes the container.
  - One `[TestMethod]`: authenticate, POST to `/api/v1/agreements`, assert `201 Created`
    and a valid `AgreementResponse` body.

- [ ] **7.5 — E2E: Placeholder**
  In `Accordly.E2E`, add `README.md` explaining Playwright E2E tests live here.
  Add one `[TestClass]` with one `[Ignore][TestMethod]` placeholder.

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

- [ ] **9.3 — Router**
  Create `src/router/index.ts` with routes:
  - `/` → `DashboardPage` (auth guard)
  - `/agreements` → `AgreementsPage` (auth guard)
  - `/agreements/:id` → `AgreementDetailPage` (auth guard)
  - `/agreements/:id/edit` → `AgreementEditorPage` (auth guard)
  - `/sign/:token` → `GuestSignPage` (no guard)
  - `/login` → `LoginPage` (no guard)
  - `/register` → `RegisterPage` (no guard)
  Auth guard reads from auth store; redirects unauthenticated users to `/login`.

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

- [ ] **10.1 — Agreements feature**
  `AgreementList.vue`: table — Title, Status badge, Created, Actions.
  Badge colors: Draft=gray, PendingSignatures=yellow, Active=green, Expired=red, Terminated=slate.
  `AgreementDetail.vue`: title, status badge, expiry, version number. Slots for signatories and attachments.
  `composables/useAgreements.ts`: wraps store actions with `loading` and `error` state.
  `index.ts` barrel re-exporting all of the above.

- [x] **10.2 — Signatures feature**
  `SignatoryList.vue`: email, role, signed/pending badge with timestamp if signed.
  `GuestSignForm.vue`: read-only agreement body, submit triggers `POST /api/v1/sign/:token`,
  shows confirmation on success.
  `index.ts` barrel.

- [x] **10.3 — Attachments feature**
  `AttachmentList.vue`: file name, size, upload date, download link.
  `AttachmentUpload.vue`: drag-and-drop zone, POSTs `multipart/form-data` to
  `/api/v1/agreements/:id/attachments`, shows upload progress.
  `index.ts` barrel.

- [x] **10.4 — Export feature**
  `ExportButton.vue`: button calls `GET /api/v1/agreements/:id/export/pdf`,
  triggers browser file download.
  `index.ts` barrel.

---

## Phase 11 — Frontend Pages

- [ ] **11.1 — DashboardPage**
  Greeting with user display name. Recent agreements via `AgreementList` (5 most recent).
  "Create Agreement" button linking to `/agreements`.

- [ ] **11.2 — AgreementsPage**
  Full `AgreementList` with pagination. "New Agreement" button opens a modal —
  title input + optional expiry date. On submit, `create()` from store, navigate to detail.

- [ ] **11.3 — AgreementDetailPage**
  Fetches by `:id` on mount. Composes `AgreementDetail`, `SignatoryList`, `AttachmentList`
  (with `AttachmentUpload`), `ExportButton`. "Edit" links to `/agreements/:id/edit`.
  Listens to SignalR events via `useSignalR` to reactively update signatories and status.

- [ ] **11.4 — AgreementEditorPage**
  Loads current version body into a Tiptap editor (StarterKit).
  "Save Version" POSTs to `/api/v1/agreements/:id/versions` with body as Markdown
  and optional change note. Navigates back to detail on success.

- [ ] **11.5 — GuestSignPage**
  Calls `GET /api/v1/sign/:token` to resolve agreement. Renders `GuestSignForm` inside
  `GuestLayout`. Shows clear error state for invalid or expired tokens.

- [ ] **11.6 — LoginPage / RegisterPage**
  `LoginPage.vue`: email + password, dispatches `login()`, redirects to `/` on success.
  `RegisterPage.vue`: email + display name + password, dispatches `register()`,
  redirects to `/login` on success. Both show field-level validation errors.

---

## Phase 12 — Frontend Tests

- [ ] **12.1 — useApi composable test**
  `tests/composables/useApi.spec.ts`
  Mock Axios. Assert Bearer header is attached when token exists.
  Assert 401 response triggers redirect to `/login`.

- [ ] **12.2 — Auth store test**
  `tests/stores/auth.spec.ts`
  Assert `login()` sets `token` in state and writes to `localStorage`.
  Assert `logout()` clears both.
  Assert `register()` calls the correct API endpoint.

---

## Completion Checklist

Before marking the scaffold complete, verify all of the following:

- [ ] `dotnet build` — zero errors, zero warnings
- [ ] `dotnet test` — all MSTest tests pass (unit + integration)
- [ ] `docker compose up -d` — all three services start cleanly
- [x] `dotnet ef database update` — migration applies against local SQL Server container
- [ ] API starts and Swagger UI loads at `https://localhost:5001/swagger`
- [ ] `pnpm tsc --noEmit` — passes with strict mode
- [ ] `pnpm test` — all Vitest tests pass
- [ ] Frontend dev server starts at `http://localhost:5173` and login page renders
