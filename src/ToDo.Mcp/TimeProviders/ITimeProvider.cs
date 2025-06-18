namespace ToDo.Mcp.Services.TimeProviders;

public interface ITimeProvider
{
  DateTime Current { get; }
}
