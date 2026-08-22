```markdown
# TicketFlow Knowledge Digest

## Solution / Project Map
The repository is structured around the `Slingcessories` solution and its related projects. Key project folders include:

- **Core Solution:**  
  - `Slingcessories.sln` — Top-level solution file containing all sub-projects.
- **Backend Service:**  
  - `Slingcessories.Service/` — Backend services (API).
  - `Slingcessories.Service/Slingcessories.Service.csproj` — Service project definition.
- **Frontend:**  
  - `Slingcessories.Web.React/` — React web application (`package.json` for dependencies).
  - `Slingcessories.Mobile.Maui/` — .NET MAUI mobile project.
  - `slingcessories_mobile_flutter/` — Flutter-based mobile implementation.
- **Data Access:**  
  - `Slingcessories.Data/` — Data layer, including models and repository patterns.
- **Tests:**  
  - `Slingcessories.Tests/` — General test suite.
  - `Slingcessories.Mcp.Tests/` — Tests for the MCP layer.
- **Scripts & SQL:**  
  - `scripts/` — Deployment and maintenance scripts.
  - Various `.sql` files for database migrations (`AddTestData.sql`, `CheckDatabase.sql`, etc.).

## Key Domains and Locations

### Domain: UI / Frontend
- **React Web App:** `Slingcessories.Web.React/`  
  - Components live under `src/Components/`.  
- **Mobile Apps:**  
  - MAUI: `Slingcessories.Mobile.Maui/` — Cross-platform .NET-based project.
  - Flutter: `slingcessories_mobile_flutter/` — Alternative mobile implementation.

### Domain: API / Backend
- **Primary Service:** `Slingcessories.Service/Controllers/` — API layer with REST endpoints.  
- **Business Logic:** Implemented in `Services/`.

### Domain: Data Layer
- **Models:** `Slingcessories.Data/Entities/`.  
- **Data Context:** `Slingcessories.Data/DbContext/`.  
- **Repositories:** Handle database operations in `Slingcessories.Data/Repositories/`.  
- **Migration Files:** Top-level `.sql` files for specific database operations.

### Domain: Test Coverage
- **Automated Tests:**  
  - `Slingcessories.Tests/` — Main unit test library.  
  - `Slingcessories.Mcp.Tests/` — Specialized MCP-related tests.

## Conventions Worth Knowing
- **File Naming:** Follows standard domain-driven design (DDD) conventions: `Entities` for models, `Repositories` for data access, `Controllers` for APIs, and `Services` for business logic.
- **Branching and CI/CD:** Guidelines in `CI_CD_SETUP.md` and related files.  
- **Testing:** Structure tests by feature or layer. Refactoring notes in `TEST_ORGANIZATION_REFACTORING.md`.  
- **Offline Mode:** Reference implementation details in `OFFLINE_MODE_IMPLEMENTATION.md`.

## Where to Look First
1. **UI:**  
   - React: Check `Slingcessories.Web.React/src/Components/` and `Pages/` for React-based components and routing.
   - MAUI: Explore XAML views and code-behind files in `Slingcessories.Mobile.Maui/`.
2. **API:**  
   - `Slingcessories.Service/Controllers/` for API endpoints.  
   - `Services/` for business logic.
3. **Data Access:**  
   - Database models in `Slingcessories.Data/Entities/`.  
   - Interact with the database in `Repositories/`.
4. **Settings / Deployment:**  
   - Scripts live in `scripts/`.  
   - CI workflows and fixes are detailed in the various `CI_CD_*` markdown files.
```