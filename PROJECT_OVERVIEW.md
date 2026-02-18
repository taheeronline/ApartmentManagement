ApartmentManagement — Project Overview

Summary
-------
A small Blazor-based apartment management sample composed of four projects that follow a layered architecture:
- `ApartmentManagement.Domain` — Domain entities and enums.
- `ApartmentManagement.Application` — Application services, DTOs and repository interfaces (use-cases and business logic).
- `ApartmentManagement.Infrastructure` — EF Core `DbContext`, repository implementations, entity configuration and migrations (persistence).
- `ApartmentManagement.UI` — Blazor interactive server UI (Razor components) and app startup.

This document describes the code structure, key concepts used, implemented functionality and notes for future projects.

Solution structure (what each project contains)
----------------------------------------------
- Domain
  - `Entities`: `Apartment`, `Flat`, `Resident` — encapsulated domain models with behavior (e.g., `Resident.MoveOut()`).
  - `Enums`: `ResidentType` and other domain-level enums.

- Application
  - `Services`: `ApartmentService`, `FlatService`, `ResidentService`. They implement use-cases, validate inputs, map domain entities to DTOs, and call repository interfaces.
  - `Interfaces`: repository contracts `iApartmentRepository`, `iFlatRepository`, `iResidentRepository` to decouple persistence.
  - `DTOs`: `ApartmentDto`, `FlatDto`, `ResidentDto` for data transfer to UI.
  - `Validation`: `ValidationConstants` defines `MaxNameLength = 70` used by application-level validation.

- Infrastructure
  - `Persistence`: `ApartmentDbContext` (EF Core), configurations under `Persistence/Configuration` with `IEntityTypeConfiguration<T>` implementations.
  - `Repositories`: concrete EF Core implementations of repository interfaces (`ApartmentRepository`, `FlatRepository`, `ResidentRepository`).
  - `Migrations`: EF migrations are present for schema changes.

- UI
  - Blazor components (Razor) under `Components/Pages` for pages: `Apartments`, `Flats`, `Residents`, `AddResident`, `QuickGridDemo`, etc.
  - `Program.cs` bootstraps services, DbContext and middleware.
  - `Middleware`: `GlobalExceptionHandlerMiddleware` (global exception handling + mapping exceptions to HTTP status and user-friendly messages).

Code logic and patterns used
---------------------------
- Layered (clean) architecture
  - Domain holds business entities and core behavior.
  - Application implements orchestration/use-cases and does NOT reference EF Core — it depends on repository interfaces.
  - Infrastructure provides EF Core implementations and wiring.
  - UI composes services and displays DTOs.

- Repository pattern
  - Repositories provide CRUD and query methods. Application calls repository interfaces so persistence details are isolated.

- DTO mapping
  - Services map domain entities to DTOs using LINQ `Select` (manual mapping). DTOs are small and designed for the UI.

- Validation
  - Services validate inputs (e.g., required fields, id > 0) and throw `ArgumentException` or `InvalidOperationException` for domain/validation errors.
  - A global constant `ValidationConstants.MaxNameLength = 70` is used for name length validation.
  - UI pages catch `ArgumentException` and `InvalidOperationException` to show inline friendly messages. Unexpected exceptions bubble to middleware.

- Error handling
  - `GlobalExceptionHandlerMiddleware` centrally intercepts unhandled exceptions and:
    - Maps `ArgumentException` -> 400 Bad Request (warning), `InvalidOperationException` -> 409 Conflict (warning), others -> 500 Internal Server Error (error).
    - Logs the exception with an `ErrorId` (GUID).
    - For API/AJAX returns JSON { Message, ErrorId } and status code.
    - For normal requests redirects to `/Error?errorId=...&message=...`.

- Logging
  - `ILogger<T>` injected in services and repositories.
  - Important operations (Add/Update/Delete) log informational messages; exceptional conditions log warnings or errors.
  - Global middleware logs unhandled exceptions with ErrorId for correlation.

- EF Core
  - `ApartmentDbContext` with DbSets for `Apartment`, `Flat`, `Resident`.
  - Entity configurations applied with `modelBuilder.ApplyConfigurationsFromAssembly(...)`.
  - Repositories use `AsNoTracking()` for read queries and include relationships where required.

