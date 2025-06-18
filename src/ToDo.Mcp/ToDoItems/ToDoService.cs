using ToDo.Mcp.Services.TimeProviders;
using ToDo.Mcp.ToDoItems.Models;

namespace ToDo.Mcp.ToDoItems;

public interface IToDoService
{
  Task<ToDoItem> MarkAsCompletedAsync(Guid id);
  Task<ToDoItem> CreateTodoAsync(string title, string description);
  Task<ToDoItem> CreateTodoAsync(ToDoItem todo);
  Task<ToDoItem> DeleteTodoAsync(Guid id);
  Task<IEnumerable<ToDoItem>> GetToDosAsync();
  Task<ToDoItem> GetTodoByIdAsync(Guid id);
  Task<ToDoItem> UpdateTodoAsync(Guid id, string title, string description);
  Task<ToDoItem> UpdateTodoAsync(ToDoItem todo);
}

public class ToDoService : IToDoService
{
  private readonly ITimeProvider _timeProvider;

  private readonly Dictionary<Guid, ToDoItem> _todoMap = new();

  public ToDoService(ITimeProvider timeProvider)
  {
    _timeProvider = timeProvider;
  }

  public Task<IEnumerable<ToDoItem>> GetToDosAsync()
  {
    return Task.FromResult(_todoMap.Values.AsEnumerable());
  }

  public Task<ToDoItem> GetTodoByIdAsync(Guid id)
  {
    var todo = _todoMap.GetValueOrDefault(id) ?? throw new Exception("Todo not found");

    return Task.FromResult(todo);
  }

  public async Task<ToDoItem> CreateTodoAsync(string title, string description)
  {
    var todo = GetToDoItemBuilder()
      .WithTitle(title)
      .WithDescription(description)
      .Build();
    return await CreateTodoAsync(todo);
  }

  public Task<ToDoItem> CreateTodoAsync(ToDoItem todo)
  {
    if (_todoMap.ContainsKey(todo.Id))
    {
      throw new Exception("Todo already exists");
    }

    _todoMap.Add(todo.Id, todo);
    return Task.FromResult(todo);
  }

  public async Task<ToDoItem> UpdateTodoAsync(ToDoItem todo)
  {
    var existingTodo = await GetTodoByIdAsync(todo.Id);
    _todoMap.Remove(existingTodo.Id);
    _todoMap.Add(todo.Id, todo);
    return todo;
  }

  public async Task<ToDoItem> UpdateTodoAsync(Guid id, string title, string description)
  {
    var existingTodo = await GetTodoByIdAsync(id);
    var todoBuilder = GetToDoItemBuilder(existingTodo)
      .WithTitle(title)
      .WithDescription(description);

    return await UpdateTodoAsync(todoBuilder.Build());
  }

  public async Task<ToDoItem> DeleteTodoAsync(Guid id)
  {
    var todo = await GetTodoByIdAsync(id);
    _todoMap.Remove(todo.Id);

    return todo;
  }

  public async Task<ToDoItem> MarkAsCompletedAsync(Guid id)
  {
    var todo = await GetTodoByIdAsync(id);
    var todoBuilder = GetToDoItemBuilder(todo)
      .WithIsCompleted(true);

    return await UpdateTodoAsync(todoBuilder.Build());
  }

  private ToDoItemBuilder GetToDoItemBuilder(ToDoItem todo)
  {
    return ToDoItemBuilder.FromToDoItem(todo)
      .WithTimeProvider(_timeProvider);
  }

  private ToDoItemBuilder GetToDoItemBuilder()
  {
    return new ToDoItemBuilder()
      .WithTimeProvider(_timeProvider);
  }
}
