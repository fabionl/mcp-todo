using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Serilog;

using ToDo.Mcp.ToDoItems;
using ToDo.Mcp.McpEndpoints;
using ToDo.Mcp.Services.TimeProviders;

var builder = Host.CreateEmptyApplicationBuilder(settings: null);

var homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
var appDataPath = Path.Combine(homePath, ".todo-mcp");
if (!Directory.Exists(appDataPath))
{
  Directory.CreateDirectory(appDataPath);
}

var logPath = Path.Combine(appDataPath, "logs");

Log.Logger = new LoggerConfiguration()
  // .WriteTo.Console()
  .WriteTo.File(
    Path.Combine(logPath, "todo-mcp.log"),
    rollingInterval: RollingInterval.Day
  )
  .CreateLogger();

builder.Services.AddLogging(loggingBuilder =>
{
  loggingBuilder.AddSerilog();
});

builder.Services
  .AddMcpServer()
  .WithStdioServerTransport()
  .WithTools<ToDoTools>();

builder.Services
  .AddSingleton<ITimeProvider, SystemTimeProvider>()
  .AddSingleton<IToDoService, ToDoService>();

var app = builder.Build();

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

