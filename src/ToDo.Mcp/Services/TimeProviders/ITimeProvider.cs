namespace ToDo.Mcp.Services.TimeProviders;

public interface ITimeProvider
{
  DateTime Current { get; }
}

public class StaticTimeProvider(DateTime value) : ITimeProvider
{
  public DateTime Current => value;
}

public class SystemTimeProvider : ITimeProvider
{
  public DateTime Current => DateTime.UtcNow;
}