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
- Vue/Vite frontend shell, router, Pinia stores, Axios/SignalR composables, layouts, pages, and initial feature components
- Root README aligned with the implemented SQL Server and MSTest stack

## Still Outstanding

- Claim-based owner identity propagation in agreement endpoints
- Auth phase 6.8a-f: contracts, refresh-token persistence, token issuance, Identity setup, register/login,
  refresh/logout, and persisted token rotation/revocation; 6.8g validation is implemented but requires Docker
  for its SQL-backed integration test before it can be marked complete
- Integration, E2E, and frontend tests
- Full editor route and Tiptap AgreementVersion save workflow
- Complete agreement detail composition and SignalR-driven UI updates
- Pagination and agreement creation modal

## Validation Notes

The initial backend solution build, the `Accordly.Unit` test project, and the frontend type check/production build have passed. Focused token issuance and refresh-token persistence unit tests also pass. The API project build passes after the Carter module separation; current NuGet vulnerability advisories remain as warnings. Keep validation results current in `SCAFFOLD.md`. Document environment errors separately from actual test failures.
