using FluentAssertions;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using Slingcessories.Services;
using System.Text.Json;

namespace Slingcessories.Tests.Blazor.Services;

/// <summary>
/// Unit tests for OfflineDataService (Blazor WebAssembly)
/// Tests offline caching, network detection, and data synchronization using IndexedDB
/// </summary>
public class OfflineDataServiceTests
{
    private readonly Mock<IJSRuntime> _jsRuntimeMock;
    private readonly OfflineDataService _service;

    public OfflineDataServiceTests()
    {
        _jsRuntimeMock = new Mock<IJSRuntime>();
        _service = new OfflineDataService(_jsRuntimeMock.Object);
    }

    #region Initialization Tests

    [Fact]
    public async Task InitializeAsync_ShouldCallIndexedDbInit()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.init", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<bool>("indexedDbHelper.isOnline", It.IsAny<object[]>()))
            .ReturnsAsync(true);

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.setDotNetHelper", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.InitializeAsync();

        // Assert
        _jsRuntimeMock.Verify(
            x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.init", It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public async Task InitializeAsync_ShouldSetIsOnlineStatus()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.init", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<bool>("indexedDbHelper.isOnline", It.IsAny<object[]>()))
            .ReturnsAsync(true);

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.setDotNetHelper", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.InitializeAsync();

