# Accordly Copilot Instructions

## Purpose

Accordly is a self-hostable peer-to-peer agreement platform. It lets parties draft, version, negotiate, cryptographically sign, attach files to, audit, and export agreements as court-ready PDF/JSON records.

Treat this file, `README.md`, and `SCAFFOLD.md` as the primary repository context. `SCAFFOLD.md` is the implementation checklist and must remain accurate as work progresses.

## Product Context

The important product invariants are:

- Agreement content is versioned. A saved version is an immutable snapshot.
- Signatures are tied to a specific agreement version and must retain timestamp, signer identity or guest email, IP address where available, and signature value.
- Attachments are scoped to an agreement version and retain a SHA-256 integrity hash.
- Audit events are append-only records of meaningful activity.
- Agreement status follows a controlled lifecycle: `Draft`, `PendingSignatures`, `Active`, `Expired`, `Terminated`.
- Role values are `Owner`, `Collaborator`, `Signer`, and `Viewer`.
- Guest signing routes are public by design, but tokens must be treated as sensitive, single-use or revocable credentials.
- Court-ready exports must preserve enough version, signature, attachment, and audit metadata to explain the agreement history.

Do not weaken these invariants for convenience. When a feature changes persisted agreement, version, signature, attachment, or audit behavior, add or update tests and update the relevant checklist item.

## Repository Layout

```text
src/Accordly.Domain/         Entities, enums, domain events; no project dependencies
src/Accordly.Contracts/      API request/response records; no project dependencies
src/Accordly.Application/    CQRS handlers, validators, interfaces, use-case orchestration
src/Accordly.Infrastructure/ EF Core SQL Server, Identity, repositories, S3/MinIO, email, Hangfire
src/Accordly.Api/            ASP.NET Core host, Carter modules, auth, middleware, SignalR

tests/Accordly.Unit/         MSTest unit tests
tests/Accordly.Integration/  MSTest/Testcontainers API tests
tests/Accordly.E2E/          MSTest project reserved for Playwright workflows
frontend/                    Vue 3, TypeScript, Pinia, Vue Router, Tailwind, Tiptap
```

The .NET 10 CLI currently generates `Accordly.slnx`. Use that solution file unless the repository explicitly changes format.

## Backend Rules

### Domain

- Keep Domain independent of Application, Infrastructure, API, EF Core, Identity, and external packages.
- Put entities in `Entities/`, enums in `Enums/`, shared entity behavior in `Common/`, and domain records in `Events/`.
- Use `Guid` identifiers and `DateTimeOffset` timestamps. New entities use `Guid.NewGuid()`.
- Preserve the base `Entity` contract: `Id` and `CreatedAt`.
- Put lifecycle rules in domain/application code rather than route handlers.
- Do not add database or HTTP concerns to domain types.

### Contracts

- Keep Contracts independent of Domain. Use transport-safe primitive representations when necessary, such as strings for enum values.
- Use C# `record` types for API requests and responses.
- Do not expose EF Core entities directly from API endpoints.
- Keep request/response names aligned with the API surface in `README.md` and `SCAFFOLD.md`.

### Application

- Use MediatR for commands and queries.
- Keep handlers focused on one use case and depend on interfaces from `Common/Interfaces/`.
- Validate commands with FluentValidation. Validation should happen before handlers through a MediatR pipeline behavior.
- Map domain entities to Contracts explicitly.
- Use cancellation tokens on asynchronous methods and pass them through to repositories and external services.
- Repository abstractions belong in Application; EF implementations belong in Infrastructure.

### Infrastructure

- Use EF Core 10 with the SQL Server provider only. Do not add PostgreSQL or Npgsql packages/types.
- Keep EF mappings in `Persistence/Configurations/`, preferably one configuration class per entity.
- Store `AgreementStatus` and `SignatoryRole` as strings.
- Use `ValueGeneratedNever()` for Guid primary keys where IDs are assigned by the domain.
- Keep `AgreementVersion` creation data immutable after insertion.
- Use `nvarchar(max)` for audit JSON payloads and a maximum length of 45 for IP address strings.
- Keep object storage behind `IStorageService`; MinIO is the local S3-compatible implementation.
- Keep email behind `IEmailService`; MailHog is the local SMTP target.
- Read credentials, connection strings, JWT values, storage settings, and server keys from configuration or environment variables. Never add real secrets to source control.
- Register infrastructure services through `AddInfrastructure(IServiceCollection, IConfiguration)`.
- EF migrations belong under the Infrastructure project and must be reviewed for all expected tables and constraints.

### API

