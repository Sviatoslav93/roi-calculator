# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

ROI Calculator is a full-stack application for building and publishing ROI forms. Clients can use these forms to calculate return on investment. The architecture uses layered DDD (Domain-Driven Design) with CQRS patterns.

**Technology Stack:**
- **Frontend:** Nuxt 4 + Vue 3 + TypeScript + PrimeVue (UI components)
- **Backend:** .NET 10 with ASP.NET Core Minimal APIs + MediatR + EF Core
- **Database:** PostgreSQL (with in-memory option for development)
- **Architecture:** Vertical Slice (API layer), Clean Architecture (Domain/Application/Infrastructure)

## Project Structure

```
roi-calculator/
├── web/roi-form/                 # Nuxt frontend application
│   ├── app/                       # Nuxt app components
│   ├── nuxt.config.ts            # PrimeVue theme config
│   └── package.json              # pnpm workspace
├── backend/                       # .NET 10 solution
│   ├── RoiCalculator.slnx        # Solution file
│   ├── src/
│   │   ├── Common/Domain.Common/ # Base classes (AggregateRoot, Entity, ValueObject, IRepository, IUnitOfWork)
│   │   ├── Hosting/RoiCalculator.AppHost/  # Aspire orchestration
│   │   └── Services/RoiForm/
│   │       ├── RoiForm.Api/      # Endpoints (Minimal APIs, vertical slices)
│   │       ├── RoiForm.Application/    # Commands/Queries (MediatR handlers)
│   │       ├── RoiForm.Domain/   # Aggregates, value objects, entities, repositories (interfaces)
│   │       └── RoiForm.Infrastructure/ # EF Core DbContext, repository implementations
│   └── Tests/RoiFrom/            # xUnit tests
└── docker-compose.yml            # PostgreSQL + API (from root)
```

## Core Patterns & Conventions

### Backend Architecture

**Vertical Slice Architecture (API):**
- Each feature in `RoiForm.Api/Endpoints/<FeatureName>/` contains: request DTOs, validator, endpoint handler
- Endpoint handler is a static class with `Map(IEndpointRouteBuilder)` method, called from `Program.cs`
- No MVC controllers; Minimal APIs only
- Request/response DTOs are records for immutability
- Validators registered globally via `AddValidatorsFromAssemblyContaining` (FluentValidation)

**Domain-Driven Design (Domain/Application/Infrastructure):**
- **Domain:** Aggregates (FormTemplate), value objects (Formula), entities (FormField), repository interfaces
- **Application:** Commands/Queries (MediatR records), command/query handlers
- **Infrastructure:** AppDbContext (EF Core), repository implementations, DatabaseSeeder
- **Error Handling:** Result<T> from FunctionalPrimitives library for functional error handling; map to ProblemDetails in API layer

**Code Style:**
- Primary constructors: `public class Handler(IRepository repo, IUnitOfWork unitOfWork)`
- Records for commands, queries, DTOs (immutability)
- No async suffix: `GetUser()` not `GetUserAsync()`
- Self-documenting code; no inline comments
- All API errors return ProblemDetails (RFC 7807) via `TypedResults.Problem()` or `Results.ValidationProblem()`
- Global exception handler in Program.cs via `app.UseExceptionHandler()` with GlobalExceptionHandler behavior

## Build, Run & Test Commands

### Frontend (Nuxt)

```bash
cd web/roi-form
npm install        # Install dependencies
npm run dev        # Development server on port 3000
npm run build      # Production build
npm run preview    # Preview production build
```

### Backend (.NET)

```bash
cd backend

# Build and run
dotnet build
dotnet run --project src/Services/RoiForm/RoiForm.Api            # API only
dotnet run --project src/Hosting/RoiCalculator.AppHost           # With Aspire (PostgreSQL included)
dotnet watch run --project src/Services/RoiForm/RoiForm.Api      # Watch mode

# Tests (xUnit)
dotnet test                                                        # Run all tests
dotnet test Tests/RoiFrom/RoiForm.Api.Tests                      # Integration tests
dotnet test Tests/RoiFrom/RoiCalculator.Core.Tests               # Domain/Application tests
dotnet test Tests/RoiFrom/RoiForm.Api.Tests --filter "RoiFormsTests"  # Single test class
dotnet test Tests/RoiFrom/RoiForm.Api.Tests --filter "FullyQualifiedName~RoiFormsTests.Create_WithValidRequest_Returns201WithId"

# Database migrations (EF Core)
dotnet ef database update --project src/Services/RoiForm/RoiForm.Infrastructure --startup-project src/Services/RoiForm/RoiForm.Api
dotnet ef migrations add <MigrationName> --project src/Services/RoiForm/RoiForm.Infrastructure --startup-project src/Services/RoiForm/RoiForm.Api
```

