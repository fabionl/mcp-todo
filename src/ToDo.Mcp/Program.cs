using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.AspNetCore;

var builder = Host.CreateEmptyApplicationBuilder(settings: null);
builder.Services
  .AddMcpServer()
  .WithStdioServerTransport();
  // .WithPrompts<MonkeyPrompts>()
  // .WithResources<MonkeyResources>()
  // .WithTools<MonkeyTools>();

var app = builder.Build();

Console.WriteLine("Starting...");

await app.RunAsync();

Console.WriteLine("Finished");
