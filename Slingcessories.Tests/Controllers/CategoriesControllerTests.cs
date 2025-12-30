using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slingcessories.Service.Controllers;
using Slingcessories.Service.Dtos;
using Slingcessories.Service.Models;
using static Slingcessories.Service.Controllers.CategoriesController;

namespace Slingcessories.Tests.Controllers;

public class CategoriesControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoCategories()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new CategoriesController(db);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var categories = okResult.Value.Should().BeAssignableTo<IEnumerable<CategoriesController.CategoryDto>>().Subject;
        categories.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_ReturnsCategories_OrderedByName()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        db.Categories.AddRange(
            new Category { Name = "Zebra" },
            new Category { Name = "Apple" },
            new Category { Name = "Banana" }
        );
        await db.SaveChangesAsync();

        var controller = new CategoriesController(db);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var categories = okResult.Value.Should().BeAssignableTo<IEnumerable<CategoriesController.CategoryDto>>().Subject.ToList();
        categories.Should().HaveCount(3);
        categories[0].Name.Should().Be("Apple");
        categories[1].Name.Should().Be("Banana");
        categories[2].Name.Should().Be("Zebra");
    }

    [Fact]
    public async Task GetById_ReturnsCategory_WhenCategoryExists()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var controller = new CategoriesController(db);

        // Act
        var result = await controller.GetById(category.Id);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var dto = okResult.Value.Should().BeOfType<CategoriesController.CategoryDto>().Subject;
        dto.Id.Should().Be(category.Id);
        dto.Name.Should().Be("TestCategory");
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenCategoryDoesNotExist()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new CategoriesController(db);

        // Act
        var result = await controller.GetById(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

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
        var dto = createdResult.Value.Should().BeOfType<CategoriesController.CategoryDto>().Subject;
        dto.Name.Should().Be("NewCategory");

        // Verify saved to database
        var saved = await db.Categories.FindAsync(dto.Id);
        saved.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenNameIsEmpty()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new CategoriesController(db);
        var createDto = new CreateCategoryDto("");

        // Act
        var result = await controller.Create(createDto);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("Category name is required.");
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenNameIsWhitespace()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new CategoriesController(db);
        var createDto = new CreateCategoryDto("   ");

        // Act
        var result = await controller.Create(createDto);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("Category name is required.");
    }

    [Fact]
    public async Task Update_UpdatesCategory_WhenValidData()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "OldName" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var controller = new CategoriesController(db);
        var updateDto = new CategoriesController.CategoryDto(category.Id, "NewName");

        // Act
        var result = await controller.Update(category.Id, updateDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        // Verify update
        var updated = await db.Categories.FindAsync(category.Id);
        updated!.Name.Should().Be("NewName");
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenIdMismatch()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new CategoriesController(db);
        var updateDto = new CategoriesController.CategoryDto(1, "NewName");

        // Act
        var result = await controller.Update(2, updateDto);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("ID mismatch.");
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenCategoryDoesNotExist()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new CategoriesController(db);
        var updateDto = new CategoriesController.CategoryDto(999, "NewName");

        // Act
        var result = await controller.Update(999, updateDto);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenNameIsEmpty()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "OldName" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var controller = new CategoriesController(db);
        var updateDto = new CategoriesController.CategoryDto(category.Id, "");

        // Act
        var result = await controller.Update(category.Id, updateDto);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("Category name is required.");
    }

    [Fact]
    public async Task Delete_DeletesCategory_WhenCategoryExists()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var controller = new CategoriesController(db);

        // Act
        var result = await controller.Delete(category.Id);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        // Verify deletion
        var deleted = await db.Categories.FindAsync(category.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenCategoryDoesNotExist()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new CategoriesController(db);

        // Act
        var result = await controller.Delete(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenCategoryHasSubcategories()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var subcategory = new Subcategory { Name = "TestSubcategory", CategoryId = category.Id };
        db.Subcategories.Add(subcategory);
        await db.SaveChangesAsync();

        var controller = new CategoriesController(db);

        // Act
        var result = await controller.Delete(category.Id);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("Cannot delete category with existing subcategories.");
    }

    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenCategoryHasAccessories()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var accessory = new Accessory { Title = "TestAccessory", Price = 10, CategoryId = category.Id, Wishlist = false };
        db.Accessories.Add(accessory);
        await db.SaveChangesAsync();

        var controller = new CategoriesController(db);

        // Act
        var result = await controller.Delete(category.Id);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("Cannot delete category with existing accessories.");
    }

    [Fact]
    public async Task GetSubcategories_ReturnsSubcategories_WhenCategoryExists()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        db.Subcategories.AddRange(
            new Subcategory { Name = "Zebra", CategoryId = category.Id },
            new Subcategory { Name = "Apple", CategoryId = category.Id }
        );
        await db.SaveChangesAsync();

        var controller = new CategoriesController(db);

        // Act
        var result = await controller.GetSubcategories(category.Id);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var subcategories = okResult.Value.Should().BeAssignableTo<IEnumerable<SubcategoryDto>>().Subject.ToList();
        subcategories.Should().HaveCount(2);
        subcategories[0].Name.Should().Be("Apple");
        subcategories[1].Name.Should().Be("Zebra");
    }

    [Fact]
    public async Task GetSubcategories_ReturnsNotFound_WhenCategoryDoesNotExist()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new CategoriesController(db);

        // Act
        var result = await controller.GetSubcategories(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetSubcategories_ReturnsEmptyList_WhenNoSubcategories()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var controller = new CategoriesController(db);

        // Act
        var result = await controller.GetSubcategories(category.Id);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var subcategories = okResult.Value.Should().BeAssignableTo<IEnumerable<SubcategoryDto>>().Subject;
        subcategories.Should().BeEmpty();
    }
}
