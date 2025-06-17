using ToDo.Mcp.Entities;

namespace ToDo.Mcp.Services;

public interface IToDoService
{
  Task<ToDoItem> CreateTodoAsync(ToDoItem todo);
  Task<ToDoItem> DeleteTodoAsync(Guid id);
  Task<IEnumerable<ToDoItem>> GetToDosAsync();
  Task<ToDoItem> GetTodoByIdAsync(Guid id);
  Task<ToDoItem> UpdateTodoAsync(ToDoItem todo);
}

public class ToDoService : IToDoService
{
  private static readonly ToDoItem[] initialTodos = [
      new ToDoItem { Id = Guid.NewGuid(), Title = "Buy groceries", Description = "Buy groceries", IsCompleted = false }
  ];

  private readonly List<ToDoItem> _todoList = [.. initialTodos];

  public ToDoService() { }

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
    _todoList.Add(todo);
    return Task.FromResult(todo);
  }

  public async Task<ToDoItem> UpdateTodoAsync(ToDoItem todo)
  {
    var existingTodo = await GetTodoByIdAsync(todo.Id);
    existingTodo.Title = todo.Title;
    existingTodo.Description = todo.Description;
    existingTodo.IsCompleted = todo.IsCompleted;
    return existingTodo;
  }

  public async Task<ToDoItem> DeleteTodoAsync(Guid id)
  {
    var todo = await GetTodoByIdAsync(id);
    _todoList.Remove(todo);
    return todo;
  }
}
