using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Slingcessories.Layout;
using Slingcessories.Services;

namespace Slingcessories.Tests.Blazor.Layout;

public class MainLayoutTests : TestContext
{
    [Fact]
    public void MainLayout_WhenUserIsLoggedOut_ShowsRegisterAndHidesLogout()
    {
        ConfigureJsInterop();
        RegisterServices(isLoggedIn: false);

        var cut = RenderComponent<MainLayout>();

        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Register"));
        cut.Markup.Should().NotContain("Log out");
        cut.Markup.Should().NotContain("Switch User");
    }

    [Fact]
    public async Task MainLayout_WhenUserIsLoggedIn_ShowsLogoutAndHidesRegister()
    {
        ConfigureJsInterop();
        await RegisterServicesAsync(isLoggedIn: true);

        var cut = RenderComponent<MainLayout>();

        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Log out"));
        cut.Markup.Should().NotContain("Register");
        cut.Markup.Should().NotContain("Switch User");
    }

    [Fact]
    public async Task MainLayout_WhenLogoutIsClicked_ClearsUserAndShowsRegister()
    {
        ConfigureJsInterop();
        await RegisterServicesAsync(isLoggedIn: true);

        var cut = RenderComponent<MainLayout>();
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Log out"));

        var logoutButton = cut.FindAll("button").Single(b => b.TextContent.Contains("Log out", StringComparison.Ordinal));
        logoutButton.Click();

        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Register"));
        cut.Markup.Should().NotContain("Log out");
    }

    private void ConfigureJsInterop()
    {
        JSInterop.SetupVoid("indexedDbHelper.init");
        JSInterop.Setup<bool>("indexedDbHelper.isOnline").SetResult(true);
        JSInterop.SetupVoid("indexedDbHelper.setDotNetHelper", _ => true);
        JSInterop.Setup<List<object>>("indexedDbHelper.getPendingChanges").SetResult(new List<object>());
        JSInterop.SetupVoid("localStorage.setItem", _ => true);
        JSInterop.SetupVoid("localStorage.removeItem", _ => true);
    }

    private void RegisterServices(bool isLoggedIn)
    {
        var userStateService = new UserStateService(JSInterop.JSRuntime);
        var offlineDataService = new OfflineDataService(JSInterop.JSRuntime);
        var userService = CreateUserService();
        var authService = CreateAuthService();

        if (isLoggedIn)
        {
            throw new InvalidOperationException("Use RegisterServicesAsync when logged-in state is needed.");
        }

        Services.AddSingleton(userStateService);
        Services.AddSingleton(offlineDataService);
        Services.AddSingleton(userService);
        Services.AddSingleton(authService);
    }

    private async Task RegisterServicesAsync(bool isLoggedIn)
    {
        var userStateService = new UserStateService(JSInterop.JSRuntime);
        var offlineDataService = new OfflineDataService(JSInterop.JSRuntime);
        var userService = CreateUserService();
        var authService = CreateAuthService();

        if (isLoggedIn)
        {
            await userStateService.SetCurrentUserAsync("user-1");
        }

        Services.AddSingleton(userStateService);
        Services.AddSingleton(offlineDataService);
        Services.AddSingleton(userService);
        Services.AddSingleton(authService);
    }

    private static UserService CreateUserService()
    {
        var handler = new StubUserApiHandler();
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        return new UserService(httpClient);
    }

    private AuthService CreateAuthService()
    {
        var handler = new StubAuthApiHandler();
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        return new AuthService(httpClient, JSInterop.JSRuntime);
    }

    private sealed class StubUserApiHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Get && request.RequestUri?.PathAndQuery == "/api/users/user-1")
            {
                var content = """
                    {"id":"user-1","firstName":"Test","lastName":"User","email":"test.user@slingcessories.local"}
                    """;

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                });
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }

    private sealed class StubAuthApiHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Post && request.RequestUri?.PathAndQuery == "/api/auth/login")
            {
                var auth = new AuthResponseDto(
                    Token: "test-token",
                    ExpiresAtUtc: DateTime.UtcNow.AddHours(1),
                    UserId: "user-1",
                    Email: "test.user@slingcessories.local",
                    FirstName: "Test",
                    LastName: "User");

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(auth)
                });
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }
    }
    }
}
