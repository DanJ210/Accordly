# Contributing to Accordly

Thank you for your interest in contributing to Accordly! This document explains how to get involved, what we expect from contributors, and how to navigate the development workflow. We want contributing to feel welcoming and friction-free, so please read this before opening your first issue or pull request.

---

## Table of Contents

1. [Code of Conduct](#code-of-conduct)
2. [Ways to Contribute](#ways-to-contribute)
3. [Development Setup](#development-setup)
4. [Branch Strategy](#branch-strategy)
5. [Commit Conventions](#commit-conventions)
6. [Pull Request Process](#pull-request-process)
7. [Coding Standards](#coding-standards)
8. [Testing Requirements](#testing-requirements)
9. [Issue Guidelines](#issue-guidelines)
10. [Security Vulnerabilities](#security-vulnerabilities)

---

## Code of Conduct

This project is governed by a simple principle: **be kind and constructive.** We welcome contributors of all experience levels. Harassment, dismissiveness, or gatekeeping of any kind will not be tolerated. If you experience or witness behavior that violates this spirit, please reach out to the maintainers at the email listed in the repository profile.

---

## Ways to Contribute

You don't need to write code to make a meaningful contribution:

- **Bug reports** — If something is broken, a clear, reproducible report is enormously valuable.
- **Feature requests** — Open an issue to discuss an idea before building it.
- **Documentation** — Typo fixes, clearer explanations, and missing docs are always welcome.
- **Code** — Bug fixes, new features, and test coverage improvements all help move the project forward.
- **Design feedback** — If you have UX or product opinions, open a discussion.

---

## Development Setup

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 22+](https://nodejs.org/) and [pnpm](https://pnpm.io/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Setup Steps

```bash
# 1. Fork and clone
git clone https://github.com/<your-username>/accordly.git
cd accordly

# 2. Add the upstream remote
git remote add upstream https://github.com/danj210/accordly.git

# 3. Start infrastructure (PostgreSQL + MinIO)
docker compose up -d

# 4. Apply migrations and run the API
cd src/Accordly.Api
dotnet ef database update
dotnet run

# 5. Start the frontend dev server
cd ../../frontend
pnpm install
pnpm dev
```

The API runs at `https://localhost:5001` and the frontend dev server at `http://localhost:5173`.

### Editor Recommendations

- **VS Code** with the C# Dev Kit, Volar (Vue), ESLint, and Tailwind CSS IntelliSense extensions.
- **Rider** for a full-featured .NET IDE experience.
- An `.editorconfig` is included in the repository — please ensure your editor respects it.

---

## Branch Strategy

| Branch | Purpose |
|---|---|
| `main` | Stable, always deployable |
| `develop` | Integration branch for in-progress work |
| `feature/<name>` | New features |
| `fix/<name>` | Bug fixes |
| `chore/<name>` | Tooling, dependencies, non-functional changes |
| `docs/<name>` | Documentation-only changes |

All pull requests should target `develop`, not `main`. Merges from `develop` to `main` are made by maintainers on a release cycle.

---

## Commit Conventions

Accordly uses [Conventional Commits](https://www.conventionalcommits.org/). Every commit message must follow this format:

```
<type>(<scope>): <short description>

[optional body]

[optional footer]
```

### Types

| Type | When to use |
|---|---|
| `feat` | A new feature |
| `fix` | A bug fix |
| `docs` | Documentation changes only |
| `style` | Formatting, whitespace — no logic change |
| `refactor` | Code restructure with no behavior change |
| `test` | Adding or updating tests |
| `chore` | Build scripts, dependencies, tooling |
| `perf` | Performance improvements |
| `ci` | CI/CD pipeline changes |

### Scopes

Use the domain area as the scope: `agreements`, `versions`, `signatures`, `attachments`, `export`, `auth`, `audit`, `frontend`, `infra`, `db`.

### Examples

```
feat(signatures): add guest signing via tokenized invite link
fix(export): correct page numbering in court-ready PDF bundle
docs(readme): update environment variable table
test(agreements): add integration tests for version diffing
chore(deps): upgrade QuestPDF to 2025.1.0
```

Breaking changes must include `BREAKING CHANGE:` in the commit footer and a `!` after the type:

```
feat(auth)!: replace session tokens with rotating JWT refresh tokens

BREAKING CHANGE: Clients must update to the new /auth/refresh endpoint.
```

---

## Pull Request Process

1. **Sync with upstream** before starting work:
   ```bash
   git fetch upstream
   git rebase upstream/develop
   ```

2. **Create a focused branch** from `develop`:
   ```bash
   git checkout -b feature/your-feature-name
   ```

3. **Write tests** for any new functionality. PRs that reduce test coverage will be asked to add tests before merging.

4. **Run the full test suite** locally before pushing:
   ```bash
   # Backend
   dotnet test

   # Frontend unit
   pnpm test

   # Frontend E2E
   pnpm test:e2e
   ```

5. **Open a pull request** against `develop` with:
   - A clear title following the commit convention (e.g., `feat(signatures): add guest signing flow`)
   - A description of **what** changed and **why**
   - Screenshots or recordings for UI changes
   - A note on any migrations or infrastructure changes

6. **Address review feedback** promptly. If a review comment is unclear, ask for clarification rather than guessing.

7. **Squash your commits** before merge if the branch history is noisy. Maintainers may ask for this.

### PR Checklist

Before marking your PR ready for review, confirm:

- [ ] All tests pass locally
- [ ] New functionality is covered by tests
- [ ] Commit messages follow the Conventional Commits format
- [ ] No secrets, credentials, or personal data are committed
- [ ] Database migrations are included if the data model changed
- [ ] `appsettings.Development.json` changes are reflected in documentation

---

## Coding Standards

### Backend (C# / .NET 10)

- Follow the [Microsoft C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).
- Use `async`/`await` consistently — avoid `.Result` and `.Wait()`.
- Prefer `record` types for immutable DTOs and value objects.
- Keep domain logic in `Accordly.Domain` and `Accordly.Application`. Infrastructure concerns belong in `Accordly.Infrastructure`.
- All public API surface must have XML documentation comments.
- Use `Result<T>` / discriminated union patterns for error handling in the application layer — no raw exceptions for domain failures.

### Frontend (Vue 3 / TypeScript)

- Use the Composition API exclusively — no Options API.
- Keep components focused: if a component exceeds ~250 lines, consider splitting it.
- Co-locate composables with the feature they belong to under `features/<name>/composables/`.
- Type everything — `any` is disallowed except in explicitly commented escape hatches.
- Follow the [Vue Style Guide](https://vuejs.org/style-guide/) (priority A and B rules are enforced by ESLint).
- Tailwind classes should be ordered consistently — use the Prettier Tailwind plugin.

### General

- No magic strings — use enums, constants, or configuration.
- Delete commented-out code before opening a PR.
- Avoid abbreviations in identifiers unless they are universally understood (e.g., `Id`, `Url`, `Pdf`).

---

## Testing Requirements

| Layer | Tool | Minimum Expectation |
|---|---|---|
| Domain / Application unit | xUnit | All use cases covered |
| API integration | xUnit + Testcontainers | Happy path + key error paths per endpoint |
| Frontend unit | Vitest + Vue Testing Library | All composables and non-trivial components |
| End-to-end | Playwright | Critical user journeys (sign up, draft, sign, export) |

New features must include tests at the appropriate layer(s). Bug fixes must include a regression test that would have caught the bug.

---

## Issue Guidelines

### Bug Reports

Please include:
- A clear, concise description of the bug
- Steps to reproduce (numbered, specific)
- Expected behavior vs. actual behavior
- Environment details (.NET version, browser, OS)
- Relevant logs or screenshots

Use the **Bug Report** issue template.

### Feature Requests

Please include:
- The problem you're trying to solve (not just the solution)
- How you currently work around it, if at all
- Any prior art or references to similar implementations

Use the **Feature Request** issue template.

### Good First Issues

Issues labeled `good first issue` are intentionally scoped and well-documented. They're a great starting point if this is your first contribution.

---

## Security Vulnerabilities

**Please do not open a public GitHub issue for security vulnerabilities.**

If you discover a security issue, please report it privately by emailing the maintainers. Include a description of the vulnerability, steps to reproduce, and any relevant proof-of-concept. We will respond within 72 hours and work with you on a coordinated disclosure timeline.

---

Thank you for helping make Accordly better. Every contribution — no matter how small — moves the project forward.
