# Accordly

> **Peer-to-peer agreements — drafted, signed, versioned, and court-ready.**

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/)
[![Vue](https://img.shields.io/badge/Vue-3.x-42b883.svg)](https://vuejs.org/)
[![Status](https://img.shields.io/badge/status-MVP%20Development-orange.svg)]()

---

## Table of Contents

1. [Overview](#overview)
2. [Problem Statement](#problem-statement)
3. [Solution](#solution)
4. [Core Features](#core-features)
5. [Architecture](#architecture)
6. [Data Model](#data-model)
7. [API Surface](#api-surface)
8. [MVP Roadmap](#mvp-roadmap)
9. [Getting Started](#getting-started)
10. [Environment Variables](#environment-variables)
11. [Documentation](#documentation)
12. [Contributing](#contributing)
13. [License](#license)

---

## Overview

**Accordly** is an open-source, self-hostable platform that lets any two (or more) parties draft, negotiate, sign, and archive binding agreements — without lawyers, notaries, or expensive SaaS subscriptions standing in the way.

Every agreement on Accordly is version-controlled, cryptographically signed, attachment-aware, and exportable to a court-ready PDF bundle. Whether you're settling a freelance scope, formalizing a partnership, or documenting a shared living arrangement, Accordly gives your word the weight it deserves.

---

## Problem Statement

Informal agreements fail — not because people act in bad faith, but because:

- **No canonical record exists.** Conversations happen across texts, emails, and memory.
- **Versions get lost.** "Which draft did we agree to?" is a question that kills deals.
- **Signatures are theater.** A DocuSign link costs money, requires accounts on both sides, and still produces a PDF you have to store yourself.
- **Disputes are expensive.** When something goes wrong, reconstructing intent from scattered messages costs time and money neither party planned for.

Small businesses, freelancers, roommates, and family members need a lightweight, trustworthy alternative — one that doesn't require a law firm or a Fortune 500 budget.

---

## Solution

Accordly solves the agreement lifecycle end-to-end:

| Stage | What Accordly Does |
|---|---|
| **Draft** | Rich-text editor with clause templates; all edits versioned automatically |
| **Negotiate** | Inline comments and counter-proposals; full diff between any two versions |
| **Sign** | Cryptographic signature with audit trail; no account required for counterparties |
| **Store** | Immutable, versioned record with attachment support |
| **Export** | One-click court-ready PDF bundle including agreement body, version history, and signature manifest |

---

## Core Features

### Agreement Lifecycle
- **Rich-text drafting** with Markdown support and clause library
- **Automatic versioning** — every save creates a numbered, immutable snapshot
- **Side-by-side diff viewer** between any two versions
- **Status workflow:** `Draft → Pending Signatures → Active → Expired / Terminated`

### Signatures
- **Cryptographic signing** using asymmetric key pairs (Ed25519)
- **Guest signing** — counterparties sign via a secure tokenized link; no Accordly account required
- **Multi-party support** — unlimited signatories per agreement
- **Signature manifest** — timestamped, IP-logged, and tamper-evident

### Attachments
- File attachments scoped to a specific agreement version
- SHA-256 integrity hash stored at upload time
- Attachment referenced in the PDF export by name, hash, and upload timestamp

### Court-Ready Export
- Single PDF bundle containing:
  - Agreement body (final agreed version)
  - Full version history with timestamps and author attribution
  - Inline diff highlights for material changes
  - Signature manifest (signer name, email, timestamp, IP, key fingerprint)
  - Attachment manifest (name, hash, timestamp)
- PDF is digitally signed by the Accordly server key for third-party verifiability

### Access & Collaboration
- Invite counterparties by email or shareable link
- Role-based access: `Owner`, `Collaborator`, `Signer`, `Viewer`
- Org-level agreement management for business accounts
- Audit log on every agreement — every view, edit, comment, and signature recorded

---

## Architecture

Accordly follows a clean separation between a stateless REST/WebSocket API backend and a reactive single-page frontend.

```
┌─────────────────────────────────────────────────────────────────┐
│                          Browser / PWA                          │
│                   Vue 3  ·  Pinia  ·  Tailwind                  │
└──────────────────────────────┬──────────────────────────────────┘
                               │ HTTPS / WSS
┌──────────────────────────────▼──────────────────────────────────┐
│                        Accordly API                             │
│              .NET 10  ·  ASP.NET Core Minimal APIs              │
│         SignalR (real-time)  ·  Carter (route modules)          │
└───────────┬──────────────────┬──────────────────────────────────┘
            │                  │
   ┌────────▼────────┐  ┌──────▼──────────┐
   │   PostgreSQL    │  │   Object Store  │
   │  (EF Core 10)   │  │  (S3-compatible)│
   └─────────────────┘  └─────────────────┘
```

### Backend — `.NET 10`

| Concern | Technology |
|---|---|
| Runtime | .NET 10 / ASP.NET Core Minimal APIs |
| Route modules | Carter |
| ORM | Entity Framework Core 10 |
| Database | PostgreSQL 16 |
| Real-time | SignalR |
| Auth | ASP.NET Core Identity + JWT Bearer |
| File storage | S3-compatible object store (MinIO for local dev) |
| PDF generation | QuestPDF |
| Cryptography | .NET `System.Security.Cryptography` (Ed25519) |
| Background jobs | Hangfire |
| Testing | xUnit + Testcontainers |

**Project layout:**

```
src/
├── Accordly.Api/            # Entry point, middleware, DI composition
├── Accordly.Domain/         # Entities, value objects, domain events
├── Accordly.Application/    # Use cases, commands, queries (CQRS)
├── Accordly.Infrastructure/ # EF Core, storage, email, crypto
└── Accordly.Contracts/      # Shared DTOs and API contracts
tests/
├── Accordly.Unit/
├── Accordly.Integration/
└── Accordly.E2E/
```

### Frontend — `Vue 3`

| Concern | Technology |
|---|---|
| Framework | Vue 3 (Composition API) |
| State | Pinia |
| Routing | Vue Router 4 |
| Styling | Tailwind CSS v4 |
| Rich text | Tiptap v2 |
| Diff viewer | diff2html |
| PDF preview | pdfjs-dist |
| HTTP | Axios + composable wrappers |
| Real-time | SignalR JS client |
| Testing | Vitest + Vue Testing Library + Playwright |

**Project layout:**

```
frontend/
├── src/
│   ├── assets/
│   ├── components/      # Shared UI components
│   ├── composables/     # Reusable logic hooks
│   ├── features/        # Feature-scoped modules
│   │   ├── agreements/
│   │   ├── signatures/
│   │   ├── attachments/
│   │   └── export/
│   ├── layouts/
│   ├── pages/
│   ├── router/
│   ├── stores/          # Pinia stores
│   └── main.ts
├── tests/
└── public/
```

---

## Data Model

### Core Entities

#### `User`
| Column | Type | Notes |
|---|---|---|
| `Id` | `uuid` | PK |
| `Email` | `varchar(320)` | Unique, verified |
| `DisplayName` | `varchar(100)` | |
| `PublicKey` | `text` | Ed25519 public key (Base64) |
| `CreatedAt` | `timestamptz` | |
| `OrganizationId` | `uuid?` | FK → Organization |

#### `Agreement`
| Column | Type | Notes |
|---|---|---|
| `Id` | `uuid` | PK |
| `Title` | `varchar(250)` | |
| `Status` | `enum` | Draft, PendingSignatures, Active, Expired, Terminated |
| `OwnerId` | `uuid` | FK → User |
| `OrganizationId` | `uuid?` | FK → Organization |
| `CurrentVersionId` | `uuid` | FK → AgreementVersion |
| `CreatedAt` | `timestamptz` | |
| `UpdatedAt` | `timestamptz` | |
| `ExpiresAt` | `timestamptz?` | |

#### `AgreementVersion`
| Column | Type | Notes |
|---|---|---|
| `Id` | `uuid` | PK |
| `AgreementId` | `uuid` | FK → Agreement |
| `VersionNumber` | `int` | Auto-increment per agreement |
| `Body` | `text` | Full agreement body (Markdown) |
| `AuthorId` | `uuid` | FK → User |
| `ChangeNote` | `varchar(500)?` | Optional summary of changes |
| `CreatedAt` | `timestamptz` | Immutable after creation |

#### `Signatory`
| Column | Type | Notes |
|---|---|---|
| `Id` | `uuid` | PK |
| `AgreementId` | `uuid` | FK → Agreement |
| `UserId` | `uuid?` | FK → User (null for guest signers) |
| `Email` | `varchar(320)` | |
| `Role` | `enum` | Owner, Collaborator, Signer, Viewer |
| `InviteToken` | `varchar(64)?` | One-time tokenized invite |
| `SignedAt` | `timestamptz?` | Null until signed |
| `SignatureValue` | `text?` | Ed25519 signature (Base64) |
| `SignerIp` | `inet?` | |
| `VersionSignedId` | `uuid?` | FK → AgreementVersion |

#### `Attachment`
| Column | Type | Notes |
|---|---|---|
| `Id` | `uuid` | PK |
| `AgreementId` | `uuid` | FK → Agreement |
| `VersionId` | `uuid` | FK → AgreementVersion (pinned to version) |
| `FileName` | `varchar(255)` | |
| `ContentType` | `varchar(100)` | |
| `StorageKey` | `text` | Object store key |
| `FileSizeBytes` | `bigint` | |
| `Sha256Hash` | `char(64)` | Hex-encoded |
| `UploadedById` | `uuid` | FK → User |
| `UploadedAt` | `timestamptz` | |

#### `AuditEvent`
| Column | Type | Notes |
|---|---|---|
| `Id` | `uuid` | PK |
| `AgreementId` | `uuid` | FK → Agreement |
| `ActorId` | `uuid?` | FK → User (null for system events) |
| `EventType` | `varchar(100)` | e.g. `agreement.viewed`, `version.created` |
| `Payload` | `jsonb?` | Structured event metadata |
| `OccurredAt` | `timestamptz` | |
| `IpAddress` | `inet?` | |

---

## API Surface

All endpoints are prefixed `/api/v1`. Authentication is Bearer JWT unless marked `[public]`.

### Agreements

| Method | Path | Description |
|---|---|---|
| `GET` | `/agreements` | List agreements for the authenticated user |
| `POST` | `/agreements` | Create a new agreement |
| `GET` | `/agreements/{id}` | Get agreement details + current version |
| `PATCH` | `/agreements/{id}` | Update title, status, or expiry |
| `DELETE` | `/agreements/{id}` | Soft-delete (owner only) |

### Versions

| Method | Path | Description |
|---|---|---|
| `GET` | `/agreements/{id}/versions` | List all versions |
| `POST` | `/agreements/{id}/versions` | Save a new version (body required) |
| `GET` | `/agreements/{id}/versions/{versionId}` | Get a specific version |
| `GET` | `/agreements/{id}/versions/diff` | Diff two versions (`?from=&to=`) |

### Signatories

| Method | Path | Description |
|---|---|---|
| `GET` | `/agreements/{id}/signatories` | List signatories and their status |
| `POST` | `/agreements/{id}/signatories` | Invite a signatory |
| `DELETE` | `/agreements/{id}/signatories/{sigId}` | Remove an unsigned signatory |
| `POST` | `/agreements/{id}/signatories/{sigId}/sign` | Submit a cryptographic signature |
| `GET` | `/sign/{token}` | `[public]` Resolve guest invite token |
| `POST` | `/sign/{token}` | `[public]` Guest sign via token |

### Attachments

| Method | Path | Description |
|---|---|---|
| `GET` | `/agreements/{id}/attachments` | List attachments for the agreement |
| `POST` | `/agreements/{id}/attachments` | Upload an attachment (multipart/form-data) |
| `GET` | `/agreements/{id}/attachments/{attachId}` | Download attachment |
| `DELETE` | `/agreements/{id}/attachments/{attachId}` | Remove attachment |

### Export

| Method | Path | Description |
|---|---|---|
| `GET` | `/agreements/{id}/export/pdf` | Generate and stream court-ready PDF bundle |
| `GET` | `/agreements/{id}/export/json` | Export full agreement record as structured JSON |

### Audit

| Method | Path | Description |
|---|---|---|
| `GET` | `/agreements/{id}/audit` | Paginated audit log for the agreement |

### Auth

| Method | Path | Description |
|---|---|---|
| `POST` | `/auth/register` | Register a new user |
| `POST` | `/auth/login` | Authenticate; returns JWT + refresh token |
| `POST` | `/auth/refresh` | Rotate refresh token |
| `POST` | `/auth/logout` | Revoke refresh token |

### Real-Time (SignalR)

Hub path: `/hubs/agreements`

| Event | Direction | Payload |
|---|---|---|
| `VersionCreated` | Server → Client | `{ agreementId, versionId, versionNumber, authorName }` |
| `SignatoryUpdated` | Server → Client | `{ agreementId, signatoryId, status }` |
| `AgreementStatusChanged` | Server → Client | `{ agreementId, newStatus }` |
| `AttachmentUploaded` | Server → Client | `{ agreementId, attachmentId, fileName }` |

---

## MVP Roadmap

### Phase 1 — Foundation · Target: October 2026
- [ ] Project scaffolding (monorepo, CI pipeline, Docker Compose)
- [ ] Auth (register, login, JWT, refresh)
- [ ] Agreement CRUD with versioning
- [ ] Diff viewer (side-by-side)
- [ ] Basic Vue frontend (agreement list, detail, editor)

### Phase 2 — Signatures · Target: November 2026
- [ ] Ed25519 key generation and storage
- [ ] Signatory invitation (registered user + guest token flow)
- [ ] Cryptographic signing endpoint
- [ ] Signature manifest display in UI
- [ ] Status workflow automation (auto-activate on all signatures collected)

### Phase 3 — Attachments & Export · Target: December 2026
- [ ] File upload to object store with SHA-256 hashing
- [ ] Attachment list and download
- [ ] QuestPDF court-ready bundle generation
- [ ] PDF digital signing with server key
- [ ] JSON export

### Phase 4 — Audit & Hardening · Target: January 2027
- [ ] Full audit event system
- [ ] Audit log UI (filterable, paginated)
- [ ] Rate limiting and abuse controls
- [ ] Comprehensive integration and E2E test suite
- [ ] Security review pass

### Phase 5 — Organizations & Polish · Target: Q1 2027
- [ ] Organization accounts (multi-user, shared agreement library)
- [ ] Role-based access control enforcement
- [ ] Notification emails (invite, signature request, agreement activated)
- [ ] Agreement templates and clause library
- [ ] Mobile-responsive PWA polish

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 22+](https://nodejs.org/) and [pnpm](https://pnpm.io/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for PostgreSQL + MinIO)

### Local Development

```bash
# 1. Clone the repository
git clone https://github.com/danj210/accordly.git
cd accordly

# 2. Start infrastructure (PostgreSQL + MinIO)
docker compose up -d

# 3. Apply database migrations
cd src/Accordly.Api
dotnet ef database update

# 4. Run the API
dotnet run

# 5. In a new terminal, start the frontend
cd ../../frontend
pnpm install
pnpm dev
```

The API will be available at `https://localhost:5001` and the Vue dev server at `http://localhost:5173`.

---

## Documentation

Project documentation is organized under [`docs/`](docs/README.md):

- [Architecture and project guide](docs/architecture.md)
- [API reference](docs/api.md)
- [Data model](docs/data-model.md)
- [Local development](docs/development.md)
- [Scaffold status and roadmap](docs/scaffold-status.md)

### Running Tests

```bash
# Backend
dotnet test

# Frontend unit tests
pnpm test

# Frontend E2E (Playwright)
pnpm test:e2e
```

---

## Environment Variables

### API (`src/Accordly.Api/appsettings.Development.json`)

| Variable | Description | Default |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string | `Host=localhost;Database=accordly;...` |
| `Storage__Endpoint` | S3-compatible endpoint | `http://localhost:9000` |
| `Storage__Bucket` | Object store bucket name | `accordly` |
| `Storage__AccessKey` | Object store access key | `minioadmin` |
| `Storage__SecretKey` | Object store secret key | `minioadmin` |
| `Jwt__Secret` | JWT signing secret (≥ 32 chars) | *Required* |
| `Jwt__Issuer` | JWT issuer claim | `accordly` |
| `Jwt__Audience` | JWT audience claim | `accordly-client` |
| `Jwt__ExpiryMinutes` | Access token lifetime | `60` |
| `ServerKey__PrivateKey` | Ed25519 server private key (Base64) | *Required* |
| `Email__Host` | SMTP host | `localhost` |
| `Email__Port` | SMTP port | `1025` |

### Frontend (`frontend/.env.local`)

| Variable | Description |
|---|---|
| `VITE_API_BASE_URL` | Accordly API base URL |
| `VITE_SIGNALR_HUB_URL` | SignalR hub URL |

---

## Contributing

Contributions are welcome! Please read [CONTRIBUTING.md](CONTRIBUTING.md) before opening a pull request.

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature`
3. Commit your changes following [Conventional Commits](https://www.conventionalcommits.org/)
4. Push and open a Pull Request against `main`

Please ensure all tests pass and new functionality is covered before requesting review.

---

## License

Accordly is released under the [MIT License](LICENSE).

---

*Built with intention. Agreements that mean something.*
