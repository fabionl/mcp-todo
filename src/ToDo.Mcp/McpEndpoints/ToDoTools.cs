using ModelContextProtocol.Server;
using System.ComponentModel;

using ToDo.Mcp.ToDoItems;
using ToDo.Mcp.ToDoItems.Models;

namespace ToDo.Mcp.McpEndpoints;

[McpServerToolType]
public class ToDoTools(IToDoService _todoService)
{
  [McpServerTool, Description("Get all TODO items")]
    public async Task<IEnumerable<ToDoItem>> GetTodos()
    {
        return await _todoService.GetToDosAsync();
    }

    [McpServerTool, Description("Get a TODO item by ID")]
    public async Task<ToDoItem> GetTodoById(
        [Description("The ID of the TODO item")] Guid id)
    {
        return await _todoService.GetTodoByIdAsync(id);
    }

    [McpServerTool, Description("Create a new TODO item")]
    public async Task<ToDoItem> CreateTodo(
        [Description("The title of the TODO item")] string title,
        [Description("The description of the TODO item")] string description = "")
    {
        return await _todoService.CreateTodoAsync(title, description);
    }

    [McpServerTool, Description("Update a TODO item")]
    public async Task<ToDoItem> UpdateTodo(
        [Description("The ID of the TODO item")] Guid id,
        [Description("The title of the TODO item")] string title,
        [Description("The description of the TODO item")] string description)
    {
        return await _todoService.UpdateTodoAsync(id, title, description);
    }

    [McpServerTool, Description("Delete a TODO item")]
    public async Task<ToDoItem> DeleteTodo(
        [Description("The ID of the TODO item")] Guid id)
    {
        return await _todoService.DeleteTodoAsync(id);
    }

    [McpServerTool, Description("Mark a TODO item as completed")]
    public async Task<ToDoItem> MarkAsCompleted(
        [Description("The ID of the TODO item")] Guid id)
    {
        return await _todoService.MarkAsCompletedAsync(id);
    }
}
