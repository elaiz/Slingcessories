using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Slingcessories.Service.Controllers;
using Slingcessories.Service.Dtos;
using Slingcessories.Service.Models;

namespace Slingcessories.Tests.Controllers;

public class AuthControllerTests
{
    [Fact]
    public async Task Register_ReturnsBadRequest_WhenClientIdIsInvalid()
    {
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var userManager = CreateUserManagerMock();
        var environment = CreateEnvironmentMock(isDevelopment: true);
        var configuration = CreateConfiguration();
        var controller = new AuthController(userManager.Object, configuration, environment.Object, db);

        var dto = new AuthRegisterDto("John", "Doe", "john@test.com", "Password123!", "Invalid.Client");

        var result = await controller.Register(dto);

        var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("Invalid client identifier.");
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenClientIdIsInvalid()
    {
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var userManager = CreateUserManagerMock();
        var environment = CreateEnvironmentMock(isDevelopment: true);
        var configuration = CreateConfiguration();
        var controller = new AuthController(userManager.Object, configuration, environment.Object, db);

        var dto = new AuthLoginDto("john@test.com", "Password123!", "Invalid.Client");

        var result = await controller.Login(dto);

        var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("Invalid client identifier.");
    }

    [Fact]
    public async Task ForgotPassword_ReturnsResetToken_InDevelopment()
    {
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var userManager = CreateUserManagerMock();
        var environment = CreateEnvironmentMock(isDevelopment: true);
        var configuration = CreateConfiguration();

        var identityUser = new IdentityUser { Id = "user-1", Email = "john@test.com", UserName = "john@test.com" };
        userManager.Setup(x => x.FindByEmailAsync("john@test.com")).ReturnsAsync(identityUser);
        userManager.Setup(x => x.GeneratePasswordResetTokenAsync(identityUser)).ReturnsAsync("dev-reset-token");

        var controller = new AuthController(userManager.Object, configuration, environment.Object, db);

        var result = await controller.ForgotPassword(new ForgotPasswordDto("john@test.com"));

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.ToString().Should().Contain("dev-reset-token");
    }

    [Fact]
    public async Task ForgotPassword_DoesNotReturnToken_InProduction()
    {
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var userManager = CreateUserManagerMock();
        var environment = CreateEnvironmentMock(isDevelopment: false);
        var configuration = CreateConfiguration();

        var identityUser = new IdentityUser { Id = "user-1", Email = "john@test.com", UserName = "john@test.com" };
        userManager.Setup(x => x.FindByEmailAsync("john@test.com")).ReturnsAsync(identityUser);
        userManager.Setup(x => x.GeneratePasswordResetTokenAsync(identityUser)).ReturnsAsync("prod-reset-token");

        var controller = new AuthController(userManager.Object, configuration, environment.Object, db);

        var result = await controller.ForgotPassword(new ForgotPasswordDto("john@test.com"));

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.ToString().Should().NotContain("prod-reset-token");
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenPasswordIsInvalid()
    {
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var userManager = CreateUserManagerMock();
        var environment = CreateEnvironmentMock(isDevelopment: true);
        var configuration = CreateConfiguration();

        var identityUser = new IdentityUser { Id = "user-1", Email = "john@test.com", UserName = "john@test.com" };
        userManager.Setup(x => x.FindByEmailAsync("john@test.com")).ReturnsAsync(identityUser);
        userManager.Setup(x => x.CheckPasswordAsync(identityUser, "bad-pass")).ReturnsAsync(false);

        var controller = new AuthController(userManager.Object, configuration, environment.Object, db);

        var result = await controller.Login(new AuthLoginDto("john@test.com", "bad-pass", "Slingcessories.Blazor"));

        var unauthorized = result.Result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
        unauthorized.Value.Should().Be("Invalid email or password.");
    }

    [Fact]
    public async Task Login_ReturnsTokenAndCreatesProfile_WhenIdentityUserExistsButProfileDoesNot()
    {
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var userManager = CreateUserManagerMock();
        var environment = CreateEnvironmentMock(isDevelopment: true);
        var configuration = CreateConfiguration();

        var identityUser = new IdentityUser { Id = "user-1", Email = "john@test.com", UserName = "john@test.com" };
        userManager.Setup(x => x.FindByEmailAsync("john@test.com")).ReturnsAsync(identityUser);
        userManager.Setup(x => x.CheckPasswordAsync(identityUser, "Password123!")).ReturnsAsync(true);

        var controller = new AuthController(userManager.Object, configuration, environment.Object, db);

        var result = await controller.Login(new AuthLoginDto("john@test.com", "Password123!", "Slingcessories.React"));

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var auth = okResult.Value.Should().BeOfType<AuthResponseDto>().Subject;

        auth.Token.Should().NotBeNullOrWhiteSpace();
        auth.UserId.Should().Be("user-1");
        auth.Email.Should().Be("john@test.com");

        var savedProfile = await db.Users.FindAsync("user-1");
        savedProfile.Should().NotBeNull();
        savedProfile!.Email.Should().Be("john@test.com");
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenEmailAlreadyExists()
    {
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var userManager = CreateUserManagerMock();
        var environment = CreateEnvironmentMock(isDevelopment: true);
        var configuration = CreateConfiguration();

        var existing = new IdentityUser { Id = "user-1", Email = "john@test.com", UserName = "john@test.com" };
        userManager.Setup(x => x.FindByEmailAsync("john@test.com")).ReturnsAsync(existing);

        var controller = new AuthController(userManager.Object, configuration, environment.Object, db);

        var result = await controller.Register(new AuthRegisterDto("John", "Doe", "john@test.com", "Password123!", "Slingcessories.Maui"));

        var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("A user with this email already exists.");
    }

    [Fact]
    public async Task Register_CreatesIdentityUserAndProfile_WhenValidRequest()
    {
        await using var db = TestHelpers.CreateInMemoryDbContext();
        var userManager = CreateUserManagerMock();
        var environment = CreateEnvironmentMock(isDevelopment: true);
        var configuration = CreateConfiguration();

        userManager.Setup(x => x.FindByEmailAsync("jane@test.com")).ReturnsAsync((IdentityUser?)null);
        userManager.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), "Password123!"))
            .ReturnsAsync(IdentityResult.Success)
            .Callback<IdentityUser, string>((createdUser, _) =>
            {
                createdUser.Id = "new-user-id";
                createdUser.Email = "jane@test.com";
                createdUser.UserName = "jane@test.com";
            });

        var controller = new AuthController(userManager.Object, configuration, environment.Object, db);

        var result = await controller.Register(new AuthRegisterDto("Jane", "Doe", "jane@test.com", "Password123!", "Slingcessories.Blazor"));

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var auth = okResult.Value.Should().BeOfType<AuthResponseDto>().Subject;

        auth.UserId.Should().Be("new-user-id");
        auth.Email.Should().Be("jane@test.com");
        auth.FirstName.Should().Be("Jane");
        auth.LastName.Should().Be("Doe");
        auth.Token.Should().NotBeNullOrWhiteSpace();

        var profile = await db.Users.FindAsync("new-user-id");
        profile.Should().NotBeNull();
        profile!.Email.Should().Be("jane@test.com");
    }

    private static Mock<UserManager<IdentityUser>> CreateUserManagerMock()
    {
        var store = new Mock<IUserStore<IdentityUser>>();
        return new Mock<UserManager<IdentityUser>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);
    }

    private static Mock<IWebHostEnvironment> CreateEnvironmentMock(bool isDevelopment)
    {
        var env = new Mock<IWebHostEnvironment>();
        env.Setup(x => x.EnvironmentName).Returns(isDevelopment ? "Development" : "Production");
        return env;
    }

    private static IConfiguration CreateConfiguration()
    {
        var values = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "TEST_KEY_123456789012345678901234567890",
            ["Jwt:Issuer"] = "Slingcessories.Service",
            ["Jwt:ValidAudiences:0"] = "Slingcessories.Blazor",
            ["Jwt:ValidAudiences:1"] = "Slingcessories.Maui",
            ["Jwt:ValidAudiences:2"] = "Slingcessories.React",
            ["Jwt:ExpiresMinutes"] = "60"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }
}
