# Local Development

## Prerequisites

- .NET 10 SDK
- Node.js 22 or newer
- pnpm
- Docker Desktop

## Start Local Services

From the repository root:

```powershell
docker compose up -d
```

Services:

- SQL Server: `localhost,1433`
- MinIO API: `http://localhost:9000`
- MinIO console: `http://localhost:9001`
- MailHog SMTP: `localhost:1025`
- MailHog UI: `http://localhost:8025`

## Backend

Restore and build from the repository root:

```powershell
dotnet restore Accordly.slnx
dotnet build Accordly.slnx
```

Run the API:

```powershell
dotnet run --project src/Accordly.Api
```

In development, Swagger is exposed by the API and the Hangfire dashboard is available at `/hangfire`.

The local SQL Server connection string is configured in `src/Accordly.Api/appsettings.Development.json`.

## Frontend

From `frontend/`:

```powershell
pnpm install
pnpm exec vue-tsc --noEmit
pnpm run build
pnpm dev
```

The Vite development server runs at `http://localhost:5173` by default. Copy `.env.example` to `.env.local` and adjust:

```text
VITE_API_BASE_URL=http://localhost:5000/api/v1
VITE_SIGNALR_HUB_URL=http://localhost:5000
```

## Tests

Backend tests use MSTest:

```powershell
dotnet test Accordly.slnx
```

Frontend tests use Vitest:

```powershell
Set-Location frontend
pnpm test
```

Integration tests require Docker because they use `Testcontainers.MsSql`.

## EF Core Migrations

Migrations belong to `Accordly.Infrastructure` and use `Accordly.Api` as the startup project:

```powershell
dotnet ef migrations add <MigrationName> --project src/Accordly.Infrastructure --startup-project src/Accordly.Api
dotnet ef database update --project src/Accordly.Infrastructure --startup-project src/Accordly.Api
```

Build the startup project before using `--no-build`; otherwise EF can load a stale referenced assembly and produce an incorrect model diff. Add a new migration for persisted model changes rather than rewriting a migration that may already be applied. Review the generated model diff and SQL before treating a migration as validated.

## Configuration and Secrets

Development defaults are local-only examples. Do not add production credentials, JWT secrets, server private keys, or cloud access keys to source control. Use environment variables or user secrets for sensitive values.

Important configuration sections:

- `ConnectionStrings:DefaultConnection`
- `Jwt`
- `RefreshToken:ExpiryHours`
- `Storage`
- `ServerKey`
- `Email`
- `Hangfire`

The JWT signing secret must be at least 32 characters. `RefreshToken:ExpiryHours` controls the persisted
refresh-token lifetime and must be greater than zero. Register and login create persisted refresh-token
records; refresh rotates them and logout revokes them.
