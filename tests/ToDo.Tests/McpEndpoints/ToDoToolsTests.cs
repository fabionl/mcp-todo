using System.Text.Json;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using ToDo.Mcp.McpEndpoints;
using ToDo.Mcp.ToDoItems.Models;
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
    
    [Fact]
    public async Task GetTodos_ReturnsTodos_WhenTodosExist()
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMinutes(1));
        var logger = LoggerFactory.CreateLogger<ToDoToolsTests>();

        // Arrange
        await using var scope = GetScope();
        await WithToDoService(async (toDoService) =>
        {
            await toDoService.CreateTodoAsync("Test Todo 1", "Description 1");
            await toDoService.CreateTodoAsync("Test Todo 2", "Description 2");
            Assert.NotEmpty(await toDoService.GetToDosAsync());
        }, scope);
        await WithToDoService(async (toDoService) =>
        {
            Assert.NotEmpty(await toDoService.GetToDosAsync());
        }, scope);
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
        Assert.NotNull(response);
        Assert.False(response.IsError);
        Assert.NotNull(response.Content);
        Assert.NotEmpty(response.Content);

        var resultItems = response.Content
            .Select(c =>
            {
                logger.LogInformation("Content Item: {Text}", JsonSerializer.Serialize(c));
                return c.Text;
            })
            .Where(text => !string.IsNullOrEmpty(text))
            .Select(text => text!)
            .SelectMany(text =>
            {
                logger.LogInformation("Item Text: {Text}", text);
                return JsonSerializer.Deserialize<ToDoItem[]>(text, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                }) ?? [];
            })
            .ToArray();
            
        Assert.NotEmpty(resultItems);
        Assert.Multiple(
            () => Assert.All(resultItems, Assert.NotNull),
            () => Assert.All(resultItems, item => Assert.NotEmpty(item.Title)),
            () => Assert.All(resultItems, item => Assert.NotEmpty(item.Description)),
            () => Assert.All(resultItems, item => Assert.NotEqual(Guid.Empty, item.Id)),
            () => Assert.All(resultItems, item => Assert.NotEqual(DateTime.MinValue, item.CreatedAt)),
            () => Assert.All(resultItems, item => Assert.NotNull(item.Status)),
            () => Assert.All(resultItems, item => Assert.False(item.Status.IsCompleted)),
            () => Assert.All(resultItems, item => Assert.False(item.IsCompleted)),
            () => Assert.All(resultItems, item => Assert.Null(item.Status.CompletedAt))
        );
    }
}