UI (Blazor) and component behavior
----------------------------------
- Pages are interactive server components (AddResident, Apartments, Flats, Residents).
- Services are injected into components, which call service methods and display results (DTOs).
- Inline user-friendly error messages are displayed when services throw validation/domain exceptions. Unexpected errors are handled by the global middleware.
- Grids (QuickGrid) have been adjusted so the Name column is fixed in width, showing ellipsis for overflow and the name input enforces `maxlength=70`.

Implemented functionality (features available now)
-------------------------------------------------
- Apartment management:
  - List, add, update, delete apartments via `ApartmentService` and UI pages.
  - Apartment quick grid with inline add/edit and delete and UI feedback (`successMessage`/`errorMessage`).

- Flat management:
  - List flats by apartment, add flat (with duplicate-check), delete flat.
  - `FlatService` enforces validation; UI shows friendly messages.

- Resident management:
  - Add resident (validates flat exists, occupancy limit), list residents, move-out, delete.
  - Move-in sets MoveInDate in domain `Resident` constructor; MoveOut sets MoveOutDate.
  - `ResidentService.GetAllAsync()` maps domain entities to `ResidentDto` including flat number/floor.

- Persistence and schema:
  - EF Core migrations exist and the schema is configured via entity configurations.
  - Note: EF configurations currently use `HasMaxLength(200)` for apartment name and `HasMaxLength(150)` for resident full name in the database configurations. Application-level validation uses 70 chars for names. These should be aligned (see notes below).

How data flows for a typical operation (Add Resident)
---------------------------------------------------
1) UI component `AddResident.razor` calls `ResidentService.AddResidentAsync(...)`.
2) `ResidentService` validates inputs, checks flat existence with `_flatRepository.GetByIdAsync`, checks active occupant count with `_residentRepository.GetActiveResidentCountByFlatAsync`.
3) If valid, service creates domain `Resident` and calls `_residentRepository.AddAsync(resident)`.
4) Repository adds entity via `ApartmentDbContext` and saves changes.
5) Service returns and UI refreshes by calling `GetAllAsync()` which maps to `ResidentDto`.

Notes, recommendations and next steps
------------------------------------
- Align DB schema with application constraints: set `HasMaxLength(70)` for apartment and resident name fields in EF Core configurations to avoid mismatches; add an EF migration. If your DB currently contains longer values, handle data cleanup before applying a restrictive migration.
- Prefer explicit validation failures over silent truncation. Currently the app throws `ArgumentException` when a name exceeds 70 characters; UI shows friendly messages. This is good for data integrity.
- Consider centralizing DTO <-> entity mapping with a library like AutoMapper if mappings grow.
- Add unit tests for services (validation, happy paths, repository interactions) and integration tests for middleware behavior.
- Add a small reusable component for alerts (success/error) used across pages to avoid duplicated markup.
- Add API endpoints or a REST surface if you want programmatic access (currently the middleware supports JSON responses for AJAX/API calls).

How to run locally (quick)
--------------------------
1) Configure the connection string in `ApartmentManagement.UI/appsettings.json`.
2) Apply EF migrations to create/update the database (from `Infrastructure` project):
   - `dotnet ef database update --project ApartmentManagement.Infrastructure --startup-project ApartmentManagement.UI`
3) Run the Blazor app (UI): `dotnet run --project ApartmentManagement.UI` and open the browser to the mapped Blazor routes.

Files to inspect for further development
---------------------------------------
- `ApartmentManagement.Domain/Entities/*.cs` — domain behavior
- `ApartmentManagement.Application/Services/*.cs` — use-case logic
- `ApartmentManagement.Application/DTOs/*.cs` — DTO structures
- `ApartmentManagement.Infrastructure/Persistence/*.cs` and `Persistence/Configuration` — EF Core setup
- `ApartmentManagement.Infrastructure/Repositories/*.cs` — DB access implementations
- `ApartmentManagement.UI/Components/Pages/*.razor` — Blazor pages
- `ApartmentManagement.UI/Middleware/GlobalExceptionHandlerMiddleware.cs` — centralized exception handling

Document created on: {DATE}

(End of overview)
