# Architecture

## System Shape

Accordly is a monorepo with a stateless ASP.NET Core API and a Vue single-page application.

```text
Browser / PWA
    |
    | HTTPS and WebSockets
    v
Accordly.Api
    |-- Carter route modules
    |-- JWT and Identity authentication
    |-- SignalR at /hubs/agreements
    |-- Swagger in development
    |
    +--> Accordly.Application --> Accordly.Domain
    |         CQRS and ports       Entities and rules
    |
    +--> Accordly.Infrastructure
              EF Core / SQL Server
              Identity persistence
              S3-compatible storage
              SMTP email
              Hangfire jobs
```

## Backend Projects

### `Accordly.Domain`

Contains entities, enums, shared entity behavior, and domain events. It must remain independent of the other Accordly projects and external infrastructure.

Core entities are `User`, `Organization`, `Agreement`, `AgreementVersion`, `Signatory`, `Attachment`, and `AuditEvent`.

### `Accordly.Contracts`

Contains API request and response records. Contracts are transport types and must not depend on Domain or EF Core.

### `Accordly.Application`

Contains use cases, MediatR commands and queries, FluentValidation validators, the MediatR validation pipeline, repository interfaces, and service interfaces. It depends on Domain and Contracts.

### `Accordly.Infrastructure`

Implements Application interfaces using EF Core SQL Server, ASP.NET Core Identity, AWS S3-compatible APIs, SMTP, and Hangfire. It depends on Domain and Application.

### `Accordly.Api`

The ASP.NET Core host. It composes dependency injection, authentication, middleware, Carter modules, SignalR, Swagger, Serilog, and the Hangfire dashboard.

## Frontend Project

`frontend/` is a Vite Vue 3 TypeScript application:

- `src/router/`: route definitions and authentication guard
- `src/stores/`: Pinia application state
- `src/composables/`: Axios and SignalR integrations
- `src/features/`: feature-owned components and barrels
- `src/layouts/`: authenticated and guest shells
- `src/pages/`: route-level views
- `tests/`: Vitest component/composable/store tests

## Infrastructure

`docker-compose.yml` provides local services on the `accordly-net` bridge network:

- SQL Server 2022 at `localhost:1433`
- MinIO S3 API at `localhost:9000` and console at `localhost:9001`
- MailHog SMTP at `localhost:1025` and web UI at `localhost:8025`

## Design Invariants

- Agreement versions are immutable snapshots.
- Signatures identify the version signed.
- Attachments are pinned to agreement versions and retain SHA-256 hashes.
- Audit events are append-only.
- Identity and authorization decisions use server-side claims and persisted credentials.
- Configuration and secrets come from configuration providers or environment variables.
