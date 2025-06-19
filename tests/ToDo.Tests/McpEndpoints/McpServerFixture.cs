using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using Xunit.Abstractions;

namespace ToDo.Tests.McpEndpoints;

public class McpServerFixture
{
    protected readonly ILoggerFactory LoggerFactory;
    protected readonly IHost Server;
    
    public McpServerFixture(ILoggerFactory loggerFactory)
    {
        LoggerFactory = loggerFactory;
        Server = AppHost.CreateHostBuilder()
            .Build();
    }
    
    protected McpServerTool GetServerTool() =>
        Server.Services.GetRequiredService<McpServerTool>();
}