        // Assert
        _service.IsOnline.Should().BeTrue();
    }

    [Fact]
    public async Task InitializeAsync_WhenOffline_ShouldSetIsOnlineToFalse()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.init", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<bool>("indexedDbHelper.isOnline", It.IsAny<object[]>()))
            .ReturnsAsync(false);

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.setDotNetHelper", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.InitializeAsync();

        // Assert
        _service.IsOnline.Should().BeFalse();
    }

    [Fact]
    public async Task InitializeAsync_ShouldRegisterDotNetHelper()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.init", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<bool>("indexedDbHelper.isOnline", It.IsAny<object[]>()))
            .ReturnsAsync(true);

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.setDotNetHelper", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.InitializeAsync();

        // Assert
        _jsRuntimeMock.Verify(
            x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.setDotNetHelper", It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public async Task InitializeAsync_WhenJSThrowsException_ShouldNotThrow()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.init", It.IsAny<object[]>()))
            .ThrowsAsync(new JSException("JS Error"));

        // Act
        Func<Task> act = async () => await _service.InitializeAsync();

        // Assert
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region Cache Operations - SetCachedDataAsync

    [Fact]
    public async Task SetCachedDataAsync_ShouldSerializeAndCacheData()
    {
        // Arrange
        var testData = new TestModel { Id = 1, Name = "Test" };
        var cacheKey = "test-key";

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.set", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.SetCachedDataAsync(cacheKey, testData);

        // Assert
        _jsRuntimeMock.Verify(
            x => x.InvokeAsync<IJSVoidResult>(
                "indexedDbHelper.set",
                It.Is<object[]>(args =>
                    args.Length == 2 &&
                    args[0].ToString() == cacheKey &&
                    args[1] is string)),
            Times.Once);
    }

    [Fact]
    public async Task SetCachedDataAsync_ShouldSerializeComplexObject()
    {
        // Arrange
        var testData = new List<TestModel>
        {
            new TestModel { Id = 1, Name = "Test1" },
            new TestModel { Id = 2, Name = "Test2" }
        };
        var cacheKey = "list-key";
        string? capturedJson = null;

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.set", It.IsAny<object[]>()))
            .Callback<string, object[]>((_, args) =>
            {
                capturedJson = args[1] as string;
            })
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.SetCachedDataAsync(cacheKey, testData);

        // Assert
        capturedJson.Should().NotBeNullOrEmpty();
        var deserialized = JsonSerializer.Deserialize<List<TestModel>>(capturedJson!);
        deserialized.Should().HaveCount(2);
        deserialized![0].Name.Should().Be("Test1");
    }

    [Fact]
    public async Task SetCachedDataAsync_WhenJSThrowsException_ShouldNotThrow()
    {
        // Arrange
        var testData = new TestModel { Id = 1, Name = "Test" };
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.set", It.IsAny<object[]>()))
            .ThrowsAsync(new JSException("Storage full"));

        // Act
        Func<Task> act = async () => await _service.SetCachedDataAsync("key", testData);

        // Assert
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region Cache Operations - GetCachedDataAsync

    [Fact]
    public async Task GetCachedDataAsync_ShouldDeserializeAndReturnData()
    {
        // Arrange
        var expectedData = new TestModel { Id = 1, Name = "Test" };
        var json = JsonSerializer.Serialize(expectedData);
        var cacheKey = "test-key";

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("indexedDbHelper.get", It.Is<object[]>(args => args[0].ToString() == cacheKey)))
            .ReturnsAsync(json);

        // Act
        var result = await _service.GetCachedDataAsync<TestModel>(cacheKey);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Test");
    }

    [Fact]
    public async Task GetCachedDataAsync_WhenKeyNotFound_ShouldReturnDefault()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("indexedDbHelper.get", It.IsAny<object[]>()))
            .ReturnsAsync((string?)null);

        // Act
        var result = await _service.GetCachedDataAsync<TestModel>("missing-key");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCachedDataAsync_WhenEmptyString_ShouldReturnDefault()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("indexedDbHelper.get", It.IsAny<object[]>()))
            .ReturnsAsync(string.Empty);

        // Act
        var result = await _service.GetCachedDataAsync<TestModel>("empty-key");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCachedDataAsync_WhenInvalidJson_ShouldReturnDefault()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("indexedDbHelper.get", It.IsAny<object[]>()))
            .ReturnsAsync("invalid-json-{{{");

        // Act
        var result = await _service.GetCachedDataAsync<TestModel>("invalid-key");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCachedDataAsync_ShouldDeserializeList()
    {
        // Arrange
        var expectedList = new List<TestModel>
        {
            new TestModel { Id = 1, Name = "Test1" },
            new TestModel { Id = 2, Name = "Test2" }
        };
        var json = JsonSerializer.Serialize(expectedList);

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("indexedDbHelper.get", It.IsAny<object[]>()))
            .ReturnsAsync(json);

        // Act
        var result = await _service.GetCachedDataAsync<List<TestModel>>("list-key");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result![0].Name.Should().Be("Test1");
    }

    #endregion

    #region Cache Operations - RemoveCachedDataAsync

    [Fact]
    public async Task RemoveCachedDataAsync_ShouldCallJSRemove()
    {
        // Arrange
        var cacheKey = "test-key";
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.remove", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.RemoveCachedDataAsync(cacheKey);

        // Assert
        _jsRuntimeMock.Verify(
            x => x.InvokeAsync<IJSVoidResult>(
                "indexedDbHelper.remove",
                It.Is<object[]>(args => args[0].ToString() == cacheKey)),
            Times.Once);
    }

    [Fact]
    public async Task RemoveCachedDataAsync_WhenJSThrowsException_ShouldNotThrow()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.remove", It.IsAny<object[]>()))
            .ThrowsAsync(new JSException("Remove failed"));

        // Act
        Func<Task> act = async () => await _service.RemoveCachedDataAsync("key");

        // Assert
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region Cache Operations - ClearAllCacheAsync

    [Fact]
    public async Task ClearAllCacheAsync_ShouldCallJSClear()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.clear", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.ClearAllCacheAsync();

        // Assert
        _jsRuntimeMock.Verify(
            x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.clear", It.IsAny<object[]>()),
            Times.Once);
    }

    [Fact]
    public async Task ClearAllCacheAsync_WhenJSThrowsException_ShouldNotThrow()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.clear", It.IsAny<object[]>()))
            .ThrowsAsync(new JSException("Clear failed"));

        // Act
        Func<Task> act = async () => await _service.ClearAllCacheAsync();

        // Assert
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region Cache Operations - GetAllCacheKeysAsync

    [Fact]
    public async Task GetAllCacheKeysAsync_ShouldReturnKeys()
    {
        // Arrange
        var expectedKeys = new List<string> { "key1", "key2", "key3" };
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<List<string>>("indexedDbHelper.getAllKeys", It.IsAny<object[]>()))
            .ReturnsAsync(expectedKeys);

        // Act
        var result = await _service.GetAllCacheKeysAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedKeys);
    }

    [Fact]
    public async Task GetAllCacheKeysAsync_WhenNoKeys_ShouldReturnEmptyList()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<List<string>>("indexedDbHelper.getAllKeys", It.IsAny<object[]>()))
            .ReturnsAsync(new List<string>());

        // Act
        var result = await _service.GetAllCacheKeysAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllCacheKeysAsync_WhenJSReturnsNull_ShouldReturnEmptyList()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<List<string>>("indexedDbHelper.getAllKeys", It.IsAny<object[]>()))
            .ReturnsAsync((List<string>?)null);

        // Act
        var result = await _service.GetAllCacheKeysAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllCacheKeysAsync_WhenJSThrowsException_ShouldReturnEmptyList()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<List<string>>("indexedDbHelper.getAllKeys", It.IsAny<object[]>()))
            .ThrowsAsync(new JSException("Get keys failed"));

        // Act
        var result = await _service.GetAllCacheKeysAsync();

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region Network Status Events

    [Fact]
    public void HandleOnline_ShouldSetIsOnlineToTrue()
    {
        // Act
        _service.HandleOnline();

        // Assert
        _service.IsOnline.Should().BeTrue();
    }

    [Fact]
    public void HandleOnline_ShouldRaiseOnlineStatusChangedEvent()
    {
        // Arrange
        var eventRaised = false;
        var statusValue = false;

        _service.OnlineStatusChanged += (isOnline) =>
        {
            eventRaised = true;
            statusValue = isOnline;
        };

        // Act
        _service.HandleOnline();

        // Assert
        eventRaised.Should().BeTrue();
        statusValue.Should().BeTrue();
    }

    [Fact]
    public void HandleOffline_ShouldSetIsOnlineToFalse()
    {
        // Act
        _service.HandleOffline();

        // Assert
        _service.IsOnline.Should().BeFalse();
    }

    [Fact]
    public void HandleOffline_ShouldRaiseOnlineStatusChangedEvent()
    {
        // Arrange
        var eventRaised = false;
        var statusValue = true;

        _service.OnlineStatusChanged += (isOnline) =>
        {
            eventRaised = true;
            statusValue = isOnline;
        };

        // Act
        _service.HandleOffline();

        // Assert
        eventRaised.Should().BeTrue();
        statusValue.Should().BeFalse();
    }

    [Fact]
    public void OnlineStatusChanged_ShouldSupportMultipleSubscribers()
    {
        // Arrange
        var subscriber1Called = false;
        var subscriber2Called = false;

        _service.OnlineStatusChanged += (_) => subscriber1Called = true;
        _service.OnlineStatusChanged += (_) => subscriber2Called = true;

        // Act
        _service.HandleOnline();

        // Assert
        subscriber1Called.Should().BeTrue();
        subscriber2Called.Should().BeTrue();
    }

    #endregion

    #region Pending Changes

    [Fact]
    public async Task AddPendingChangeAsync_ShouldCallJS()
    {
        // Arrange
        var changeType = "create";
        var entityType = "accessory";
        var entityId = 123;
        var data = new TestModel { Id = 1, Name = "Test" };

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.addPendingChange", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.AddPendingChangeAsync(changeType, entityType, entityId, data);

        // Assert
        _jsRuntimeMock.Verify(
            x => x.InvokeAsync<IJSVoidResult>(
                "indexedDbHelper.addPendingChange",
                It.Is<object[]>(args =>
                    args.Length == 4 &&
                    args[0].ToString() == changeType &&
                    args[1].ToString() == entityType &&
                    (int)args[2] == entityId)),
            Times.Once);
    }

    [Fact]
    public async Task HasPendingChangesAsync_WhenChangesExist_ShouldReturnTrue()
    {
        // Arrange
        var changes = new List<object> { new { id = 1 }, new { id = 2 } };
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<List<object>>("indexedDbHelper.getPendingChanges", It.IsAny<object[]>()))
            .ReturnsAsync(changes);

        // Act
        var result = await _service.HasPendingChangesAsync();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task HasPendingChangesAsync_WhenNoChanges_ShouldReturnFalse()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<List<object>>("indexedDbHelper.getPendingChanges", It.IsAny<object[]>()))
            .ReturnsAsync(new List<object>());

        // Act
        var result = await _service.HasPendingChangesAsync();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ClearPendingChangesAsync_ShouldCallJS()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.clearPendingChanges", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        // Act
        await _service.ClearPendingChangesAsync();

        // Assert
        _jsRuntimeMock.Verify(
            x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.clearPendingChanges", It.IsAny<object[]>()),
            Times.Once);
    }

    #endregion

    #region Disposal

    [Fact]
    public async Task DisposeAsync_ShouldDisposeObjectReference()
    {
        // Arrange
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.init", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<bool>("indexedDbHelper.isOnline", It.IsAny<object[]>()))
            .ReturnsAsync(true);
        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.setDotNetHelper", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        await _service.InitializeAsync();

        // Act
        await _service.DisposeAsync();

        // Assert - Should not throw
        // DotNetObjectReference is disposed internally
    }

    [Fact]
    public async Task DisposeAsync_WhenNotInitialized_ShouldNotThrow()
    {
        // Act
        Func<Task> act = async () => await _service.DisposeAsync();

        // Assert
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region Integration Scenarios

    [Fact]
    public async Task Scenario_OnlineToOfflineTransition_ShouldMaintainCachedData()
    {
        // Arrange
        var testData = new TestModel { Id = 1, Name = "Test" };
        var json = JsonSerializer.Serialize(testData);

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.set", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("indexedDbHelper.get", It.IsAny<object[]>()))
            .ReturnsAsync(json);

        // Act - Cache data while online
        await _service.SetCachedDataAsync("test-key", testData);

        // Simulate going offline
        _service.HandleOffline();

        // Try to retrieve cached data
        var result = await _service.GetCachedDataAsync<TestModel>("test-key");

        // Assert
        _service.IsOnline.Should().BeFalse();
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test");
    }

    [Fact]
    public async Task Scenario_MultipleDataTypes_ShouldCacheIndependently()
    {
        // Arrange
        var stringData = "Hello World";
        var listData = new List<int> { 1, 2, 3 };
        var objectData = new TestModel { Id = 1, Name = "Test" };

        var stringJson = JsonSerializer.Serialize(stringData);
        var listJson = JsonSerializer.Serialize(listData);
        var objectJson = JsonSerializer.Serialize(objectData);

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("indexedDbHelper.set", It.IsAny<object[]>()))
            .ReturnsAsync(Mock.Of<IJSVoidResult>());

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("indexedDbHelper.get", It.Is<object[]>(args => args[0].ToString() == "string-key")))
            .ReturnsAsync(stringJson);

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("indexedDbHelper.get", It.Is<object[]>(args => args[0].ToString() == "list-key")))
            .ReturnsAsync(listJson);

        _jsRuntimeMock
            .Setup(x => x.InvokeAsync<string?>("indexedDbHelper.get", It.Is<object[]>(args => args[0].ToString() == "object-key")))
            .ReturnsAsync(objectJson);

        // Act
        await _service.SetCachedDataAsync("string-key", stringData);
        await _service.SetCachedDataAsync("list-key", listData);
        await _service.SetCachedDataAsync("object-key", objectData);

        var stringResult = await _service.GetCachedDataAsync<string>("string-key");
        var listResult = await _service.GetCachedDataAsync<List<int>>("list-key");
        var objectResult = await _service.GetCachedDataAsync<TestModel>("object-key");

        // Assert
        stringResult.Should().Be(stringData);
        listResult.Should().BeEquivalentTo(listData);
        objectResult!.Name.Should().Be("Test");
    }

    #endregion

    #region Test Helper Classes

    private class TestModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    #endregion
}