- Use Carter modules implementing `ICarterModule` and registering routes in `AddRoutes(IEndpointRouteBuilder)`.
- Keep routes under `/api/v1` and match the API surface in `README.md`.
- Protect agreement, version, signatory, attachment, export, and audit routes with authorization.
- Keep `/api/v1/sign/{token}` and `/api/v1/auth/*` public where specified.
- Obtain the authenticated user ID from claims. Do not use `Guid.Empty`, request-provided owner IDs, or client-controlled identity values for authorization decisions.
- Use `201 Created` for successful agreement creation and return the appropriate Contract response.
- Use `Results.NotFound()` for missing resources and consistent problem/error payloads for validation and unexpected failures.
- Keep exception translation in the global middleware, not duplicated across endpoints.
- JWT validation must check issuer, audience, lifetime, and signing key.
- Identity manages credentials; refresh tokens must be stored, rotated, revoked, and validated server-side.
- Map the SignalR hub at `/hubs/agreements`. Preserve the client event names `VersionCreated`, `SignatoryUpdated`, `AgreementStatusChanged`, and `AttachmentUploaded`.
- Keep Swagger/OpenAPI enabled in development and Serilog structured logging enabled for API requests.

## Frontend Rules

- Use Vue 3 Composition API with TypeScript strict mode.
- Use Pinia for client state and Vue Router for navigation.
- Keep API calls in composables/stores rather than duplicating Axios setup in components.
- Attach the bearer token through the shared Axios request interceptor. Redirect to `/login` on a 401 response.
- Do not put secrets in frontend code. Only expose `VITE_*` public configuration values.
- Keep guest signing visually and behaviorally separate from authenticated application views.
- Preserve the route model in `src/router/index.ts`, including the auth guard and public signing route.
- Keep feature-specific UI under `src/features/` with barrel exports where the existing feature uses them.
- Agreement detail should compose signatories, attachments/upload, export, version history, and SignalR updates as those features become real.
- Use Tiptap for agreement editing and create a new AgreementVersion on save; do not mutate an existing version body.
- Make status badges distinguish the five agreement statuses and ensure layouts work on narrow screens.
- Keep frontend tests in `frontend/tests/` and use Vitest with Vue Test Utils or Testing Library.

## Testing Requirements

- Backend tests use MSTest only: `[TestClass]`, `[TestMethod]`, `[TestInitialize]`, `[ClassInitialize]`, and `[ClassCleanup]` as appropriate. Do not use xUnit or NUnit attributes.
- Unit tests must cover status transitions, command validation, and handler repository/unit-of-work calls.
- Use Moq for unit-test collaborators.
- Integration tests use `Microsoft.AspNetCore.Mvc.Testing` and `Testcontainers.MsSql`; start one SQL Server container per test class and dispose it in `[ClassCleanup]`.
- Integration tests must exercise authenticated API behavior and assert status codes plus response bodies.
- Frontend tests must cover bearer-header attachment, 401 redirect behavior, token persistence, logout clearing, and registration endpoint usage.
- Do not mark a checklist item complete merely because a file exists. Run the narrowest relevant test or build command first.

## Development Workflow

1. Read the nearby implementation, its tests, and the relevant section of `SCAFFOLD.md` before editing.
2. Form one concrete hypothesis about the behavior or missing piece and choose a focused check that can disprove it.
3. Make the smallest change that advances one checklist item. Preserve unrelated working-tree changes.
4. Run focused validation immediately after the edit.
5. Update `SCAFFOLD.md` only when the item is genuinely implemented and validated. Keep partial work unchecked and add a short note when needed.
6. Finish with a broader build or test when the touched change crosses project boundaries.
7. Do not commit, reset, or create branches unless explicitly requested.

Prefer existing patterns over new abstractions. Avoid broad refactors, speculative features, placeholder comments, and unrelated formatting changes.

## Useful Commands

From the repository root:

```powershell
dotnet restore Accordly.slnx
dotnet build Accordly.slnx
dotnet test Accordly.slnx
```

From `frontend/`:

```powershell
pnpm install
pnpm exec vue-tsc --noEmit
pnpm run build
pnpm test
pnpm dev
```

Local infrastructure:

```powershell
docker compose up -d
docker compose down
```

Before claiming completion, verify the relevant command actually ran. If the local .NET SDK reports a workload-manifest resolver failure, report it as an environment blocker and do not claim tests passed.

## Current Scaffold Status

The current working tree has a buildable initial slice: the .NET projects and references, domain entities/enums, Contracts records, Application interfaces and agreement handlers, EF/SQL Server infrastructure, API composition root and initial Carter routes, Docker Compose services, and the Vue frontend shell/stores/composables/layouts/basic feature components.

The following areas remain unfinished and must stay visible in `SCAFFOLD.md` until implemented and validated:

- Domain event records
- MediatR validation pipeline behavior
- Initial EF Core migration
- Complete route-module separation and correct claim-based identity propagation
- Docker-backed auth integration validation for the register/login/refresh/logout lifecycle
- Unit, integration, E2E, and frontend tests
- Full editor route/page and version-save workflow
- Complete agreement detail composition, SignalR UI updates, pagination, and create modal
- Accurate README references to SQL Server and MSTest where older text still says PostgreSQL or xUnit

When completing one of these areas, update both the implementation and the corresponding checklist entry rather than silently treating a stub as complete.
