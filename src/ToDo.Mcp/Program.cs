using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Serilog;

using ToDo.Mcp.ToDoItems;
using ToDo.Mcp.McpEndpoints;
using ToDo.Mcp.Services.TimeProviders;

var app = AppHost.CreateHostBuilder(args)
  .Build();

try
{
  Log.Information("Starting MCP server...");

  await app.RunAsync();
}
catch (Exception ex)
{
  Log.Fatal(ex, "Error starting MCP server");
  throw;
}
finally
{
  Log.CloseAndFlush();
}

public static class AppHost
{
  public static IHostBuilder CreateHostBuilder() => CreateHostBuilder([]);
  public static IHostBuilder CreateHostBuilder(string[] args)
  {
    // TODO: check is args include --logs with a location for logs
    var homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    var appDataPath = Path.Combine(homePath, ".todo-mcp");
    if (!Directory.Exists(appDataPath))
    {
      Directory.CreateDirectory(appDataPath);
    }

    var logPath = Path.Combine(appDataPath, "logs");

    Log.Logger = new LoggerConfiguration()
      .WriteTo.Console(standardErrorFromLevel: Serilog.Events.LogEventLevel.Verbose)
      .WriteTo.File(
        Path.Combine(logPath, "todo-mcp.log"),
        rollingInterval: RollingInterval.Day
      )
      .CreateLogger();

    var builder = Host.CreateDefaultBuilder(args)
      .ConfigureServices((hostContext, services) =>
      {
        services.AddAppServices();
      });



    return builder;
  }

  private static IServiceCollection AddAppServices(this IServiceCollection services)
  {
    services.AddLogging(loggingBuilder =>
    {
      loggingBuilder.AddSerilog();
    });

    services
      .AddMcpServer()
      .WithStdioServerTransport()
      .WithTools<ToDoTools>();

    services
      .AddSingleton<ITimeProvider, SystemTimeProvider>()
      .AddSingleton<IToDoService, ToDoService>();

    return services;
  }
}
