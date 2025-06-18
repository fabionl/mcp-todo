using ModelContextProtocol.Server;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

using ToDo.Mcp.ToDoItems;
using ToDo.Mcp.ToDoItems.Models;

namespace ToDo.Mcp.McpEndpoints;

[McpServerToolType]
public class ToDoTools(IToDoService _todoService, ILogger<ToDoTools> _logger)
{
  [McpServerTool, Description("Get all TODO items")]
    public async Task<IEnumerable<ToDoItem>> GetTodos()
    {
        _logger.LogInformation("Getting all TODO items");
        try
        {
            return await _todoService.GetToDosAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all TODO items");
            throw;
        }
    }

    [McpServerTool, Description("Get a TODO item by ID")]
    public async Task<ToDoItem> GetTodoById(
        [Description("The ID of the TODO item")] Guid id)
    {
        _logger.LogInformation("Getting TODO item by ID: {Id}", id);
        try
        {
            return await _todoService.GetTodoByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting TODO item by ID: {Id}", id);
            throw;
        }
    }

    [McpServerTool, Description("Create a new TODO item")]
    public async Task<ToDoItem> CreateTodo(
        [Description("The title of the TODO item")] string title,
        [Description("The description of the TODO item")] string description = "")
    {
        _logger.LogInformation("Creating new TODO item: {Title}, {Description}", title, description);
        try
        {
            return await _todoService.CreateTodoAsync(title, description);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating new TODO item: {Title}, {Description}", title, description);
            throw;
        }
    }

    [McpServerTool, Description("Update a TODO item")]
    public async Task<ToDoItem> UpdateTodo(
        [Description("The ID of the TODO item")] Guid id,
        [Description("The title of the TODO item")] string title,
        [Description("The description of the TODO item")] string description)
    {
        _logger.LogInformation("Updating TODO item: {Id}, {Title}, {Description}", id, title, description);
        try
        {
            return await _todoService.UpdateTodoAsync(id, title, description);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating TODO item: {Id}, {Title}, {Description}", id, title, description);
            throw;
        }
    }

    [McpServerTool, Description("Delete a TODO item")]
    public async Task<ToDoItem> DeleteTodo(
        [Description("The ID of the TODO item")] Guid id)
    {
        _logger.LogInformation("Deleting TODO item: {Id}", id);
        try
        {
            return await _todoService.DeleteTodoAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting TODO item: {Id}", id);
            throw;
        }
    }

    [McpServerTool, Description("Mark a TODO item as completed")]
    public async Task<ToDoItem> MarkAsCompleted(
        [Description("The ID of the TODO item")] Guid id)
    {
        _logger.LogInformation("Marking TODO item as completed: {Id}", id);
        try
        {
            return await _todoService.MarkAsCompletedAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking TODO item as completed: {Id}", id);
            throw;
        }
    }
}
