using ToDo.Mcp.Services.TimeProviders;
using ToDo.Mcp.ToDoItems.Models;

namespace ToDo.Mcp.ToDoItems;

public class ToDoItemBuilder
{
    private bool _isDirty = true;
    private ITimeProvider _timeProvider = new SystemTimeProvider();
    private Guid? _id = null;
    private string _title = string.Empty;
    private string _description = string.Empty;
    private bool _isCompleted = false;
    private DateTime? _createdAt = null;
    private DateTime? _completedAt = null;

    public ToDoItemBuilder() { }

    public static ToDoItemBuilder FromToDoItem(ToDoItem toDoItem)
    {
        return new ToDoItemBuilder().WithToDoItem(toDoItem);
    }

    public ToDoItemBuilder WithToDoItem(ToDoItem toDoItem)
    {
        _id = toDoItem.Id;
        _title = toDoItem.Title;
        _description = toDoItem.Description;
        _isCompleted = toDoItem.IsCompleted;
        _createdAt = toDoItem.CreatedAt;
        _completedAt = toDoItem.CompletedAt;
        _isDirty = false;
        return this;
    }

    public ToDoItemBuilder WithId(Guid id)
    {
        if (_id != id)
        {
            _id = id;
            _isDirty = true;
        }
        return this;
    }

    public ToDoItemBuilder WithTimeProvider(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
        return this;
    }

    public ToDoItemBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public ToDoItemBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public ToDoItemBuilder WithIsCompleted(bool isCompleted)
    {
        WithCompletedAt(isCompleted ? _timeProvider.Current : null);
        return this;
    }

    public ToDoItemBuilder WithCreatedAt(DateTime? createdAt = null)
    {
        _createdAt = createdAt;
        return this;
    }

    public ToDoItemBuilder WithCompletedAt(DateTime? completedAt = null)
    {
        _completedAt = completedAt;
        _isCompleted = completedAt is not null;
        return this;
    }

    public ToDoItem Build()
    {
        SanitizeBeforeBuild();
        return new ToDoItem
        {
            Id = _id ?? Guid.CreateVersion7(),
            Title = _title,
            Description = _description,
            IsCompleted = _isCompleted,
            CreatedAt = _createdAt ?? _timeProvider.Current,
            CompletedAt = _completedAt
        };
    }

    private void SanitizeBeforeBuild()
    {
        if (_isDirty)
        {
            _id ??= Guid.CreateVersion7();
            _createdAt ??= _timeProvider.Current;
        }
    }
}
