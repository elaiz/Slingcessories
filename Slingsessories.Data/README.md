# Slingsessories.Data - SQL Server Database Project

This is a SQL Server Database Project (SSDT) for the Slingsessory database.

## Database Structure

### Tables

1. **Categories**
   - `Id` (INT, Identity, PK)
   - `Name` (NVARCHAR(450), Unique)

2. **Subcategories**
   - `Id` (INT, Identity, PK)
   - `Name` (NVARCHAR(450))
   - `CategoryId` (INT, FK to Categories)
   - Unique index on (CategoryId, Name)
   - Cascading delete from Categories

3. **Accessories**
   - `Id` (INT, Identity, PK)
   - `Title` (NVARCHAR(MAX))
   - `PictureUrl` (NVARCHAR(MAX), nullable)
   - `Units` (INT)
   - `Price` (DECIMAL(18,2))
   - `Url` (NVARCHAR(MAX), nullable)
   - `Wishlist` (BIT, default 0)
   - `CategoryId` (INT, FK to Categories)
   - `SubcategoryId` (INT, nullable, FK to Subcategories)
   - Foreign key to Subcategories with SET NULL on delete

## Files Created

- `dbo/Tables/Categories.sql` - Categories table definition
- `dbo/Tables/Subcategories.sql` - Subcategories table with FK to Categories
- `dbo/Tables/Accessories.sql` - Accessories table with FKs
- `Scripts/PostDeployment/Script.PostDeployment.sql` - Post-deployment script for seed data

## Connection String

From `appsettings.json`:
```
Server=TRUNKS\SQLEXPRESS;Database=Slingsessory;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True
```

## To Update the .sqlproj File

Open `Slingsessories.Data.sqlproj` in Visual Studio and add the following items:

```xml
  <ItemGroup>
    <Folder Include="dbo\" />
    <Folder Include="dbo\Tables\" />
    <Folder Include="Scripts\" />
    <Folder Include="Scripts\PostDeployment\" />
  </ItemGroup>
  <ItemGroup>
    <Build Include="dbo\Tables\Categories.sql" />
    <Build Include="dbo\Tables\Subcategories.sql" />
    <Build Include="dbo\Tables\Accessories.sql" />
  </ItemGroup>
  <ItemGroup>
    <PostDeploy Include="Scripts\PostDeployment\Script.PostDeployment.sql" />
  </ItemGroup>
```

Place these ItemGroups before the closing `</Project>` tag.

## Deployment

This project can be built and deployed using:
- Visual Studio (SQL Server Data Tools)
- MSBuild
- SqlPackage.exe

## Notes

- The database schema matches the Entity Framework Core migrations in the `Slingsessory.service` project
- All foreign key relationships and indexes are preserved from the existing database
- Post-deployment scripts can be customized for seed data
