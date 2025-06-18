namespace ToDo.Mcp.ToDoItems.Models;

public record ToDoItem(Guid Id, string Title, string Description, ToDoItemStatus Status, DateTime CreatedAt)
{
    public bool IsCompleted => Status.IsCompleted;
};

public record ToDoItemStatus(bool IsCompleted, DateTime? CompletedAt);
