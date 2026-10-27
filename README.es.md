# Plantilla de inicio Full Stack con Clean Architecture (.NET 10 + Vue 3)

[![Language: English](https://img.shields.io/badge/Language-English-blue.svg)](README.md)
[![Versión en Español](https://img.shields.io/badge/Versión-Español-red.svg)](#)
[![CI](https://github.com/Nain9Dev/template-dotnet-clean-arch/actions/workflows/ci.yml/badge.svg)](https://github.com/Nain9Dev/template-dotnet-clean-arch/actions/workflows/ci.yml)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Vue](https://img.shields.io/badge/Vue-3.5-4FC08D?logo=vuedotjs)](https://vuejs.org/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql)](https://www.postgresql.org/)
[![Coste](https://img.shields.io/badge/Cost-0.00_%E2%82%AC-22c55e)](#filosofía-de-cero-coste-facturable)

Plantilla de inicio full-stack de nivel empresarial construida con **.NET 10 Web API**, **Clean Architecture**, **Entity Framework Core 10** para mutaciones, **Dapper** para consultas de alto rendimiento y un cliente moderno en **Vue 3 + TypeScript**. Integrada en el ecosistema **NainDev** con soporte de alojamiento a coste cero.

---

## Arquitectura

```mermaid
graph TD
    subgraph Presentation ["Capa de Presentación"]
        VueClient["SPA Vue 3<br/>TypeScript + Vite"]
        ApiControllers["Controladores ASP.NET Core<br/>RFC 7807 ProblemDetails"]
        EcosystemBar["<nain-ecosystem-bar><br/>Navegación Suite"]
    end

    subgraph Application ["Capa de Aplicación"]
        AppServices["WorkItemService<br/>Casos de uso y orquestación"]
        AppDtos["DTOs y Records<br/>Atributos de validación"]
        ReadContract["IWorkItemReadRepository<br/>Contrato de consulta CQRS"]
    end

    subgraph Domain ["Capa de Dominio (Core)"]
        WorkItemEntity["WorkItem (Agregado raíz)<br/>Invariantes de negocio"]
        Enums["Enums y Objetos de Valor<br/>Status, Priority"]
        WriteContract["IWorkItemRepository<br/>Contrato de persistencia"]
        DomainExceptions["DomainException<br/>EntityNotFoundException"]
    end

    subgraph Infrastructure ["Capa de Infraestructura"]
        EfContext["ApplicationDbContext<br/>EF Core 10 (PostgreSQL / SQLite)"]
        DapperRepo["WorkItemDapperReadRepository<br/>Consultas parametrizadas Dapper"]
        EfRepo["WorkItemEfRepository<br/>Seguimiento de cambios y escrituras"]
    end

    VueClient -->|HTTP / JSON| ApiControllers
    ApiControllers --> AppServices
    AppServices --> Domain
    AppServices --> ReadContract
    AppServices --> WriteContract
    DapperRepo -.->|Implementa| ReadContract
    EfRepo -.->|Implementa| WriteContract
    DapperRepo --> EfContext
    EfRepo --> EfContext
```

## Stack tecnológico

- **Backend**: .NET 10 SDK, ASP.NET Core Web API (Controllers), EF Core 10 (Npgsql), Dapper 2.1, xUnit, FluentAssertions.
- **Frontend**: Vue 3.5 (Composition API con `<script setup>`), TypeScript estricto, Vite, Pinia, Axios.
- **Base de datos**: PostgreSQL 16 (producción con Docker Compose), SQLite en memoria (para pruebas rápidas de integración).
- **Contenedores**: Docker Compose para orquestación multi-servicio.

## Inicio rápido

### 1. Docker Compose (Recomendado)

```bash
docker compose up --build
```

- Frontend: <http://localhost:5173>
- Backend Swagger: <http://localhost:5000/swagger>
- Comprobación de salud: <http://localhost:5000/health>

### 2. Desarrollo local manual

**Backend**:
```bash
cd backend
dotnet restore template-dotnet-clean-arch.slnx
dotnet test template-dotnet-clean-arch.slnx
dotnet run --project src/Presentation/Template.Api
```

**Frontend**:
```bash
cd frontend
npm install
npm run dev
```

## Filosofía de cero coste facturable

Esta plantilla sigue las directrices del Engineering Handbook de NainDev:
- Demostración y pruebas locales mediante FOSS y recursos sin coste.
- Pruebas automatizadas en CI con GitHub Actions.
- Sin servicios cloud de pago vinculados por defecto.

## Licencia

Distribuido bajo la [Licencia MIT](LICENSE).
