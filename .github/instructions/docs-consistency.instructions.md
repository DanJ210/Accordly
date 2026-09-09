---
description: "Keeps Accordly documentation authoritative and aligned with the .NET API, domain, infrastructure, tests, and Vue frontend. Use when changing docs, project behavior, routes, models, configuration, or scaffold status."
applyTo: "README.md,SCAFFOLD.md,docs/**/*.md,.github/copilot-instructions.md,.github/instructions/**/*.md,src/**/*.cs,tests/**/*.cs,frontend/src/**/*.ts,frontend/src/**/*.vue,frontend/package.json,docker-compose.yml"
---

# Documentation Consistency

## Ownership Boundary

Each durable fact should have one authoritative owner. Do not copy the same behavior documentation into several files. Link to the owning document instead.

| Topic | Authoritative owner |
| --- | --- |
| REST routes, methods, request/response concepts, access requirements, status/error behavior | `docs/api.md` |
| Domain entities, enums, DTO concepts, persistence fields, and relationships | `docs/data-model.md` |
| System design, project boundaries, service layering, storage, Identity, and SignalR | `docs/architecture.md` |
| Local setup, Docker services, configuration keys, migrations, build/run/test commands | `docs/development.md` and `.github/copilot-instructions.md` for agent workflow |
| Implemented versus planned work and validation state | `docs/scaffold-status.md` and `SCAFFOLD.md` |
| Documentation navigation and links | `docs/README.md` |
| Product vision, feature intent, and long-term roadmap | `README.md` |
| How Copilot should work in this repository | `.github/copilot-instructions.md` |

`README.md` is the product-facing overview. `SCAFFOLD.md` is the detailed implementation checklist. The topic documents under `docs/` explain the current system. `.github/copilot-instructions.md` explains agent behavior and validation; it should link to documentation instead of becoming a second API, architecture, or data-model reference.

When documentation conflicts with implementation, inspect the code and tests first. Then update the authoritative document and any concise navigation or status references in the same change. Do not silently preserve a known contradiction.

## Same-Change Update Rules

Update the owning document in the same change as the implementation change:

- Added, removed, renamed, or changed a Carter route, HTTP method, request body, response, status code, authorization rule, public guest-signing route, or SignalR event -> update `docs/api.md`.
- Changed a Domain entity, enum, domain event, Contract record, EF mapping, Identity persistence shape, or frontend API-facing type -> update `docs/data-model.md`.
- Changed project dependencies, service boundaries, authentication flow, claim resolution, storage, email, Hangfire, SignalR wiring, or export architecture -> update `docs/architecture.md`.
- Changed connection strings, environment variables, Docker services, migration commands, prerequisites, build commands, or test commands -> update `docs/development.md` and the relevant operational section of `.github/copilot-instructions.md`.
- Added or completed a scaffold task, changed validation evidence, or discovered a blocker -> update both `SCAFFOLD.md` and `docs/scaffold-status.md`.
- Added or renamed a document -> update `docs/README.md` and the documentation navigation in `.github/copilot-instructions.md`.
- Changed product scope or roadmap direction -> update `README.md`; do not present planned work as shipped in `docs/`.

A checked item in `SCAFFOLD.md` means the work exists and has been validated. Keep partial work unchecked and describe it as incomplete or planned.

## Accuracy Requirements

- Document behavior that exists on the current branch. Use explicit `planned`, `stub`, `partial`, or `not yet implemented` language for unfinished work.
- Verify API route lists against `src/Accordly.Api/Modules/` and `src/Accordly.Api/Program.cs` before changing `docs/api.md`.
- Verify entity and DTO lists against `src/Accordly.Domain/Entities/`, `src/Accordly.Domain/Enums/`, `src/Accordly.Contracts/`, and EF configurations before changing `docs/data-model.md`.
- Verify frontend route, store, composable, and feature claims against `frontend/src/` before changing frontend documentation.
- The implementation target is SQL Server 2022 with EF Core SQL Server. Do not document PostgreSQL or Npgsql as the current implementation.
- Backend tests use MSTest, Moq, and Testcontainers.MsSql. Do not describe the test stack as xUnit.
- Do not state exact test counts unless the count was verified by a test run and is intended to be maintained.
- Do not document client-supplied user IDs as authoritative identity. Authorization must resolve the acting user server-side from authenticated claims; any temporary development fallback must be clearly labeled and documented.
- Do not document development credentials as production secrets. Keep secrets, private keys, and real access tokens out of tracked files.
- Do not claim migrations, authentication, route modules, tests, or frontend workflows are complete merely because a placeholder file exists.
- Avoid duplicating long architecture, API, or data-model sections in `.github/copilot-instructions.md`; link to the owning `docs/` page.

## Documentation Review Pass

Before completing a code change that affects a documented behavior:

1. Identify the owning document from the table above.
2. Compare the implementation and tests with that document.
3. Update the owner in the same change.
4. Update `docs/README.md` only if navigation changed.
5. Update `SCAFFOLD.md` and `docs/scaffold-status.md` if implementation status changed.
6. Run the narrowest relevant validation command and report blockers separately from test failures.

For a documentation-only change, check links, filenames, route names, project paths, configuration keys, and status wording against the repository before finalizing.
