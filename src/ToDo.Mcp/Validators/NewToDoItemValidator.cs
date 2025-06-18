using ToDo.Mcp.ToDoItems.Models;
using ToDo.Mcp.Services.Validators;

namespace ToDo.Mcp.Validators;

public class NewToDoItemValidator : IValidatable<ToDoItem>
{
    public ValidationResult Validate(ToDoItem item)
    {
        var errors = new List<ValidationError>();
        if (string.IsNullOrEmpty(item.Title))
        {
          errors.Add(new ValidationError("Title", "Title is required"));
        }

        return errors.Count == 0
            ? ValidationResult.Success()
            : ValidationResult.Failure(errors.ToArray());
    }
}
