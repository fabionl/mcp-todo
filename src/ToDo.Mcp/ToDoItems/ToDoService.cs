using ToDo.Mcp.Services.TimeProviders;
using ToDo.Mcp.ToDoItems.Models;

namespace ToDo.Mcp.ToDoItems;

public interface IToDoService
{
  Task<ToDoItem> MarkAsCompletedAsync(Guid id);
  Task<ToDoItem> CreateTodoAsync(ToDoItem todo);
  Task<ToDoItem> DeleteTodoAsync(Guid id);
  Task<IEnumerable<ToDoItem>> GetToDosAsync();
  Task<ToDoItem> GetTodoByIdAsync(Guid id);
  Task<ToDoItem> UpdateTodoAsync(ToDoItem todo);
}

public class ToDoService : IToDoService
{
  private readonly ITimeProvider _timeProvider;

  private static readonly ToDoItem[] initialTodos = [
      new ToDoItem { Id = Guid.NewGuid(), Title = "Buy groceries", Description = "Buy groceries", IsCompleted = false }
  ];

  private readonly List<ToDoItem> _todoList = [.. initialTodos];

  public ToDoService(ITimeProvider timeProvider)
  {
    _timeProvider = timeProvider;
  }

  public Task<IEnumerable<ToDoItem>> GetToDosAsync()
  {
    return Task.FromResult(_todoList.AsEnumerable());
  }

  public Task<ToDoItem> GetTodoByIdAsync(Guid id)
  {
    var todo = _todoList
      .FirstOrDefault(todo => todo.Id == id)
        ?? throw new Exception("Todo not found");
    return Task.FromResult(todo);
  }

  public Task<ToDoItem> CreateTodoAsync(ToDoItem todo)
  {
    _todoList.Add(SanitizeTodo(todo));
    return Task.FromResult(todo);
  }

  public async Task<ToDoItem> UpdateTodoAsync(ToDoItem todo)
  {
    var existingTodo = await GetTodoByIdAsync(todo.Id);
    var todoBuilder = ToDoItemBuilder.FromToDoItem(existingTodo)
      .WithTitle(todo.Title)
      .WithDescription(todo.Description)
      .WithIsCompleted(todo.IsCompleted);

    var updatedTodo = todoBuilder.Build();

    _todoList.Remove(existingTodo);
    _todoList.Add(updatedTodo);
    return updatedTodo;
  }

  public async Task<ToDoItem> DeleteTodoAsync(Guid id)
  {
    var todo = await GetTodoByIdAsync(id);
    _todoList.Remove(todo);
    return todo;
  }

  public async Task<ToDoItem> MarkAsCompletedAsync(Guid id)
  {
    var todo = await GetTodoByIdAsync(id);
    var todoBuilder = ToDoItemBuilder.FromToDoItem(todo)
      .WithIsCompleted(true);
    return await UpdateTodoAsync(todoBuilder.Build());
  }

  private ToDoItem SanitizeTodo(ToDoItem todo)
  {
    todo.CreatedAt = todo.CreatedAt == default ? _timeProvider.Current : todo.CreatedAt;
    return todo;
  }


}
