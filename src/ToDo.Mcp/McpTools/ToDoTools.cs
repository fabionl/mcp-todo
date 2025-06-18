using ModelContextProtocol.Server;
using System.ComponentModel;

using ToDo.Mcp.ToDoItems;
using ToDo.Mcp.ToDoItems.Models;

namespace ToDo.Mcp.McpTools;

[McpServerToolType]
public class ToDoTools(IToDoService _todoService)
{
  [McpServerTool, Description("Get all todo items")]
    public async Task<IEnumerable<ToDoItem>> GetTodos()
    {
        return await _todoService.GetToDosAsync();
    }

    [McpServerTool, Description("Get a todo item by ID")]
    public async Task<ToDoItem> GetTodoById(
        [Description("The ID of the todo item")] Guid id)
    {
        return await _todoService.GetTodoByIdAsync(id);
    }

    [McpServerTool, Description("Create a new todo item")]
    public async Task<ToDoItem> CreateTodo(
        [Description("The title of the todo item")] string title,
        [Description("The description of the todo item")] string description = "")
    {
        var todoBuilder = new ToDoItemBuilder()
            .WithTitle(title)
            .WithDescription(description);
        return await _todoService.CreateTodoAsync(todoBuilder.Build());
    }

    [McpServerTool, Description("Update a todo item")]
    public async Task<ToDoItem> UpdateTodo(
        [Description("The ID of the todo item")] Guid id,
        [Description("The title of the todo item")] string title,
        [Description("The description of the todo item")] string description,
        [Description("Whether the todo item is completed")] bool isCompleted)
    {
        var todoBuilder = new ToDoItemBuilder()
            .WithId(id)
            .WithTitle(title)
            .WithDescription(description)
            .WithIsCompleted(isCompleted);
        return await _todoService.UpdateTodoAsync(todoBuilder.Build());
    }

    [McpServerTool, Description("Delete a todo item")]
    public async Task<ToDoItem> DeleteTodo(
        [Description("The ID of the todo item")] Guid id)
    {
        return await _todoService.DeleteTodoAsync(id);
    }
}
