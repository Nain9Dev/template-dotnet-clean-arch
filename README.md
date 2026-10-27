<picture>
  <source media="(prefers-color-scheme: dark)" srcset="docs/assets/banner-dark.svg">
  <source media="(prefers-color-scheme: light)" srcset="docs/assets/banner-light.svg">
  <img alt="Template .NET Clean Arch Architectural Blueprint" src="docs/assets/banner-dark.svg" width="100%">
</picture>

# Full Stack Clean Architecture Starter (.NET 10 + Vue 3)

[![Language: English](https://img.shields.io/badge/Language-English-blue.svg)](#)
[![Versión en Español](https://img.shields.io/badge/Versión-Español-red.svg)](README.es.md)
[![CI](https://github.com/Nain9Dev/template-dotnet-clean-arch/actions/workflows/ci.yml/badge.svg)](https://github.com/Nain9Dev/template-dotnet-clean-arch/actions/workflows/ci.yml)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Vue](https://img.shields.io/badge/Vue-3.5-4FC08D?logo=vuedotjs)](https://vuejs.org/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql)](https://www.postgresql.org/)
[![Cost](https://img.shields.io/badge/Cost-0.00_%E2%82%AC-22c55e)](#zero-billable-cost-philosophy)

Enterprise-grade full-stack starter template built with **.NET 10 Web API**, **Clean Architecture**, **Entity Framework Core 10** for mutations, **Dapper** for high-throughput queries, and a modern **Vue 3 + TypeScript** client. Integrated into the **NainDev Ecosystem Suite** with zero-billable-cost hosting support.

---

## Architecture Overview

```mermaid
graph TD
    subgraph Presentation ["Presentation Layer"]
        VueClient["Vue 3 SPA<br/>TypeScript + Vite"]
        ApiControllers["ASP.NET Core Controllers<br/>RFC 7807 ProblemDetails"]
        EcosystemBar["<nain-ecosystem-bar><br/>Suite Navigation"]
    end

    subgraph Application ["Application Layer"]
        AppServices["WorkItemService<br/>Use Cases & Orchestration"]
        AppDtos["DTOs & Records<br/>Validation Attributes"]
        ReadContract["IWorkItemReadRepository<br/>CQRS Query Contract"]
    end

    subgraph Domain ["Domain Layer (Core)"]
        WorkItemEntity["WorkItem (Aggregate Root)<br/>Business Invariants"]
        Enums["Enums & Value Objects<br/>Status, Priority"]
        WriteContract["IWorkItemRepository<br/>Domain Persistence Contract"]
        DomainExceptions["DomainException<br/>EntityNotFoundException"]
    end

    subgraph Infrastructure ["Infrastructure Layer"]
        EfContext["ApplicationDbContext<br/>EF Core 10 (PostgreSQL / SQLite)"]
        DapperRepo["WorkItemDapperReadRepository<br/>Dapper Parameterized Queries"]
        EfRepo["WorkItemEfRepository<br/>Change Tracking & Writes"]
    end

    VueClient -->|REST HTTP / JSON| ApiControllers
    ApiControllers --> AppServices
    AppServices --> Domain
    AppServices --> ReadContract
    AppServices --> WriteContract
    Infrastructure -.->|Implements| WriteContract
    Infrastructure -.->|Implements| ReadContract
    EfRepo --> EfContext
    DapperRepo --> EfContext
```

### Dependency Rule
- **Domain**: Pure business rules with zero dependencies on frameworks, databases, or third-party libraries.
- **Application**: Depends only on Domain. Exposes business contracts, DTOs, and use case orchestration.
- **Infrastructure**: Implements interfaces defined by Domain and Application using EF Core and Dapper.
- **Presentation**: ASP.NET Core controllers and the Vue 3 single page application.

---

## Persistence Strategy: EF Core Writes + Dapper Reads

| Operation Type | Tool | Rationale | Performance & Integrity |
| :--- | :--- | :--- | :--- |
| **Domain Mutations** | **EF Core 10** | Full change tracking, identity map, aggregate root lifecycle encapsulation, and schema migrations. | Transactional safety and zero risk of partial updates. |
| **High-Throughput Queries** | **Dapper** | Zero object allocation overhead, explicit SQL parameterization, and instant dictionary mapping. | Sub-millisecond latency for complex dashboards and listings. |

---

## Quickstart

### Option 1: Local Development (Instant SQLite Fallback)

1. **Backend**:
   ```bash
   cd backend
   dotnet run --project src/Presentation/Template.Api/Template.Api.csproj
   # Runs on http://localhost:5000 (OpenAPI doc at http://localhost:5000/openapi/v1.json)
   ```

2. **Frontend**:
   ```bash
   cd frontend
   npm install
   npm run dev
   # Accessible at http://localhost:5173
   ```

### Option 2: Docker Compose (PostgreSQL 16)

```bash
docker compose up --build
```
- Frontend client: `http://localhost:8080`
- API backend: `http://localhost:5000`
- PostgreSQL: `localhost:5432`

---

## API Reference

| Verb | Path | Description | Response |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/v1/health` | Service health status check | `200 OK` |
| `GET` | `/api/v1/work-items` | List items (optional `?status=`) via Dapper | `200 OK` |
| `GET` | `/api/v1/work-items/{id}` | Retrieve single item by ID | `200 OK` / `404 ProblemDetails` |
| `POST` | `/api/v1/work-items` | Create new work item via EF Core | `201 Created` / `400 ProblemDetails` |
| `PUT` | `/api/v1/work-items/{id}` | Update title, description, priority | `200 OK` / `404 ProblemDetails` |
| `PATCH` | `/api/v1/work-items/{id}/status` | Transition item status | `200 OK` / `400 ProblemDetails` |
| `DELETE` | `/api/v1/work-items/{id}` | Delete item | `204 NoContent` / `404 ProblemDetails` |

---

## Quality & Verification Gates

```bash
# Run unit tests and integration tests
dotnet test backend/template-dotnet-clean-arch.slnx

# Run frontend type-check and production build
cd frontend && npm run build
```

Both tests run automatically on every pull request via centralized reusable workflows in `Nain9Dev/.github`.

---

## Zero Billable Cost Philosophy

This repository is designed to be hosted entirely within free-tier infrastructure:
- **Client**: Static export deployable to GitHub Pages, Cloudflare Pages, or Netlify at **0.00 €**.
- **API & DB**: Multi-stage Linux container runnable on free cloud instances or local container runtimes.
- **Observability**: Uptime monitoring integrated with [status.naindev.com](https://status.naindev.com/).

---

## License

MIT License. Free for personal and commercial projects.
