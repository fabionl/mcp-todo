using ModelContextProtocol.Client;
using ToDo.Tests.Managers;

namespace ToDo.Tests.McpEndpoints;

public class ToDoToolsTests : IAsyncLifetime
{
  private McpClientManager _mcpClientManager = null!;

  public Task InitializeAsync()
  {
    var host = AppHost.CreateHostBuilder()
        .Build();

    _mcpClientManager = new McpClientManager(host);

    return Task.CompletedTask;
  }

  public async Task DisposeAsync()
  {
    await _mcpClientManager.DisposeAsync();
  }

  [Fact]
  public async Task GetTodos_ShouldReturnEmptyList_WhenNoTodosExist()
  {
    // Arrange
    var client = await _mcpClientManager.CreateMcpClient();

    // Act
    var response = await client.CallToolAsync("getTodos");

    // Assert
    Assert.False(response.IsError);
    Assert.Empty(response.Content);
  }
}