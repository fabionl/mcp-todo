using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Serilog;
using ToDo.Mcp.McpEndpoints;
using Xunit.Abstractions;

namespace ToDo.Tests.McpEndpoints;

public class ToDoToolsTests(ITestOutputHelper output) : McpServerFixture(output.ToLoggerFactory())
{
    [Fact]
    public async Task GetTodos_ReturnsEmptyList_WhenNoTodosExist()
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMinutes(1));
        var logger = LoggerFactory.CreateLogger<ToDoToolsTests>();
        Log.Logger = new LoggerConfiguration()
            .WriteTo.TestOutput(output)
            .CreateLogger();

        // Arrange
        await using var scope = Server.Services.CreateAsyncScope();
        var mcpServerTools = scope.ServiceProvider.GetServices<McpServerTool>();
        var mcpServerTool = mcpServerTools.FirstOrDefault(t => t.ProtocolTool.Name == nameof(ToDoTools.GetTodos));
        Debug.Assert(mcpServerTool is not null, "mcpServerTool cannot be found");
        // foreach (var mcpServerTool in mcpServerTools)
        // {
        //     logger.LogInformation("{Name} Input Schema: {InputSchema}",
        //         mcpServerTool.ProtocolTool.Name,
        //         JsonSerializer.Serialize(mcpServerTool.ProtocolTool.InputSchema));
        // }

        // Act
        var mcpServer = scope.ServiceProvider.GetRequiredService<IMcpServer>();
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