using FluentAssertions;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using Slingcessories.Services;

namespace Slingcessories.Tests.Blazor.Services;

/// <summary>
/// Unit tests for UserStateService (Blazor)
/// Tests user selection, state persistence, and event notifications
/// </summary>
public class UserStateServiceTests
{
    private readonly Mock<IJSRuntime> _jsRuntimeMock;
    private readonly UserStateService _service;

    public UserStateServiceTests()
    {
        _jsRuntimeMock = new Mock<IJSRuntime>();
        _service = new UserStateService(_jsRuntimeMock.Object);
    }

    #region Initialization Tests

    [Fact]
    public async Task InitializeAsync_WhenUserIdExists_ShouldLoadFromStorage()
    {
        // Arrange
        var expectedUserId = "user-123";
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("localStorage.getItem", new object[] { "currentUserId" }))
            .ReturnsAsync(expectedUserId);

        // Act
        await _service.InitializeAsync();

        // Assert
        _service.CurrentUserId.Should().Be(expectedUserId);
    }

    [Fact]
    public async Task InitializeAsync_WhenNoUserIdExists_ShouldBeNull()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("localStorage.getItem", new object[] { "currentUserId" }))
            .ReturnsAsync((string?)null);

        // Act
        await _service.InitializeAsync();

        // Assert
        _service.CurrentUserId.Should().BeNull();
    }

    [Fact]
    public async Task InitializeAsync_WhenEmptyStringReturned_ShouldBeNull()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("localStorage.getItem", new object[] { "currentUserId" }))
            .ReturnsAsync(string.Empty);

        // Act
        await _service.InitializeAsync();

        // Assert
        _service.CurrentUserId.Should().BeNull();
    }

    [Fact]
    public async Task InitializeAsync_WhenLocalStorageThrows_ShouldNotThrow()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("localStorage.getItem", new object[] { "currentUserId" }))
            .ThrowsAsync(new JSException("localStorage not available"));

        // Act
        Func<Task> act = async () => await _service.InitializeAsync();

        // Assert
        await act.Should().NotThrowAsync();
        _service.CurrentUserId.Should().BeNull();
    }

    [Fact]
    public void CurrentUserId_BeforeInitialization_ShouldBeNull()
    {
        // Assert
        _service.CurrentUserId.Should().BeNull();
    }

    #endregion

    #region SetCurrentUserAsync Tests

    [Fact]
    public async Task SetCurrentUserAsync_ShouldUpdateCurrentUserId()
    {
        // Arrange
        var userId = "user-456";
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.SetCurrentUserAsync(userId);

        // Assert
        _service.CurrentUserId.Should().Be(userId);
    }

    [Fact]
    public async Task SetCurrentUserAsync_ShouldPersistToLocalStorage()
    {
        // Arrange
        var userId = "user-789";
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.SetCurrentUserAsync(userId);

        // Assert
        _jsRuntimeMock.Verify(
            x => x.InvokeAsync<IJSVoidResult>(
                "localStorage.setItem",
                It.Is<object[]>(args =>
                    args.Length == 2 &&
                    args[0].ToString() == "currentUserId" &&
                    args[1].ToString() == userId)),
            Times.Once);
    }

    [Fact]
    public async Task SetCurrentUserAsync_ShouldRaiseOnUserChangedEvent()
    {
        // Arrange
        var eventRaised = false;
        _service.OnUserChanged += () => eventRaised = true;

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.SetCurrentUserAsync("user-abc");

        // Assert
        eventRaised.Should().BeTrue();
    }

    [Fact]
    public async Task SetCurrentUserAsync_WithMultipleSubscribers_ShouldNotifyAll()
    {
        // Arrange
        var subscriber1Called = false;
        var subscriber2Called = false;

        _service.OnUserChanged += () => subscriber1Called = true;
        _service.OnUserChanged += () => subscriber2Called = true;

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.SetCurrentUserAsync("user-def");

        // Assert
        subscriber1Called.Should().BeTrue();
        subscriber2Called.Should().BeTrue();
    }

    [Fact]
    public async Task SetCurrentUserAsync_WhenLocalStorageThrows_ShouldNotThrow()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .ThrowsAsync(new JSException("Storage full"));

        // Act
        Func<Task> act = async () => await _service.SetCurrentUserAsync("user-xyz");

        // Assert
        await act.Should().NotThrowAsync();
        _service.CurrentUserId.Should().Be("user-xyz"); // Should still update in-memory state
    }

    [Fact]
    public async Task SetCurrentUserAsync_MultipleTimesSequentially_ShouldUpdateCorrectly()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.SetCurrentUserAsync("user-1");
        await _service.SetCurrentUserAsync("user-2");
        await _service.SetCurrentUserAsync("user-3");

        // Assert
        _service.CurrentUserId.Should().Be("user-3");
    }

    #endregion

    #region ClearCurrentUserAsync Tests

    [Fact]
    public async Task ClearCurrentUserAsync_ShouldSetCurrentUserIdToNull()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.removeItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        await _service.SetCurrentUserAsync("user-123");

        // Act
        await _service.ClearCurrentUserAsync();

        // Assert
        _service.CurrentUserId.Should().BeNull();
    }

    [Fact]
    public async Task ClearCurrentUserAsync_ShouldRemoveFromLocalStorage()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.removeItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.ClearCurrentUserAsync();

        // Assert
        _jsRuntimeMock.Verify(
            x => x.InvokeAsync<IJSVoidResult>(
                "localStorage.removeItem",
                It.Is<object[]>(args =>
                    args.Length == 1 &&
                    args[0].ToString() == "currentUserId")),
            Times.Once);
    }

    [Fact]
    public async Task ClearCurrentUserAsync_ShouldRaiseOnUserChangedEvent()
    {
        // Arrange
        var eventRaised = false;
        _service.OnUserChanged += () => eventRaised = true;

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.removeItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.ClearCurrentUserAsync();

        // Assert
        eventRaised.Should().BeTrue();
    }

    [Fact]
    public async Task ClearCurrentUserAsync_WhenLocalStorageThrows_ShouldNotThrow()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.removeItem", It.IsAny<object[]>()))
            .ThrowsAsync(new JSException("localStorage error"));

        // Act
        Func<Task> act = async () => await _service.ClearCurrentUserAsync();

        // Assert
        await act.Should().NotThrowAsync();
        _service.CurrentUserId.Should().BeNull(); // Should still clear in-memory state
    }

    #endregion

    #region Event Management Tests

    [Fact]
    public async Task OnUserChanged_WhenNoSubscribers_ShouldNotThrow()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        Func<Task> act = async () => await _service.SetCurrentUserAsync("user-123");

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task OnUserChanged_ShouldBeInvokedInCorrectOrder()
    {
        // Arrange
        var callOrder = new List<int>();
        _service.OnUserChanged += () => callOrder.Add(1);
        _service.OnUserChanged += () => callOrder.Add(2);
        _service.OnUserChanged += () => callOrder.Add(3);

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.SetCurrentUserAsync("user-123");

        // Assert
        callOrder.Should().ContainInOrder(1, 2, 3);
    }

    #endregion

    #region Integration Scenarios

    [Fact]
    public async Task Scenario_SetUserThenClear_ShouldWorkCorrectly()
    {
        // Arrange
        var eventCount = 0;
        _service.OnUserChanged += () => eventCount++;

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.removeItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.SetCurrentUserAsync("user-123");
        var userIdAfterSet = _service.CurrentUserId;

        await _service.ClearCurrentUserAsync();
        var userIdAfterClear = _service.CurrentUserId;

        // Assert
        userIdAfterSet.Should().Be("user-123");
        userIdAfterClear.Should().BeNull();
        eventCount.Should().Be(2); // One for set, one for clear
    }

    [Fact]
    public async Task Scenario_InitializeWithExistingUserThenChange_ShouldMaintainState()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("localStorage.getItem", new object[] { "currentUserId" }))
            .ReturnsAsync("original-user");

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.InitializeAsync();
        var originalUser = _service.CurrentUserId;

        await _service.SetCurrentUserAsync("new-user");
        var newUser = _service.CurrentUserId;

        // Assert
        originalUser.Should().Be("original-user");
        newUser.Should().Be("new-user");
    }

    [Fact]
    public async Task Scenario_MultipleSetOperationsWithEvents_ShouldTrackAllChanges()
    {
        // Arrange
        var userChanges = new List<string?>();
        _service.OnUserChanged += () => userChanges.Add(_service.CurrentUserId);

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.removeItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.SetCurrentUserAsync("user-1");
        await _service.SetCurrentUserAsync("user-2");
        await _service.ClearCurrentUserAsync();
        await _service.SetCurrentUserAsync("user-3");

        // Assert
        userChanges.Should().ContainInOrder("user-1", "user-2", null, "user-3");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task SetCurrentUserAsync_WithNullUserId_ShouldSetToNull()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.SetCurrentUserAsync(null!);

        // Assert
        _service.CurrentUserId.Should().BeNull();
    }

    [Fact]
    public async Task SetCurrentUserAsync_WithEmptyString_ShouldSetToEmptyString()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.SetCurrentUserAsync(string.Empty);

        // Assert
        _service.CurrentUserId.Should().Be(string.Empty);
    }

    [Fact]
    public async Task SetCurrentUserAsync_WithWhitespace_ShouldPreserveWhitespace()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.SetCurrentUserAsync("   ");

        // Assert
        _service.CurrentUserId.Should().Be("   ");
    }

    [Fact]
    public async Task SetCurrentUserAsync_WithSpecialCharacters_ShouldHandleCorrectly()
    {
        // Arrange
        var specialUserId = "user@domain.com-123_ABC";
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("localStorage.setItem", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.SetCurrentUserAsync(specialUserId);

        // Assert
        _service.CurrentUserId.Should().Be(specialUserId);
    }

    #endregion
}
