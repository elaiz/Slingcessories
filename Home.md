# Slingcessories Wiki

Welcome to the **Slingcessories** project! This application helps you manage your slingshot collection and associated accessories, tracking inventory, quantities, and wishlists across multiple platforms.

## ?? Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Projects](#projects)
- [Getting Started](#getting-started)
- [API Reference](#api-reference)
- [Features](#features)
- [Development](#development)
- [Database Schema](#database-schema)
- [Testing](#testing)
- [Contributing](#contributing)

---

## Overview

**Slingcessories** is a full-stack application for managing slingshots and their accessories. The project consists of:

- **Blazor WebAssembly** frontend for web browsers
- **.NET MAUI** mobile application for cross-platform mobile devices
- **ASP.NET Core Web API** backend service with SQL Server database
- **GraphQL** API for flexible data queries
- **Comprehensive unit tests** using xUnit and FluentAssertions

### Key Features

? Multi-user support with user registration and selection  
? Slingshot inventory management (Year, Model, Color)  
? Accessory management with categories and subcategories  
? Quantity tracking per slingshot-accessory association  
? Wishlist functionality for desired accessories  
? Price tracking and automatic totals calculation  
? Hierarchical category/subcategory organization  
? Offline support with IndexedDB (Blazor WebAssembly)  
? RESTful API + GraphQL endpoints  
? Cross-platform mobile support (Android, iOS, Windows, macOS)  

---

## Architecture

### Technology Stack

| Component | Technology |
|-----------|-----------|
| **Frontend (Web)** | Blazor WebAssembly, C# 12, .NET 8 |
| **Frontend (Mobile)** | .NET MAUI, .NET 10 |
| **Backend API** | ASP.NET Core Web API, .NET 8 |
| **Database** | SQL Server with Entity Framework Core |
| **GraphQL** | HotChocolate v13 |
| **Testing** | xUnit, FluentAssertions |
| **State Management** | Browser Storage API (IndexedDB) |

### Project Structure

```
Slingcessories/
??? Slingcessories/                    # Blazor WebAssembly Client
?   ??? Components/                    # Reusable Razor components
?   ??? Pages/                         # Page components (Home, Slingshots, Categories, etc.)
?   ??? Services/                      # Client services (UserService, PageStateService)
?   ??? wwwroot/                       # Static files, JS interop
?
??? Slingcessories.Service/            # ASP.NET Core Web API
?   ??? Controllers/                   # REST API controllers
?   ??? GraphQL/                       # GraphQL queries and types
?   ??? Data/                          # DbContext and migrations
?   ??? Models/                        # Domain entities
?   ??? Dtos/                          # Data transfer objects
?
??? Slingcessories.Mobile.Maui/        # .NET MAUI Mobile App
?   ??? Pages/                         # Mobile UI pages
?   ??? Services/                      # API service clients
?   ??? Models/                        # Mobile DTOs
?
??? Slingcessories.Tests/              # Unit Tests
?   ??? Controllers/                   # Controller test suites
?
??? Slingcessories.Data/               # SQL Server Database Project
    ??? dbo/Tables/                    # Table definitions
```

---

## Projects

### 1. Slingcessories (Blazor WebAssembly)

The web client application that runs entirely in the browser.

**Key Files:**
- `Program.cs` - Configures HttpClient to point to API service
- `Pages/Home.razor` - Dashboard with statistics
- `Pages/Slingshots.razor` - Slingshot management with accessories
- `Pages/Categories.razor` - Category and subcategory management
- `Components/AccessoryList.razor` - Accessory grid component
- `Services/UserStateService.cs` - User selection and persistence
- `Services/PageStateService.cs` - UI state management with IndexedDB

**Features:**
- Single Page Application (SPA) architecture
- Offline-capable with IndexedDB storage
- Responsive Bootstrap UI
- Real-time totals calculation
- Collapsible hierarchical views

### 2. Slingcessories.Service (ASP.NET Core API)

The backend API service providing REST and GraphQL endpoints.

**Key Components:**

#### REST API Controllers
- `UsersController` - User registration and management
- `SlingshotsController` - Slingshot CRUD operations (filtered by userId)
- `AccessoriesController` - Accessory management with quantities
- `CategoriesController` - Category and subcategory management
- `SubcategoriesController` - Subcategory operations

#### GraphQL API
- `Query.cs` - GraphQL queries with filtering, sorting, and projections
- Custom types for accessories and slingshots with navigation properties
- Grouped queries (by category, by subcategory)

#### Database
- Entity Framework Core with SQL Server
- Automatic migrations on startup
- AppDbContext with entities: User, Slingshot, Accessory, Category, Subcategory, AccessorySlingshot

### 3. Slingcessories.Mobile.Maui (.NET MAUI)

Cross-platform mobile application for Android, iOS, Windows, and macOS.

**Platforms:**
- Android (net10.0-android)
- iOS (net10.0-ios)
- macOS Catalyst (net10.0-maccatalyst)
- Windows (net10.0-windows10.0.19041.0)

**Key Services:**
- `ApiService.cs` - Centralized API client for all endpoints
- `MauiProgram.cs` - Dependency injection and HTTP client configuration

### 4. Slingcessories.Tests (Unit Tests)

Comprehensive test suite using xUnit and FluentAssertions.

**Test Coverage:**
- All controller endpoints
- CRUD operations
- Validation logic
- Business rules (e.g., cannot delete category with subcategories)
- Error handling

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (for MAUI)
- [SQL Server](https://www.microsoft.com/sql-server/) or [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (recommended) or Visual Studio Code

### Setup Instructions

#### 1. Clone the Repository

```bash
git clone https://github.com/elaiz/Slingcessories.git
cd Slingcessories
```

#### 2. Configure Database Connection

Update the connection string in `Slingcessories.Service/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=Slingcessories;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

**Note:** Replace `YOUR_SERVER` with your SQL Server instance name (e.g., `localhost\\SQLEXPRESS` or `TRUNKS\\SQLEXPRESS`).

#### 3. Run Database Migrations

The API automatically applies migrations on startup. Alternatively, you can run migrations manually:

```bash
cd Slingcessories.Service
dotnet ef database update
```

#### 4. Run the API Service

```bash
cd Slingcessories.Service
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7289`
- HTTP: `http://localhost:5000` (if configured)
- Swagger UI: `https://localhost:7289/swagger`
- GraphQL Playground: `https://localhost:7289/graphql`

#### 5. Run the Blazor WebAssembly Client

Update the API URL in `Slingcessories/Program.cs` if needed (default: `https://localhost:7289/`).

```bash
cd Slingcessories
dotnet run
```

The web app will be available at `https://localhost:7291` (or as configured).

#### 6. Run the MAUI Mobile App

**For Android:**
```bash
cd Slingcessories.Mobile.Maui
dotnet build -t:Run -f net10.0-android
```

**For iOS:**
```bash
dotnet build -t:Run -f net10.0-ios
```

**For Windows:**
```bash
dotnet build -t:Run -f net10.0-windows10.0.19041.0
```

Update the API URL in `Slingcessories.Mobile.Maui/appsettings.json` or `MauiProgram.cs`.

---

## API Reference

### Base URL

```
https://localhost:7289/api
```

### Authentication

Currently, the API uses a simple user selection model without authentication. Users are identified by their `userId` (string/GUID).

### REST Endpoints

#### Users

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users` | Get all users |
| GET | `/api/users/{id}` | Get user by ID |
| POST | `/api/users/register` | Register new user |
| PUT | `/api/users/{id}` | Update user |
| DELETE | `/api/users/{id}` | Delete user |

**Example: Register User**
```http
POST /api/users/register
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com"
}
```

#### Slingshots

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/slingshots?userId={userId}` | Get slingshots for user |
| GET | `/api/slingshots/{id}` | Get slingshot by ID |
| POST | `/api/slingshots` | Create slingshot |
| PUT | `/api/slingshots/{id}` | Update slingshot |
| DELETE | `/api/slingshots/{id}` | Delete slingshot |

**Example: Create Slingshot**
```http
POST /api/slingshots
Content-Type: application/json

{
  "year": 2024,
  "model": "Axiom Ocularis",
  "color": "Black",
  "userId": "user-guid-here"
}
```

#### Accessories

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/accessories?wishlist={bool}` | Get all accessories (optional wishlist filter) |
| GET | `/api/accessories/{id}` | Get accessory by ID |
| POST | `/api/accessories` | Create accessory |
| PUT | `/api/accessories/{id}` | Update accessory |
| DELETE | `/api/accessories/{id}` | Delete accessory |

**Example: Create Accessory**
```http
POST /api/accessories
Content-Type: application/json

{
  "title": "Elite Band",
  "price": 25.00,
  "pictureUrl": "https://example.com/image.jpg",
  "url": "https://store.com/product",
  "wishlist": false,
  "categoryId": 1,
  "subcategoryId": 2,
  "slingshotQuantities": {
    "1": 2,
    "3": 1
  }
}
```

#### Categories

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/categories` | Get all categories |
| GET | `/api/categories/{id}` | Get category by ID |
| POST | `/api/categories` | Create category |
| PUT | `/api/categories/{id}` | Update category |
| DELETE | `/api/categories/{id}` | Delete category |
| GET | `/api/categories/{id}/subcategories` | Get subcategories for category |

#### Subcategories

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/subcategories` | Get all subcategories |
| GET | `/api/subcategories/{id}` | Get subcategory by ID |
| POST | `/api/subcategories` | Create subcategory |
| PUT | `/api/subcategories/{id}` | Update subcategory |
| DELETE | `/api/subcategories/{id}` | Delete subcategory |

### GraphQL Endpoint

**URL:** `https://localhost:7289/graphql`

**Example Query:**
```graphql
query {
  accessories {
    id
    title
    price
    wishlist
    category {
      name
    }
    subcategory {
      name
    }
  }
}
```

**Example Query with Filtering:**
```graphql
query {
  accessories(where: { wishlist: { eq: true } }) {
    id
    title
    price
  }
}
```

**Example Query - Accessories Grouped by Category:**
```graphql
query {
  accessoriesGroupedByCategory {
    categoryId
    categoryName
    count
    accessories {
      id
      title
      price
    }
  }
}
```

---

## Features

### User Management

- **Multi-user Support**: Multiple users can manage their own slingshot collections
- **User Registration**: Simple registration with first name, last name, and email
- **User Selection**: Quick user switching from any page
- **Persistent Selection**: Selected user is saved to browser storage

### Slingshot Management

- **CRUD Operations**: Create, read, update, delete slingshots
- **User Filtering**: Slingshots are automatically filtered by selected user
- **Unique Constraint**: Each Year + Model + Color combination must be unique
- **Accessory Association**: Link accessories to slingshots with quantities
- **Price Totals**: Automatic calculation of owned and wishlist totals
- **Expandable Details**: View associated accessories in collapsible sections

### Accessory Management

- **Categorization**: Organize accessories by category and optional subcategory
- **Quantity Tracking**: Track quantity of each accessory per slingshot
- **Wishlist Support**: Mark accessories as wishlist items
- **Price Management**: Store and track accessory prices
- **Product Links**: Store URLs and images for each accessory
- **Bulk Operations**: Add multiple accessories to a slingshot at once
- **Hierarchical Display**: View accessories grouped by category and subcategory

### Category Management

- **Hierarchical Structure**: Categories can have subcategories
- **Flexible Organization**: Accessories can belong to category only or category + subcategory
- **Protected Deletion**: Cannot delete categories with subcategories or accessories
- **Easy Reorganization**: Drag-and-drop support (UI feature)

### UI/UX Features

- **Responsive Design**: Bootstrap-based responsive layout
- **Collapsible Sections**: Expand/collapse categories and subcategories
- **Inline Editing**: Edit items without navigating away
- **Confirmation Dialogs**: Confirm before deleting items
- **Error Handling**: User-friendly error messages
- **Loading States**: Visual feedback during API calls
- **State Persistence**: UI state (expanded/collapsed) saved to IndexedDB
- **Icons**: Bootstrap Icons for visual clarity

---

## Development

### Project Configuration

#### Blazor WebAssembly

Edit `Slingcessories/Program.cs` to configure the API endpoint:

```csharp
builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7289/")
});
```

#### API Service

Edit `Slingcessories.Service/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=Slingcessories;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

#### MAUI Mobile

Edit `Slingcessories.Mobile.Maui/appsettings.json`:

```json
{
  "ApiBaseUrl": "https://your-api-url.com/"
}
```

For local development on Android emulator, use `http://10.0.2.2:5000/` to access the host machine's localhost.

### Running Migrations

**Create a new migration:**
```bash
cd Slingcessories.Service
dotnet ef migrations add MigrationName
```

**Apply migrations:**
```bash
dotnet ef database update
```

**Remove last migration:**
```bash
dotnet ef migrations remove
```

### Building for Production

**Blazor WebAssembly:**
```bash
cd Slingcessories
dotnet publish -c Release
```

Output will be in `bin/Release/net8.0/publish/wwwroot/`.

**API Service:**
```bash
cd Slingcessories.Service
dotnet publish -c Release
```

**MAUI (Android APK):**
```bash
cd Slingcessories.Mobile.Maui
dotnet publish -f net10.0-android -c Release
```

---

## Database Schema

### Tables

#### Users
```sql
CREATE TABLE [Users] (
    [Id] NVARCHAR(450) PRIMARY KEY,
    [FirstName] NVARCHAR(450) NOT NULL,
    [LastName] NVARCHAR(450) NOT NULL,
    [Email] NVARCHAR(450) NOT NULL UNIQUE
);
```

#### Slingshots
```sql
CREATE TABLE [Slingshots] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Year] INT NOT NULL,
    [Model] NVARCHAR(450) NOT NULL,
    [Color] NVARCHAR(450) NOT NULL,
    [UserId] NVARCHAR(450) NOT NULL,
    CONSTRAINT [FK_Slingshots_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]),
    UNIQUE([Year], [Model], [Color])
);
```

#### Categories
```sql
CREATE TABLE [Categories] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(450) NOT NULL UNIQUE,
    [UserId] NVARCHAR(450) NOT NULL
);
```

#### Subcategories
```sql
CREATE TABLE [Subcategories] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(450) NOT NULL,
    [CategoryId] INT NOT NULL,
    CONSTRAINT [FK_Subcategories_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [Categories]([Id])
);
```

#### Accessories
```sql
CREATE TABLE [Accessories] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Title] NVARCHAR(450) NOT NULL,
    [PictureUrl] NVARCHAR(MAX),
    [Price] DECIMAL(18,2) NOT NULL,
    [Url] NVARCHAR(MAX),
    [Wishlist] BIT NOT NULL,
    [CategoryId] INT NOT NULL,
    [SubcategoryId] INT,
    CONSTRAINT [FK_Accessories_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [Categories]([Id]),
    CONSTRAINT [FK_Accessories_Subcategories] FOREIGN KEY ([SubcategoryId]) REFERENCES [Subcategories]([Id])
);
```

#### AccessorySlingshots (Many-to-Many with Quantity)
```sql
CREATE TABLE [AccessorySlingshots] (
    [AccessoryId] INT NOT NULL,
    [SlingshotId] INT NOT NULL,
    [Quantity] INT NOT NULL DEFAULT 1,
    PRIMARY KEY ([AccessoryId], [SlingshotId]),
    CONSTRAINT [FK_AccessorySlingshots_Accessories] FOREIGN KEY ([AccessoryId]) REFERENCES [Accessories]([Id]),
    CONSTRAINT [FK_AccessorySlingshots_Slingshots] FOREIGN KEY ([SlingshotId]) REFERENCES [Slingshots]([Id])
);
```

### Entity Relationships

```
Users 1:N Slingshots
Categories 1:N Subcategories
Categories 1:N Accessories
Subcategories 1:N Accessories (optional)
Accessories N:M Slingshots (via AccessorySlingshots with Quantity)
```

---

## Testing

### Running Tests

**Run all tests:**
```bash
cd Slingcessories.Tests
dotnet test
```

**Run with coverage:**
```bash
dotnet test --collect:"XPlat Code Coverage"
```

**Run specific test:**
```bash
dotnet test --filter "FullyQualifiedName~UsersControllerTests.GetAll_ReturnsEmptyList_WhenNoUsers"
```

### Test Structure

All tests use in-memory SQLite database for isolation. Each test:
1. Creates a fresh in-memory database
2. Seeds test data if needed
3. Executes the operation
4. Asserts expected results
5. Cleans up automatically

**Example Test:**
```csharp
[Fact]
public async Task Create_CreatesCategory_WhenValidData()
{
    // Arrange
    await using var db = TestHelpers.CreateInMemoryDbContext();
    var controller = new CategoriesController(db);
    var createDto = new CreateCategoryDto("NewCategory");

    // Act
    var result = await controller.Create(createDto);

    // Assert
    var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
    var dto = createdResult.Value.Should().BeOfType<CategoryDto>().Subject;
    dto.Name.Should().Be("NewCategory");
}
```

---

## Contributing

We welcome contributions! Please follow these guidelines:

### Branching Strategy

- `master` - Main stable branch
- `feature/*` - Feature branches
- `bugfix/*` - Bug fix branches

### Pull Request Process

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes
4. Add or update tests as needed
5. Ensure all tests pass (`dotnet test`)
6. Commit your changes (`git commit -m 'Add amazing feature'`)
7. Push to the branch (`git push origin feature/amazing-feature`)
8. Open a Pull Request

### Code Style

- Follow C# coding conventions
- Use meaningful variable and method names
- Add XML documentation comments for public APIs
- Keep methods small and focused
- Write unit tests for new features

### Commit Messages

Use clear, descriptive commit messages:
- `feat: Add wishlist filtering to accessories page`
- `fix: Correct quantity calculation in totals`
- `docs: Update API reference with new endpoints`
- `test: Add tests for slingshot deletion logic`

---

## License

This project is licensed under the MIT License - see the LICENSE file for details.

---

## Support

For questions, issues, or feature requests, please [open an issue](https://github.com/elaiz/Slingcessories/issues) on GitHub.

---

## Acknowledgments

- Built with [Blazor WebAssembly](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
- Powered by [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
- Mobile support via [.NET MAUI](https://dotnet.microsoft.com/apps/maui)
- GraphQL via [HotChocolate](https://chillicream.com/docs/hotchocolate)
- Testing with [xUnit](https://xunit.net/) and [FluentAssertions](https://fluentassertions.com/)

---

**Happy Slingshooting! ??**
