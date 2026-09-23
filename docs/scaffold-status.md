# Scaffold Status

`SCAFFOLD.md` remains the detailed task list. This page summarizes the current state of the project.

## Implemented Initial Slice

- .NET 10 solution in `Accordly.slnx`
- Domain entities, enums, and base entity
- Domain event records for status changes, version creation, and signatures
- Agreement status transition rules and unit tests
- Contracts records for agreements, versions, signatories, attachments, and auth
- Application repository/service interfaces
- Create-agreement command, handler, validator, get-agreement query, and MediatR validation pipeline
- Create-agreement validator and handler unit tests
- SQL Server EF Core context and entity configurations
- Initial EF Core migration, inspected SQL, and verified SQL Server application
- Server-side refresh-token persistence with SHA-256 hashes, lifecycle metadata, indexes, and an additive migration
- EF repositories, unit of work, S3-compatible storage, SMTP email, and Hangfire registration
- Scoped token service for configured JWT issuance and cryptographically secure refresh-token creation
- API composition root with Carter, JWT bearer configuration, Serilog, Swagger, exception middleware, and SignalR hub
- Separate Carter modules for versions, signatories, attachments, export, auth, and audit, with authorization excluded from public auth and guest-signing routes
- Docker Compose services for SQL Server, MinIO, and MailHog
- Vue/Vite frontend shell, base router and auth guard, Pinia stores, Axios/SignalR composables, layouts, page shells, and initial presentation components
- Root README aligned with the implemented SQL Server and MSTest stack

## Still Outstanding

- Claim-based acting-user resolution and agreement authorization; current agreement routes still contain temporary client-supplied/empty identity values
- Real agreement CRUD, immutable version workflows/diff, signatory and guest signing, attachment persistence, exports, audit recording/querying, and authorized SignalR publication behind the existing route-module shells
- Agreement integration coverage beyond the validated auth lifecycle test, followed by real Playwright E2E infrastructure
- Frontend Vitest setup and behavioral coverage for API interception, auth state, routing, and agreement workflows
- Agreement editor route, Tiptap editor, and validated AgreementVersion save workflow
- Complete agreement list/detail presentation, child-data composition, and SignalR-driven reconciliation
- Dashboard recent agreements, pagination, and accessible agreement creation modal
- Working guest signature submission and attachment multipart upload; the current components are presentation-only shells

## Validation Notes

The backend solution build, the `Accordly.Unit` test project, the Docker-backed authentication lifecycle integration test, and the frontend type check/production build have passed. The auth integration test validates registration, duplicate registration, invalid login, shared Identity/domain IDs, hashed refresh-token persistence, rotation/replay rejection, and idempotent logout/revocation. The API project build passes after the Carter module separation; current NuGet vulnerability advisories remain as warnings. Existing non-auth route modules and frontend page/feature shells must not be treated as complete behavior until their focused checklist items and tests pass. Keep validation results current in `SCAFFOLD.md`. Document environment errors separately from actual test failures.
