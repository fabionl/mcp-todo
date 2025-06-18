using System.Data.Common;
using System.Diagnostics;

using ToDo.Mcp.Services.TimeProviders;
using ToDo.Mcp.ToDoItems;
using ToDo.Mcp.ToDoItems.Models;

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

    // Act
    await _toDoService.CreateTodoAsync(toDo1);

    // Assert
    var result = await _toDoService.GetToDosAsync();
    Assert.Single(result);
  }

  [Fact]
  public async Task UpdateToDoAsync_UpdatesToDo()
  {
    // Arrange
    var newTitle = "Updated ToDo 1";
    var toDo1 = _toDoBuilder.WithTitle("ToDo 1")
      .WithDescription("Description 1")
      .Build();
    var createdToDo = await _toDoService.CreateTodoAsync(toDo1);
    Assert.Equal(toDo1, createdToDo);

    // Act
    await _toDoService.UpdateTodoAsync(toDo1.Id, newTitle, toDo1.Description);

    // Assert
    var result = await _toDoService.GetTodoByIdAsync(toDo1.Id);
    Assert.Multiple(
      () => Assert.Equal(newTitle, result.Title),
      () => Assert.Equal(toDo1.Description, result.Description),
      () => Assert.Equal(toDo1.IsCompleted, result.IsCompleted),
      () => Assert.Equal(toDo1.CreatedAt, result.CreatedAt),
      () => Assert.Equal(toDo1.Status, result.Status),
      () => Assert.Equal(toDo1.Status.CompletedAt, result.Status.CompletedAt)
    );
  }

  [Fact]
  public async Task DeleteToDoAsync_DeletesToDo()
  {
    // Arrange
    var title = "ToDo 1";
    var description = "Description 1";
    var todo = await _toDoService.CreateTodoAsync(title, description);

    // Act
    await _toDoService.DeleteTodoAsync(todo.Id);

    // Assert
    var result = await _toDoService.GetToDosAsync();
    Assert.Empty(result);
  }

  [Fact]
  public async Task MarkAsCompleted_MarksToDoAsCompletedAsync()
  {
    // Arrange
    var expectedStatus = new ToDoItemStatus(IsCompleted: true, CompletedAt: _timeProvider.Current);
    var toDo1 = _toDoBuilder.WithTitle("ToDo 1")
      .WithDescription("Description 1")
      .Build();
    await _toDoService.CreateTodoAsync(toDo1);

    // Act
    await _toDoService.MarkAsCompletedAsync(toDo1.Id);

    // Assert
    var result = await _toDoService.GetTodoByIdAsync(toDo1.Id);
    Assert.True(result.IsCompleted);
    Assert.Equal(expectedStatus, result.Status);
    Assert.NotNull(result.Status.CompletedAt);
  }
}
