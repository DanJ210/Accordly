# Scaffold Status

`SCAFFOLD.md` remains the detailed task list. This page summarizes the current state of the project projects.

## Implemented Initial Slice

- .NET 10 solution in `Accordly.slnx`
- Domain entities, enums, and base entity
- Domain event records for status changes, version creation, and signatures
- Contracts records for agreements, versions, signatories, attachments, and auth
- Application repository/service interfaces
- Create-agreement command, handler, validator, and get-agreement query
- SQL Server EF Core context and entity configurations
- EF repositories, unit of work, S3-compatible storage, SMTP email, and Hangfire registration
- API composition root with Carter, JWT bearer configuration, Serilog, Swagger, exception middleware, and SignalR hub
- Docker Compose services for SQL Server, MinIO, and MailHog
- Vue/Vite frontend shell, router, Pinia stores, Axios/SignalR composables, layouts, pages, and initial feature components

## Still Outstanding

- MediatR validation pipeline behavior
- Initial EF Core migration and database verification
- Complete Carter module separation for versions, signatories, attachments, export, auth, and audit
- Claim-based owner identity propagation in agreement endpoints
- Real Identity registration, login, JWT issuance, refresh-token rotation, and logout
- Agreement status transition rules
- Unit, integration, E2E, and frontend tests
- Full editor route and Tiptap AgreementVersion save workflow
- Complete agreement detail composition and SignalR-driven UI updates
- Pagination and agreement creation modal
- Documentation cleanup in the original README where legacy PostgreSQL/xUnit references remain

## Validation Notes

The initial backend solution build and frontend type check/production build have passed. Keep validation results current in `SCAFFOLD.md`. If `dotnet test` is blocked by a local SDK workload-manifest problem, document the environment error separately from actual test failures.
