using System.Diagnostics;

using ToDo.Mcp.Entities;
using ToDo.Mcp.Services;
using ToDo.Mcp.Services.TimeProviders;

namespace ToDo.Tests.Services;

public class ToDoServiceTests
{
    private readonly ITimeProvider _timeProvider;
    private readonly ToDoService _toDoService;

    public ToDoServiceTests()
    {
        _timeProvider = new StaticTimeProvider(DateTime.Now);
        _toDoService = new ToDoService(_timeProvider);
    }

    [Fact]
    public async Task GetToDosAsync_ReturnsInitialToDos()
    {
        // Act
        var initialToDoList = await _toDoService.GetToDosAsync();

        // Assert
        Assert.Single(initialToDoList);
    }

    [Fact]
    public async Task GetToDoByIdAsync_ReturnsToDo()
    {
        // Arrange
        var toDo1 = new ToDoItem { Title = "ToDo 1", Description = "Description 1" };
        await _toDoService.CreateTodoAsync(toDo1);

        // Act
        var result = await _toDoService.GetTodoByIdAsync(toDo1.Id);

        // Assert
        Assert.Equal(toDo1, result);
    }

    [Fact]
    public async Task CreateToDoAsync_AddsToDoToService()
    {
        // Arrange
        var toDo1 = new ToDoItem { Title = "ToDo 1", Description = "Description 1" };
        var initialToDoCount = (await _toDoService.GetToDosAsync()).Count();
        var expectedToDoCount = initialToDoCount + 1;

        // Act
        await _toDoService.CreateTodoAsync(toDo1);

        // Assert
        var result = await _toDoService.GetToDosAsync();
        Assert.Equal(expectedToDoCount, result.Count());
    }

    [Fact]
    public async Task UpdateToDoAsync_UpdatesToDo()
    {
        // Arrange
        var toDo1 = new ToDoItem { Title = "ToDo 1", Description = "Description 1" };
        await _toDoService.CreateTodoAsync(toDo1);

        // Act
        toDo1.Title = "Updated ToDo 1";
        await _toDoService.UpdateTodoAsync(toDo1);

        // Assert
        var result = await _toDoService.GetTodoByIdAsync(toDo1.Id);
        Assert.Equal(toDo1, result);
    }

    [Fact]
    public async Task DeleteToDoAsync_DeletesToDo()
    {
        // Arrange
        var initialToDoList = (await _toDoService.GetToDosAsync())
          .ToList();
        Debug.Assert(initialToDoList.Count > 0);

        // Act
        foreach (var todo in initialToDoList)
        {
            await _toDoService.DeleteTodoAsync(todo.Id);
        }

        // Assert
        var result = await _toDoService.GetToDosAsync();
        Assert.Empty(result);
    }

    [Fact]
    public async Task MarkAsCompleted_MarksToDoAsCompletedAsync()
    {
        // Arrange
        var toDo1 = new ToDoItem { Title = "ToDo 1", Description = "Description 1" };
        await _toDoService.CreateTodoAsync(toDo1);

        // Act
        await _toDoService.MarkAsCompletedAsync(toDo1.Id);

        // Assert
        var result = await _toDoService.GetTodoByIdAsync(toDo1.Id);
        Assert.True(result.IsCompleted);
        Assert.NotNull(result.CompletedAt);
    }
}
