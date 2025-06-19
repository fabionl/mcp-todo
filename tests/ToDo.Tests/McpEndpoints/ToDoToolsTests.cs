using System.Text.Json;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using ToDo.Mcp.McpEndpoints;
using Xunit.Abstractions;

namespace ToDo.Tests.McpEndpoints;

public class ToDoToolsTests(ITestOutputHelper output) : McpServerFixture(output)
{
    [Fact]
    public async Task GetTodos_ReturnsEmptyList_WhenNoTodosExist()
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMinutes(1));
        var logger = LoggerFactory.CreateLogger<ToDoToolsTests>();

        // Arrange
        await using var scope = GetScope();
        var mcpServerTool = GetMcpServerTool(nameof(ToDoTools.GetTodos), scope);

        // Act
        var mcpServer = GetMcpServer(scope);
        var request = new RequestContext<CallToolRequestParams>(mcpServer)
        {
            Params = new CallToolRequestParams
            {
                Name = nameof(ToDoTools.GetTodos)
            }
        };
        var response = await mcpServerTool.InvokeAsync(
            request, cts.Token);

        // Assert
        logger.LogInformation("Response: {Response}",
            JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
        Assert.NotNull(response);
        Assert.False(response.IsError);
        Assert.NotNull(response.Content);
        
        logger.LogInformation("Response Content: {Content}",
            JsonSerializer.Serialize(response.Content, new JsonSerializerOptions { WriteIndented = true }));
    }
}