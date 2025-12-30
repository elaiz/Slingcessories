using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slingcessories.Service.Controllers;
using Slingcessories.Service.Dtos;
using Slingcessories.Service.Models;

namespace Slingcessories.Tests.Controllers;

public class SubcategoriesControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoSubcategories()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new SubcategoriesController(db);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var subcategories = okResult.Value.Should().BeAssignableTo<IEnumerable<SubcategoryDto>>().Subject;
        subcategories.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_ReturnsSubcategories_OrderedByName()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        db.Subcategories.AddRange(
            new Subcategory { Name = "Zebra", CategoryId = category.Id },
            new Subcategory { Name = "Apple", CategoryId = category.Id },
            new Subcategory { Name = "Banana", CategoryId = category.Id }
        );
        await db.SaveChangesAsync();

        var controller = new SubcategoriesController(db);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var subcategories = okResult.Value.Should().BeAssignableTo<IEnumerable<SubcategoryDto>>().Subject.ToList();
        subcategories.Should().HaveCount(3);
        subcategories[0].Name.Should().Be("Apple");
        subcategories[1].Name.Should().Be("Banana");
        subcategories[2].Name.Should().Be("Zebra");
    }

    [Fact]
    public async Task GetByCategory_ReturnsSubcategories_ForSpecificCategory()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category1 = new Category { Name = "Category1" };
        var category2 = new Category { Name = "Category2" };
        db.Categories.AddRange(category1, category2);
        await db.SaveChangesAsync();

        db.Subcategories.AddRange(
            new Subcategory { Name = "Sub1", CategoryId = category1.Id },
            new Subcategory { Name = "Sub2", CategoryId = category2.Id },
            new Subcategory { Name = "Sub3", CategoryId = category1.Id }
        );
        await db.SaveChangesAsync();

        var controller = new SubcategoriesController(db);

        // Act
        var result = await controller.GetByCategory(category1.Id);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var subcategories = okResult.Value.Should().BeAssignableTo<IEnumerable<SubcategoryDto>>().Subject.ToList();
        subcategories.Should().HaveCount(2);
        subcategories.Should().OnlyContain(s => s.CategoryId == category1.Id);
    }

    [Fact]
    public async Task GetByCategory_ReturnsEmptyList_WhenNoneForCategory()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var controller = new SubcategoriesController(db);

        // Act
        var result = await controller.GetByCategory(category.Id);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var subcategories = okResult.Value.Should().BeAssignableTo<IEnumerable<SubcategoryDto>>().Subject;
        subcategories.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByCategory_OrdersByName()
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

        var controller = new SubcategoriesController(db);

        // Act
        var result = await controller.GetByCategory(category.Id);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var subcategories = okResult.Value.Should().BeAssignableTo<IEnumerable<SubcategoryDto>>().Subject.ToList();
        subcategories[0].Name.Should().Be("Apple");
        subcategories[1].Name.Should().Be("Zebra");
    }

    [Fact]
    public async Task Create_CreatesSubcategory_WhenValidData()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var controller = new SubcategoriesController(db);
        var createDto = new CreateSubcategoryDto("NewSubcategory", category.Id);

        // Act
        var result = await controller.Create(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var dto = createdResult.Value.Should().BeOfType<SubcategoryDto>().Subject;
        dto.Name.Should().Be("NewSubcategory");
        dto.CategoryId.Should().Be(category.Id);

        // Verify saved to database
        var saved = await db.Subcategories.FindAsync(dto.Id);
        saved.Should().NotBeNull();
    }

    [Fact]
    public async Task Update_UpdatesSubcategory_WhenValidData()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category1 = new Category { Name = "Category1" };
        var category2 = new Category { Name = "Category2" };
        db.Categories.AddRange(category1, category2);
        await db.SaveChangesAsync();

        var subcategory = new Subcategory { Name = "OldName", CategoryId = category1.Id };
        db.Subcategories.Add(subcategory);
        await db.SaveChangesAsync();

        var controller = new SubcategoriesController(db);
        var updateDto = new SubcategoryDto(subcategory.Id, "NewName", category2.Id);

        // Act
        var result = await controller.Update(subcategory.Id, updateDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        // Verify update
        var updated = await db.Subcategories.FindAsync(subcategory.Id);
        updated!.Name.Should().Be("NewName");
        updated.CategoryId.Should().Be(category2.Id);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenIdMismatch()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new SubcategoriesController(db);
        var updateDto = new SubcategoryDto(1, "NewName", 1);

        // Act
        var result = await controller.Update(2, updateDto);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("ID mismatch");
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenSubcategoryDoesNotExist()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new SubcategoriesController(db);
        var updateDto = new SubcategoryDto(999, "NewName", 1);

        // Act
        var result = await controller.Update(999, updateDto);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_DeletesSubcategory_WhenSubcategoryExists()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var category = new Category { Name = "TestCategory" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var subcategory = new Subcategory { Name = "TestSubcategory", CategoryId = category.Id };
        db.Subcategories.Add(subcategory);
        await db.SaveChangesAsync();

        var controller = new SubcategoriesController(db);

        // Act
        var result = await controller.Delete(subcategory.Id);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        // Verify deletion
        var deleted = await db.Subcategories.FindAsync(subcategory.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenSubcategoryDoesNotExist()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new SubcategoriesController(db);

        // Act
        var result = await controller.Delete(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
