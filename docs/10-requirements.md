# Functional & Architectural Requirements

## Core Objectives
1. **Clean Architecture Separation**: Ensure Domain layer has zero dependencies on external frameworks or databases.
2. **Deterministic Dual Persistence**: EF Core for domain entity write operations; Dapper for high-throughput read models.
3. **Cross-Engine Support**: Seamless PostgreSQL support in production/Docker, with SQLite fallback for zero-overhead local development and CI testing.
4. **Accessible Frontend**: Vue 3 + TypeScript client satisfying WCAG 2.2 AA accessibility requirements and integrating `<nain-ecosystem-bar>`.
5. **Zero Billable Cost (0.00 €)**: Standard containerized deployment runnable on free VPS tiers, local Docker Compose, or container PaaS.

## Traceability Matrix
- `ARCH-001`: Independent domain entities and aggregate roots (`WorkItem`).
- `ARCH-002`: CQRS read/write separation (`IWorkItemRepository` via EF Core, `IWorkItemReadRepository` via Dapper).
- `ARCH-003`: Centralized error normalization with RFC 7807 ProblemDetails middleware.
- `ARCH-004`: Reusable GitHub Actions CI pipeline execution (`Nain9Dev/.github`).
