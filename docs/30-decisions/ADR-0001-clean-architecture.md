# ADR-0001: Clean Architecture with EF Core Writes and Dapper Reads

## Context
Full stack enterprise applications often suffer from either ORM overhead on read paths or complex domain logic leakages when using micro-ORMs exclusively for mutations.

## Decision
Adopt a 4-layer Clean Architecture solution:
- **Domain**: Pure business entities, domain exceptions, and write contracts (`IWorkItemRepository`).
- **Application**: Use cases, DTOs, and read contracts (`IWorkItemReadRepository`).
- **Infrastructure**: EF Core `ApplicationDbContext` for transactional write persistence and Dapper for zero-allocation query projection.
- **Presentation**: ASP.NET Core controllers with ProblemDetails RFC 7807 middleware and Vue 3 TypeScript client.

## Trade-Off Analysis
| Dimension | Pure EF Core | Pure Dapper | Hybrid EF Core + Dapper (Chosen) |
| :--- | :--- | :--- | :--- |
| **Write Integrity** | High (Unit of Work, change tracker) | Low (manual SQL statements) | **High** (EF Core transactional aggregates) |
| **Read Latency** | Moderate (projection translation overhead) | Ultra-fast (raw SQL mapping) | **Ultra-fast** (Dapper parameterized queries) |
| **Domain Isolation** | Moderate | Poor | **Strict** (domain is framework-agnostic) |
| **Maintenance Cost** | Low | High (repetitive SQL boilerplate) | **Balanced** (EF manages migrations and mutations; Dapper powers read queries) |

## Status
Approved.