### Docker

```bash
docker-compose up -d       # Start PostgreSQL and API
docker-compose down        # Stop services
docker-compose logs -f api # View API logs
```

## Key Files to Understand

- **Program.cs:** `backend/src/Services/RoiForm/RoiForm.Api/Program.cs` — Service registration, middleware, endpoint mapping, health checks, observability
- **Configuration:** `backend/src/Services/RoiForm/RoiForm.Api/appsettings*.json` — Connection strings, logging, feature flags
- **DbContext:** `backend/src/Services/RoiForm/RoiForm.Infrastructure/Data/AppDbContext.cs` — EF Core setup with auto-auditing (CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
- **Domain Model:** `backend/src/Services/RoiForm/RoiForm.Domain/FormManagement/FormTemplate.cs` — Aggregate root with static factory Create(), status transitions (Publish, Unpublish, Archive)
- **Error Mapping:** `backend/src/Services/RoiForm/RoiForm.Api/Extensions/ProblemDetailsExtensions.cs` — Maps functional errors to HTTP status codes

## Important Features & Behaviors

- **Feature Flags:** `UseInMemoryDatabase` in Development config enables in-memory DB (no PostgreSQL required for development)
- **Database Seeding:** Runs on startup via DatabaseSeeder injected in Program.cs
- **Health Checks:** `/health`, `/health/live`, `/health/ready` endpoints
- **CORS:** LocalDev policy for localhost in development
- **OpenTelemetry:** Full tracing and metrics instrumentation (console exporters in dev, OTEL Protocol in production)
- **Serilog:** Structured logging with trace ID correlation

## Testing

**Framework:** xUnit with FluentAssertions

**Integration Tests (RoiForm.Api.Tests):**
- Uses `WebApplicationFactory<Program>` via `RoiCalculatorFactory` custom fixture
- Tests create HTTP requests and verify responses
- Test class structure: RoiFormsTests tests all FormTemplate endpoints

**Domain/Unit Tests (RoiCalculator.Core.Tests):**
- Tests aggregates, value objects, domain logic
- Example: FormTemplateTests, FormulaTests

## Adding a New Endpoint

1. Create `RoiForm.Api/Endpoints/<FeatureName>/` folder
2. Add request DTO class with validation attributes (`[Required]`, `[MaxLength]`, etc.)
3. Add validator class extending FluentValidation.AbstractValidator<T> (auto-registered)
4. Add endpoint handler class:
   - Static class with static `Map(IEndpointRouteBuilder app)` method
   - Inside Map(), call `app.MapPost(...)` or `app.MapGet(...)` with handler lambda
   - Handler receives request, `ISender sender` (MediatR), `CancellationToken ct`
   - Return `Results.Created()`, `Results.Ok()`, or `TypedResults.Problem()`
5. Call from Program.cs: `app.Map<FeatureName>Endpoints()`

Example: CreateFormTemplateEndpoint.cs
- MapPost("/form-templates", Handle)
- Handle maps CreateFormTemplateRequest to CreateFormTemplateCommand
- Send command via MediatR, convert Result<Guid> to IResult using extension

## HTTP API Design

**FormTemplates endpoints:**
- POST `/form-templates` — Create
- GET `/form-templates/{id}` — Get by ID
- GET `/form-templates` — List (paginated)
- PATCH `/form-templates/{id}` — Update
- PATCH `/form-templates/{id}/name` — Rename
- PATCH `/form-templates/{id}/templateStatus` — Change status

**Error codes:**
- 400: Validation error, InvalidState error
- 401: Unauthorized
- 403: Forbidden
- 404: Not found
- 409: Conflict (e.g., duplicate name)
- 500: Unhandled exception

## Configuration & Environment

**Development (appsettings.Development.json):**
- UseInMemoryDatabase: true (no PostgreSQL required)
- Log level: Debug
- OTEL endpoint: http://localhost:4317

**Production (appsettings.json):**
- PostgreSQL required
- Log level: Information
- CORS: Update from localhost-only to your domain

## Code Style Guidelines

From .github/copilot-instructions.md and backend instructions:
- KISS, YAGNI: Simple, readable, maintainable code
- Primary constructors for DI
- Records for value types and DTOs
- No async suffix on methods
- Result<T> for domain errors (functional style)
- ProblemDetails for API errors
- Self-documenting code; no comments needed if names are clear
