using System.IO.Pipelines;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace ToDo.Tests.Managers;

public class McpClientManager : IAsyncDisposable
{
    private readonly Pipe _clientToServerPipe = new();
    private readonly Pipe _serverToClientPipe = new();
    private readonly ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;
    private readonly ILoggerProvider _loggerProvider = NullLoggerProvider.Instance;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private CancellationToken _cancellationToken => _cancellationTokenSource.Token;

    protected IHost Server { get; private set; }
    private Task _serverTask = Task.CompletedTask;
    private bool _serverStarted = false;
    private readonly SemaphoreSlim _serverStartSemaphore = new SemaphoreSlim(1, 1);

    public McpClientManager(IHost server) : this(server, new CancellationTokenSource()) { }

    public McpClientManager(IHost server, CancellationTokenSource cancellationTokenSource)
    {
        Server = server;
        _cancellationTokenSource = cancellationTokenSource;
    }

    public async Task<IMcpClient> CreateMcpClient()
    {
        await EnsureServerStartedAsync();

        var clientTransport = new StreamClientTransport(
            serverInput: _clientToServerPipe.Writer.AsStream(),
            _serverToClientPipe.Reader.AsStream(),
            _loggerFactory
        );

        return await McpClientFactory.CreateAsync(
            clientTransport: clientTransport,
            loggerFactory: _loggerFactory,
            cancellationToken: _cancellationToken);
    }

    private async Task EnsureServerStartedAsync()
    {
        await _serverStartSemaphore.WaitAsync(_cancellationToken);
        try
        {
            if (_serverStarted)
            {
                return;
            }

            if (_serverTask.IsFaulted)
            {
                throw _serverTask.Exception ?? new Exception("Server task failed");
            }
            else if (_serverTask.IsCanceled)
            {
                throw new Exception("Server task was cancelled");
            }
            else if (_serverTask.IsCompletedSuccessfully)
            {
                _serverTask = Server.RunAsync(_cancellationToken);
                
                // Give the server a moment to initialize
                await Task.Delay(500, _cancellationToken);
                _serverStarted = true;
            }
        }
        finally
        {
            _serverStartSemaphore.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _cancellationTokenSource.CancelAsync();

        _clientToServerPipe.Writer.Complete();
        _serverToClientPipe.Reader.Complete();

        try
        {
            if (!_serverTask.IsCompleted)
            {
                await _serverTask.WaitAsync(TimeSpan.FromSeconds(5), _cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected if cancellation is requested
        }

        _serverStartSemaphore.Dispose();
        _loggerProvider.Dispose();
        _loggerFactory.Dispose();
    }
}
