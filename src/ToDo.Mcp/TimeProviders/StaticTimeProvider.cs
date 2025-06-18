namespace ToDo.Mcp.Services.TimeProviders;

public class StaticTimeProvider(DateTime value) : ITimeProvider
{
  public DateTime Current => value;
}
