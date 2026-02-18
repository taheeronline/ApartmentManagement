Project Steps — Practical Cheatsheet and Guidelines

This is a portable, non-requirement-specific step-by-step guide for creating a clean, maintainable .NET project (Blazor/web + layered architecture). Use it as a checklist when you start new projects or get stuck.

1) Clarify scope and goals (first)
- Write a short summary: what problem you solve and who the users are.
- List core features (MVP) and nice-to-have features.
- Define non-functional requirements: security, performance, scalability, supported platforms.

2) Choose a high-level architecture
- Prefer a layered or clean architecture: `Domain`, `Application`, `Infrastructure`, `UI`.
- Decide whether you'll implement commands/query separation, eventing, or simple services.

3) Create repository and solution skeleton
- Create a Git repo and branch strategy (e.g., `main`/`master`, feature branches).
- Create projects reflecting the layers: `Project.Domain`, `Project.Application`, `Project.Infrastructure`, `Project.UI`.
- Add a `README.md` and a `PROJECT_OVERVIEW.md` or `PROJECT_STEPS_GUIDELINE.md` for documentation.

4) Design domain model
- Model core entities first with behavior (encapsulation): constructors for invariants, domain methods (no persistence concerns).
- Keep setters private where behavior is required.
- Add enums and value objects if needed.

5) Define application layer (use-cases)
- Create service classes representing use-cases (e.g., `ResidentService`, `ApartmentService`).
- Define repository interfaces in `Application.Interfaces` so `Application` does not depend on EF Core.
- Implement validation in services (throw `ArgumentException` or domain-specific exceptions).
- Return DTOs (not entities) to the UI to avoid leaking domain internals.

6) Create DTOs and mapping strategy
- Keep DTOs small and focused for UI needs: `ApartmentDto`, `ResidentDto`.
- For small apps, manual mapping (`Select`/`new DTO`) is fine. For larger mapping surface consider `AutoMapper`.
- Centralize common validation constants in one place (e.g., `ValidationConstants`).

7) Implement Infrastructure (persistence)
- Add `DbContext` (e.g., `ApartmentDbContext`) and entity configurations using `IEntityTypeConfiguration<T>`.
- Implement repository classes that fulfill `Application.Interfaces` contracts using EF Core.
- Use `AsNoTracking()` for read-only queries and `Include(...)` where necessary.
- Log intentions (Add/Update/Delete) at repository level when appropriate.

8) Configure UI (Blazor or API)
- For Blazor: use interactive server components or WASM per your needs. Keep UI focused on rendering DTOs.
- Inject `Application` services in pages: `@inject ResidentService ResidentService`.
- Catch validation/domain exceptions in pages to show friendly messages; let unexpected exceptions bubble to global middleware.
- Keep components small and composable.

9) Cross-cutting concerns
- Logging: inject `ILogger<T>` in services/repositories. Log Info for main operations and Warning/Error for unexpected conditions.
- Error handling: implement a global exception middleware to centralize logging and consistent responses.
  - Map exception types to status codes (e.g., `ArgumentException` -> 400).
  - Return JSON for API/AJAX and render a friendly Error page for browser requests.
- Validation: enforce both client-side (`maxlength`, simple checks) and server-side (services throw validation exceptions). Prefer explicit errors over silent truncation.

10) Database migrations and schema
- Configure entity `HasMaxLength(...)`, relationships and constraints in configuration classes.
- Create EF Core migrations and review SQL changes before applying to production.
- When tightening constraints (shortening column length) handle existing data (cleanup/truncate or migration script).

11) Tests
- Add unit tests for services (mock repositories). Test success, validation failures, and error flows.
- Add integration tests for persistence using an in-memory or test database.
- Add UI tests or component tests if supported.

12) Documentation and developer ergonomics
- Keep `PROJECT_OVERVIEW.md` with architecture, how data flows, and key files to inspect.
- Add `CONTRIBUTING.md` with local setup steps and commands (how to run migrations, how to run the app).

13) CI / CD and release
- Add CI lint/build/test steps. Run tests and build on PRs.
- Automate migrations and deployment in a controlled manner; use feature flags if needed.

14) Finalize and polish
- Review logging and remove noisy try/catch blocks that only rethrow.
- Ensure DTO and domain validations align with DB constraints.
- Create a reusable alert/toast component for consistent UI messages.
- Run static analyzers and fix warnings.

Quick debugging checklist
- If UI shows empty data: check service method, repository query, `DbContext` registration and connection string.
- If exceptions are swallowed: remove unnecessary try/catch; rely on middleware to log and expose ErrorId.
- If DB constraint error occurs: verify `HasMaxLength` in entity configuration and existing DB data length.

Common pitfalls and tips
- Don't put EF Core types (e.g., `DbContext`) into `Application` layer — use interfaces and DI to decouple.
- Prefer immutable domain methods for behavior (e.g., `Resident.MoveOut()`), not procedural updates scattered across services.
- Validate at service boundaries — keep domain model robust and the UI thin.

Useful commands (examples)
- Add migration:
  - `dotnet ef migrations add InitialCreate --project Project.Infrastructure --startup-project Project.UI`
- Apply migrations:
  - `dotnet ef database update --project Project.Infrastructure --startup-project Project.UI`
- Run the UI project:
  - `dotnet run --project Project.UI`

Keep this file in your repo as a quick reference and update it as you learn better patterns.

