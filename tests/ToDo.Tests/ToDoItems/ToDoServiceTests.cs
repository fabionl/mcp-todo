using System.Diagnostics;

using ToDo.Mcp.Services.TimeProviders;
using ToDo.Mcp.ToDoItems;

namespace ToDo.Tests.ToDoItems;

public class ToDoServiceTests
{
    private readonly ITimeProvider _timeProvider;
    private readonly ToDoService _toDoService;
    private readonly ToDoItemBuilder _toDoBuilder;

    public ToDoServiceTests()
    {
        _timeProvider = new StaticTimeProvider(DateTime.Now);
        _toDoService = new ToDoService(_timeProvider);
        _toDoBuilder = new ToDoItemBuilder()
          .WithTimeProvider(_timeProvider);
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
        var toDo1 = _toDoBuilder.WithTitle("ToDo 1")
          .WithDescription("Description 1")
          .Build();
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
        var toDo1 = _toDoBuilder.WithTitle("ToDo 1")
          .WithDescription("Description 1")
          .Build();
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
        var toDo1 = _toDoBuilder.WithTitle("ToDo 1")
          .WithDescription("Description 1")
          .Build();
        var createdToDo = await _toDoService.CreateTodoAsync(toDo1);
        Assert.Equal(toDo1, createdToDo);

        // Act
        toDo1.Title = "Updated ToDo 1";
        await _toDoService.UpdateTodoAsync(toDo1);

        // Assert
        var result = await _toDoService.GetTodoByIdAsync(toDo1.Id);
        Assert.Multiple(
          () => Assert.Equal(toDo1.Title, result.Title),
          () => Assert.Equal(toDo1.Description, result.Description),
          () => Assert.Equal(toDo1.IsCompleted, result.IsCompleted),
          () => Assert.Equal(toDo1.CreatedAt, result.CreatedAt),
          () => Assert.Equal(toDo1.CompletedAt, result.CompletedAt)
        );
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
        var toDo1 = _toDoBuilder.WithTitle("ToDo 1")
          .WithDescription("Description 1")
          .Build();
        await _toDoService.CreateTodoAsync(toDo1);

        // Act
        await _toDoService.MarkAsCompletedAsync(toDo1.Id);

        // Assert
        var result = await _toDoService.GetTodoByIdAsync(toDo1.Id);
        Assert.True(result.IsCompleted);
        Assert.NotNull(result.CompletedAt);
    }
}
