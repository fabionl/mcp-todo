namespace ToDo.Mcp.Services.Validators;

public interface IValidatable<T>
{
    ValidationResult Validate(T item);
}

public record ValidationResult(bool IsValid, ValidationError[] Errors)
{
    public static ValidationResult Success() => new(true, []);
    public static ValidationResult Failure(ValidationError[] errors) => new(false, errors);
}

public record ValidationError(string PropertyName, string Message);