using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using Serilog;
using ToDo.Mcp.ToDoItems;
using Xunit.Abstractions;

namespace ToDo.Tests.McpEndpoints;

public class McpServerFixture
{
    protected readonly ILoggerFactory LoggerFactory;
    protected readonly IHost Server;
  
    public McpServerFixture(ITestOutputHelper output)
    {
        LoggerFactory = output.ToLoggerFactory();
        Log.Logger = new LoggerConfiguration()
            .WriteTo.TestOutput(output)
            .CreateLogger();
        Server = AppHost.CreateHostBuilder()
            .Build();
    }

    protected async Task WithToDoService(Func<IToDoService, Task> action, AsyncServiceScope scope)
    {
        var toDoService = scope.ServiceProvider.GetRequiredService<IToDoService>();
        await action(toDoService);
    }
    
    protected IServiceProvider Services =>
        Server.Services;

    protected AsyncServiceScope GetScope() =>
        Services.CreateAsyncScope();

    protected McpServerTool GetMcpServerTool(string toolName)
    {
        using var scope = GetScope();
        return GetMcpServerTool(toolName, scope);
    }
    
    protected McpServerTool GetMcpServerTool(string toolName, AsyncServiceScope scope)
    {
        var mcpServerTool = GetMcpServerTools(scope)
            .FirstOrDefault(t => t.ProtocolTool.Name == toolName)
            ?? throw new InvalidOperationException($"MCP Server Tool '{toolName}' not found.");

        return mcpServerTool;
    }
    
    protected IMcpServer GetMcpServer(AsyncServiceScope scope)
    {
        var mcpServer = scope.ServiceProvider.GetRequiredService<IMcpServer>();
        return mcpServer;
    }
    
    private IEnumerable<McpServerTool> GetMcpServerTools(AsyncServiceScope scope)
    {
        var mcpServerTools = scope.ServiceProvider.GetServices<McpServerTool>();
        return mcpServerTools;
    }
    
}