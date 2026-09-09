<div align="center">

# ⚖️ Accordly

**A peer-to-peer agreement platform built for trust, transparency, and legal durability.**

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Backend: .NET 10](https://img.shields.io/badge/Backend-.NET%2010-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Frontend: Vue 3](https://img.shields.io/badge/Frontend-Vue%203-42b883?logo=vue.js)](https://vuejs.org/)
[![Status: MVP In Progress](https://img.shields.io/badge/Status-MVP%20In%20Progress-orange)]()

</div>

---

## Table of Contents

1. [Purpose](#1-purpose)
2. [Problem Statement](#2-problem-statement)
3. [Solution Overview](#3-solution-overview)
4. [Core Features](#4-core-features)
5. [Architecture](#5-architecture)
6. [Tech Stack](#6-tech-stack)
7. [Data Model Overview](#7-data-model-overview)
8. [API Surface](#8-api-surface)
9. [MVP Scope](#9-mvp-scope)
10. [Roadmap & Future Enhancements](#10-roadmap--future-enhancements)
11. [Getting Started](#11-getting-started)
12. [Contributing](#12-contributing)
13. [License](#13-license)

---

## 1. Purpose

Accordly is an open-source, peer-to-peer agreement platform that gives individuals and
organizations a structured, auditable way to create, sign, version, and store agreements —
without relying on expensive legal intermediaries or opaque SaaS services.

Every agreement on Accordly is a living document: it can be negotiated, revised,
co-signed, and ultimately locked into a tamper-evident record that is suitable for dispute
resolution, court submission, or archival reference.

---

## 2. Problem Statement

Informal agreements between parties — freelancers and clients, landlords and tenants,
business partners, family members — are routinely made over email, chat, or verbal
conversation. These channels share a common set of critical failures:

- **No canonical version** — parties often work from different copies, leading to disputes
  about what was actually agreed.
- **No auditable history** — edits and negotiations happen silently, with no record of who
  changed what and when.
- **No enforceable signatures** — a reply-all email is not a signature; a screenshot of a
  chat is not a contract.
- **No structured export** — presenting an agreement in court or arbitration requires
  expensive reformatting and counsel.
- **Vendor lock-in** — proprietary e-signature platforms hold your agreements hostage behind
  subscription paywalls.

Accordly is purpose-built to eliminate every one of these failure modes.

---

## 3. Solution Overview

Accordly treats every agreement as a **versioned, cryptographically signed ledger entry**.
Parties collaborate on agreement text in real time, negotiate through structured amendment
proposals, and finalize the agreement with legally meaningful digital signatures. Every state
transition — draft, amendment, counter-proposal, signature, revocation — is recorded as an
immutable event in the agreement's history.

The result is a full audit trail that can be exported as a self-contained, court-ready PDF
bundle: agreement text, version history, signature manifest, and supporting attachments all
in one document.

```
Party A drafts agreement
        │
        ▼
Party B reviews & proposes amendments ──► Party A accepts / counter-proposes
        │                                        │
        └────────────────────────────────────────┘
                        │
                        ▼
              Both parties co-sign
                        │
                        ▼
            Agreement is locked & sealed
                        │
                        ▼
        Exportable as court-ready PDF bundle
```

---

## 4. Core Features

### ✍️ Agreement Authoring
- Rich-text agreement editor with structured clause support
- Party invitations via email or shareable link
- Real-time collaborative editing with conflict resolution
- Named agreement templates for common use cases

### 🔄 Versioning & Negotiation
- Every save creates an immutable version snapshot
- Proposed amendments are tracked as discrete change sets
- Each party can accept, reject, or counter-propose amendments
- Full diff view between any two versions

### 🔏 Digital Signatures
- Cryptographic signature capture (RSA / ECDSA key pairs)
- Identity verification via email confirmation + optional ID assertion
- Signature timestamps anchored to a trusted time source
- Multi-party signature sequencing (parallel or ordered)
- Signature revocation with recorded reason and timestamp

### 📎 Attachments
- Upload supporting documents (PDFs, images, spreadsheets) as exhibits
- Attachments are hashed and bound to the agreement version at signing time
- Reference exhibits inline in agreement clauses

### 📄 Court-Ready Export
- One-click export to a sealed PDF bundle including:
  - Final agreement text
  - Complete version history & diff log
  - Signature manifest with timestamps and key fingerprints
  - Attachment exhibit index with hash verification
- Export metadata is itself signed to certify bundle authenticity

### 🔐 Security & Privacy
- End-to-end encryption for agreement content in transit
- Encryption at rest for stored agreements and attachments
- Granular access control: drafter, reviewer, signatory, witness, observer
- Optional private mode: zero server-side plaintext storage

### 📊 Dashboard & Notifications
- Unified inbox for pending actions (review, sign, counter-propose)
- Agreement lifecycle status at a glance
- Email and in-app notifications for all state transitions

---

## 5. Architecture

Accordly follows a clean **API-first, layered architecture** with a decoupled frontend
and backend.

```
┌─────────────────────────────────────────────────────┐
│                   Vue 3 Frontend                     │
│          (SPA · Pinia · Vue Router · Quasar)         │
└──────────────────────┬──────────────────────────────┘
                       │ HTTPS / REST + WebSocket
┌──────────────────────▼──────────────────────────────┐
│              .NET 10 Web API (ASP.NET Core)          │
│  ┌───────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │ Agreement │  │  Signature   │  │   Export     │  │
│  │  Service  │  │   Service    │  │   Service    │  │
│  └───────────┘  └──────────────┘  └──────────────┘  │
│  ┌───────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │   User /  │  │  Versioning  │  │  Attachment  │  │
│  │  Identity │  │   Engine     │  │   Service    │  │
│  └───────────┘  └──────────────┘  └──────────────┘  │
└──────────────────────┬──────────────────────────────┘
                       │
       ┌───────────────┼──────────────────┐
       ▼               ▼                  ▼
 PostgreSQL       Redis Cache        Object Store
 (primary DB)   (sessions/events)  (attachments/exports)
```

### Key Architectural Decisions

| Decision | Choice | Rationale |
|---|---|---|
| Agreement storage | PostgreSQL (JSONB columns for clause trees) | Relational integrity + flexible schema for clause structures |
| Version history | Event-sourced append-only log | Immutability, audit trail, time-travel queries |
| Real-time collaboration | SignalR WebSocket hub | Native .NET integration, scales to Azure SignalR Service |
| Signature crypto | .NET `System.Security.Cryptography` + BouncyCastle | FIPS-compliant, no external key custody |
| PDF export | QuestPDF | Code-first PDF generation, fully testable |
| Attachment storage | S3-compatible object store (MinIO for local dev) | Decoupled, swappable, CDN-ready |
| Auth | ASP.NET Core Identity + JWT + optional OAuth2 | Stateless API tokens, extensible to SSO |

---

## 6. Tech Stack

### Backend — `.NET 10` (ASP.NET Core Web API)

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 10 Minimal APIs + Controllers |
| ORM | Entity Framework Core 10 |
| Database | PostgreSQL 16 |
| Caching | Redis (StackExchange.Redis) |
| Real-time | SignalR |
| Auth | ASP.NET Core Identity · JWT Bearer · OpenIddict |
| Crypto | System.Security.Cryptography · BouncyCastle.Cryptography |
| PDF Generation | QuestPDF |
| Object Storage | AWSSDK.S3 (MinIO-compatible) |
| Testing | xUnit · FluentAssertions · Testcontainers |
| Observability | OpenTelemetry · Serilog · Seq |
| API Docs | Scalar (OpenAPI 3.1) |

### Frontend — `Vue 3`

| Layer | Technology |
|---|---|
| Framework | Vue 3 (Composition API) |
| Build | Vite 6 |
| State | Pinia |
| Router | Vue Router 4 |
| UI Components | Quasar Framework 2 |
| Rich Text Editor | TipTap 2 (ProseMirror-based) |
| Diff Viewer | vue-diff |
| HTTP Client | Axios |
| WebSocket | @microsoft/signalr |
| Testing | Vitest · Vue Test Utils · Playwright |

### Infrastructure (Self-hosted & Cloud-ready)

| Concern | Tool |
|---|---|
| Containerization | Docker + Docker Compose |
| Orchestration | Kubernetes (Helm charts provided) |
| CI/CD | GitHub Actions |
| Secrets | HashiCorp Vault / Azure Key Vault |
| Object Store | MinIO (local) · AWS S3 / Azure Blob (cloud) |

---

## 7. Data Model Overview

### `users`
```
id            UUID PK
email         VARCHAR UNIQUE NOT NULL
display_name  VARCHAR
public_key    TEXT               -- RSA/ECDSA public key for signature verification
created_at    TIMESTAMPTZ
last_login_at TIMESTAMPTZ
```

### `agreements`
```
id              UUID PK
title           VARCHAR NOT NULL
status          ENUM (draft | negotiating | pending_signatures | executed | revoked | expired)
created_by      UUID FK → users.id
created_at      TIMESTAMPTZ
executed_at     TIMESTAMPTZ
expires_at      TIMESTAMPTZ
is_private      BOOLEAN DEFAULT FALSE
```

### `agreement_versions`
```
id              UUID PK
agreement_id    UUID FK → agreements.id
version_number  INTEGER NOT NULL
content         JSONB NOT NULL         -- Structured clause tree
change_summary  TEXT
created_by      UUID FK → users.id
created_at      TIMESTAMPTZ
parent_version  UUID FK → agreement_versions.id
```

### `parties`
```
id              UUID PK
agreement_id    UUID FK → agreements.id
user_id         UUID FK → users.id
role            ENUM (drafter | signatory | witness | observer)
invited_at      TIMESTAMPTZ
accepted_at     TIMESTAMPTZ
```

### `amendments`
```
id              UUID PK
agreement_id    UUID FK → agreements.id
proposed_by     UUID FK → users.id
base_version    UUID FK → agreement_versions.id
diff_patch      JSONB NOT NULL
status          ENUM (pending | accepted | rejected | superseded)
proposed_at     TIMESTAMPTZ
resolved_at     TIMESTAMPTZ
```

### `signatures`
```
id              UUID PK
agreement_id    UUID FK → agreements.id
version_id      UUID FK → agreement_versions.id
party_id        UUID FK → parties.id
signature_data  TEXT NOT NULL          -- Base64-encoded cryptographic signature
algorithm       VARCHAR                -- e.g. ECDSA-P256-SHA256
signed_at       TIMESTAMPTZ
ip_address      INET
revoked_at      TIMESTAMPTZ
revoke_reason   TEXT
```

### `attachments`
```
id              UUID PK
agreement_id    UUID FK → agreements.id
version_id      UUID FK → agreement_versions.id
file_name       VARCHAR NOT NULL
content_type    VARCHAR
storage_key     VARCHAR NOT NULL       -- Object store path
sha256_hash     CHAR(64) NOT NULL      -- Binding hash at upload time
uploaded_by     UUID FK → users.id
uploaded_at     TIMESTAMPTZ
```

### `audit_events`
```
id              UUID PK
agreement_id    UUID FK → agreements.id
actor_id        UUID FK → users.id
event_type      VARCHAR NOT NULL       -- e.g. version.created, signature.added
payload         JSONB
occurred_at     TIMESTAMPTZ
ip_address      INET
```

---

## 8. API Surface

All endpoints are prefixed with `/api/v1`. Authentication requires a JWT Bearer token
unless marked `[public]`.

### Authentication

| Method | Endpoint | Description |
|---|---|---|
| POST | `/auth/register` | Register a new user account `[public]` |
| POST | `/auth/login` | Issue JWT access + refresh token pair `[public]` |
| POST | `/auth/refresh` | Rotate refresh token |
| POST | `/auth/logout` | Revoke refresh token |
| GET | `/auth/me` | Get authenticated user profile |
| PUT | `/auth/me/keys` | Upload or rotate public key |

### Agreements

| Method | Endpoint | Description |
|---|---|---|
| GET | `/agreements` | List agreements for authenticated user |
| POST | `/agreements` | Create a new agreement |
| GET | `/agreements/{id}` | Get agreement detail and current version |
| PATCH | `/agreements/{id}` | Update agreement metadata (title, expiry) |
| DELETE | `/agreements/{id}` | Soft-delete / revoke agreement |

### Versions

| Method | Endpoint | Description |
|---|---|---|
| GET | `/agreements/{id}/versions` | List all versions |
| POST | `/agreements/{id}/versions` | Save a new version snapshot |
| GET | `/agreements/{id}/versions/{versionId}` | Get specific version content |
| GET | `/agreements/{id}/versions/{a}/diff/{b}` | Diff two versions |

### Parties

| Method | Endpoint | Description |
|---|---|---|
| GET | `/agreements/{id}/parties` | List all parties |
| POST | `/agreements/{id}/parties` | Invite a party by email |
| PUT | `/agreements/{id}/parties/{partyId}/accept` | Accept party invitation |
| DELETE | `/agreements/{id}/parties/{partyId}` | Remove a party (drafter only) |

### Amendments

| Method | Endpoint | Description |
|---|---|---|
| GET | `/agreements/{id}/amendments` | List amendments |
| POST | `/agreements/{id}/amendments` | Propose an amendment |
| PUT | `/agreements/{id}/amendments/{amendId}/accept` | Accept an amendment |
| PUT | `/agreements/{id}/amendments/{amendId}/reject` | Reject an amendment |

### Signatures

| Method | Endpoint | Description |
|---|---|---|
| GET | `/agreements/{id}/signatures` | List all signatures |
| POST | `/agreements/{id}/signatures` | Submit a cryptographic signature |
| DELETE | `/agreements/{id}/signatures/{sigId}` | Revoke own signature (with reason) |

### Attachments

| Method | Endpoint | Description |
|---|---|---|
| GET | `/agreements/{id}/attachments` | List attachments |
| POST | `/agreements/{id}/attachments` | Upload an attachment |
| GET | `/agreements/{id}/attachments/{attachId}` | Download attachment |
| DELETE | `/agreements/{id}/attachments/{attachId}` | Remove attachment |

### Export

| Method | Endpoint | Description |
|---|---|---|
| POST | `/agreements/{id}/export/pdf` | Generate and return court-ready PDF bundle |
| GET | `/agreements/{id}/export/audit` | Export full audit event log as JSON |

### Real-time (SignalR)

| Hub | Event | Direction | Description |
|---|---|---|---|
| `/hubs/agreement` | `VersionSaved` | Server → Client | Notify when a new version is saved |
| `/hubs/agreement` | `AmendmentProposed` | Server → Client | Notify all parties of a new amendment |
| `/hubs/agreement` | `SignatureAdded` | Server → Client | Notify when a party signs |
| `/hubs/agreement` | `AgreementExecuted` | Server → Client | Broadcast when all signatures are collected |

---

## 9. MVP Scope

The MVP delivers the core loop: **create → negotiate → sign → export**.

### ✅ In Scope for MVP

- User registration, login, JWT auth, public key upload
- Create, edit, and version agreements (rich text, clause structure)
- Invite parties by email; accept/decline invitations
- Propose, accept, and reject amendments with full diff view
- Cryptographic signature submission and verification
- Basic attachment upload (PDF and image support)
- Court-ready PDF bundle export
- Email notifications for all lifecycle events
- Full audit event log per agreement
- Responsive Vue 3 SPA with dashboard and agreement detail views
- Docker Compose environment for local development
- OpenAPI documentation via Scalar

### ❌ Out of Scope for MVP

- SSO / OAuth2 social login
- In-app real-time collaborative editing (SignalR hub is scaffolded but editing is
  optimistic-lock only)
- Agreement templates library
- Witness / notary workflows
- Mobile native apps
- Blockchain anchoring
- Payment / escrow integration
- Advanced analytics dashboard

---

## 10. Roadmap & Future Enhancements

### v1.1 — Collaboration & Templates
- [ ] Real-time collaborative editing via SignalR operational transforms
- [ ] Template library: NDA, freelance contract, lease agreement, MOU, and more
- [ ] Clause library: reusable, pre-approved clause building blocks
- [ ] In-agreement commenting and threaded discussion

### v1.2 — Identity & Verification
- [ ] OAuth2 / OpenID Connect social login (Google, Microsoft, LinkedIn)
- [ ] Identity assertion: upload government ID for enhanced party verification
- [ ] Witness and notary co-signer workflows
- [ ] Verified organization accounts with domain verification

### v1.3 — Legal Integrations
- [ ] DocuSign and Adobe Sign import/export
- [ ] Jurisdiction-aware clause suggestions (US, EU, UK)
- [ ] Legal metadata tagging (governing law, dispute resolution, venue)
- [ ] Attorney review invitation (read-only counsel access)

### v1.4 — Trust & Immutability
- [ ] Blockchain anchoring: hash agreements to a public ledger (Ethereum / Polygon)
- [ ] Timestamp Authority (TSA) RFC 3161 time-stamping for signatures
- [ ] Decentralized identity (DID) support

### v1.5 — Automation & Integrations
- [ ] Webhook outbound events for all agreement state transitions
- [ ] Zapier / Make.com integration
- [ ] Payment and escrow triggers on agreement execution
- [ ] API key management for programmatic agreement creation

### v2.0 — Mobile & Offline
- [ ] iOS and Android apps (Capacitor or MAUI Hybrid)
- [ ] Offline-capable agreement drafting with sync on reconnect
- [ ] Biometric signature capture on mobile

---

## 11. Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 22+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [pnpm](https://pnpm.io/) (recommended) or npm

### 1. Clone the Repository

```bash
git clone https://github.com/your-org/accordly.git
cd accordly
```

### 2. Start Infrastructure (PostgreSQL, Redis, MinIO)

```bash
docker compose up -d
```

### 3. Configure the Backend

```bash
cp src/Accordly.Api/appsettings.Development.json.example \
   src/Accordly.Api/appsettings.Development.json
# Edit the file to set your JWT secret, DB connection string, etc.
```

### 4. Run Database Migrations

```bash
cd src/Accordly.Api
dotnet ef database update
```

### 5. Start the Backend

```bash
dotnet run --project src/Accordly.Api
# API available at https://localhost:7080
# OpenAPI docs at https://localhost:7080/scalar
```

### 6. Start the Frontend

```bash
cd frontend
pnpm install
pnpm dev
# App available at http://localhost:5173
```

### Repository Structure

```
accordly/
├── src/
│   ├── Accordly.Api/            # ASP.NET Core Web API
│   ├── Accordly.Core/           # Domain models, interfaces, business logic
│   ├── Accordly.Infrastructure/ # EF Core, repositories, storage, crypto
│   └── Accordly.Tests/          # xUnit test projects
├── frontend/                    # Vue 3 SPA
│   ├── src/
│   │   ├── components/
│   │   ├── pages/
│   │   ├── stores/                # Pinia stores
│   │   └── composables/
│   └── tests/
├── docker-compose.yml
├── helm/                          # Kubernetes Helm chart
├── .github/workflows/             # CI/CD pipelines
└── docs/                          # Architecture decision records (ADRs)
```

---

## 12. Contributing

Contributions are welcome and appreciated. Accordly follows a conventional commit and
pull-request workflow.

### Getting Involved

1. **Browse open issues** — look for issues tagged `good first issue` or `help wanted`.
2. **Propose a feature** — open a GitHub Discussion before building something significant.
3. **Report a bug** — use the Bug Report issue template and include reproduction steps.

### Pull Request Process

1. Fork the repository and create a feature branch from `main`:
   ```bash
   git checkout -b feat/your-feature-name
   ```
2. Write tests for all new behavior. PRs that reduce test coverage will not be merged.
3. Follow the existing code style. Backend: run `dotnet format`. Frontend: run `pnpm lint`.
4. Write [Conventional Commits](https://www.conventionalcommits.org/) messages:
   ```
   feat(signatures): add ECDSA P-384 support
   fix(export): handle agreements with no attachments
   docs(readme): update API surface table
   ```
5. Open a pull request against `main` with a clear description of the change and why it
   was made.
6. All PRs require at least **one approving review** and a passing CI pipeline.

### Code of Conduct

This project adheres to the [Contributor Covenant Code of Conduct](CODE_OF_CONDUCT.md).
By participating, you agree to uphold a respectful, inclusive environment for all
contributors.

### Security Vulnerabilities

Please **do not** open public GitHub issues for security vulnerabilities. Instead, email
`security@accordly.io` with a detailed description. We aim to acknowledge within 48 hours
and provide a fix timeline within 7 days.

---

## 13. License

Accordly is released under the [MIT License](LICENSE).

```
Copyright (c) 2026 Accordly Contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction...
```

---

<div align="center">

**Built with care for people who need their word to mean something.**

[Report a Bug](https://github.com/your-org/accordly/issues) · [Request a Feature](https://github.com/your-org/accordly/discussions) · [Read the Docs](https://docs.accordly.io)

</div>
