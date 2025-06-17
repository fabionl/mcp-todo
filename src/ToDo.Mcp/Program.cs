using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using ToDo.Mcp.Services;
using ToDo.Mcp.McpTools;

var builder = Host.CreateEmptyApplicationBuilder(settings: null);
builder.Services
  .AddMcpServer()
  .WithStdioServerTransport()
  .WithTools<ToDoTools>();

builder.Services.AddSingleton<IToDoService, ToDoService>();

var app = builder.Build();

Console.WriteLine("Starting...");

await app.RunAsync();

