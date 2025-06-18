namespace ToDo.Mcp.Services.TimeProviders;

public class SystemTimeProvider : ITimeProvider
{
  public DateTime Current => DateTime.UtcNow;
}