using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slingcessories.Service.Controllers;
using Slingcessories.Service.Dtos;
using Slingcessories.Service.Models;

namespace Slingcessories.Tests.Controllers;

public class UsersControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoUsers()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new UsersController(db);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var users = okResult.Value.Should().BeAssignableTo<IEnumerable<UserDto>>().Subject;
        users.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_ReturnsUsers_OrderedByFirstNameThenLastName()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        db.Users.AddRange(
            new User { Id = "1", FirstName = "Bob", LastName = "Smith", Email = "bob@test.com" },
            new User { Id = "2", FirstName = "Alice", LastName = "Jones", Email = "alice@test.com" },
            new User { Id = "3", FirstName = "Alice", LastName = "Adams", Email = "alice.a@test.com" }
        );
        await db.SaveChangesAsync();

        var controller = new UsersController(db);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var users = okResult.Value.Should().BeAssignableTo<IEnumerable<UserDto>>().Subject.ToList();
        users.Should().HaveCount(3);
        users[0].FirstName.Should().Be("Alice");
        users[0].LastName.Should().Be("Adams");
        users[1].FirstName.Should().Be("Alice");
        users[1].LastName.Should().Be("Jones");
        users[2].FirstName.Should().Be("Bob");
        users[2].LastName.Should().Be("Smith");
    }

    [Fact]
    public async Task GetById_ReturnsUser_WhenUserExists()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var user = new User { Id = "test-id", FirstName = "John", LastName = "Doe", Email = "john@test.com" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var controller = new UsersController(db);

        // Act
        var result = await controller.GetById("test-id");

        // Assert
        result.Result.Should().BeNull();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be("test-id");
        result.Value.FirstName.Should().Be("John");
        result.Value.LastName.Should().Be("Doe");
        result.Value.Email.Should().Be("john@test.com");
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new UsersController(db);

        // Act
        var result = await controller.GetById("non-existent-id");

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task Register_CreatesUser_WhenValidData()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new UsersController(db);
        var createDto = new CreateUserDto("Jane", "Doe", "jane@test.com");

        // Act
        var result = await controller.Register(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var userDto = createdResult.Value.Should().BeOfType<UserDto>().Subject;
        userDto.FirstName.Should().Be("Jane");
        userDto.LastName.Should().Be("Doe");
        userDto.Email.Should().Be("jane@test.com");
        userDto.Id.Should().NotBeNullOrEmpty();

        // Verify user was saved to database
        var savedUser = await db.Users.FindAsync(userDto.Id);
        savedUser.Should().NotBeNull();
        savedUser!.Email.Should().Be("jane@test.com");
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenEmailAlreadyExists()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        db.Users.Add(new User { Id = "1", FirstName = "John", LastName = "Doe", Email = "existing@test.com" });
        await db.SaveChangesAsync();

        var controller = new UsersController(db);
        var createDto = new CreateUserDto("Jane", "Smith", "existing@test.com");

        // Act
        var result = await controller.Register(createDto);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("A user with this email already exists.");
    }

    [Fact]
    public async Task Update_UpdatesUser_WhenValidData()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var user = new User { Id = "test-id", FirstName = "John", LastName = "Doe", Email = "john@test.com" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var controller = new UsersController(db);
        var updateDto = new UserDto("test-id", "Jane", "Smith", "jane@test.com");

        // Act
        var result = await controller.Update("test-id", updateDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        // Verify update
        var updatedUser = await db.Users.FindAsync("test-id");
        updatedUser!.FirstName.Should().Be("Jane");
        updatedUser.LastName.Should().Be("Smith");
        updatedUser.Email.Should().Be("jane@test.com");
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenIdMismatch()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new UsersController(db);
        var updateDto = new UserDto("id1", "Jane", "Smith", "jane@test.com");

        // Act
        var result = await controller.Update("id2", updateDto);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("ID mismatch");
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new UsersController(db);
        var updateDto = new UserDto("non-existent", "Jane", "Smith", "jane@test.com");

        // Act
        var result = await controller.Update("non-existent", updateDto);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_DeletesUser_WhenUserExists()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var user = new User { Id = "test-id", FirstName = "John", LastName = "Doe", Email = "john@test.com" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var controller = new UsersController(db);

        // Act
        var result = await controller.Delete("test-id");

        // Assert
        result.Should().BeOfType<NoContentResult>();

        // Verify deletion
        var deletedUser = await db.Users.FindAsync("test-id");
        deletedUser.Should().BeNull();
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var controller = new UsersController(db);

        // Act
        var result = await controller.Delete("non-existent");

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
