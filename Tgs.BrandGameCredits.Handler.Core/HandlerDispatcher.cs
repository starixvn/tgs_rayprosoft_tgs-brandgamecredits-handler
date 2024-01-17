using System.Collections.Concurrent;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client;
using Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

namespace Tgs.BrandGameCredits.Handler.Core;

internal class HandlerDispatcher(IServiceProvider serviceProvider) : IHandlerDispatcher, IDisposable, IAsyncDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly ConcurrentDictionary<string, HandlerChannel> _counters = new();
    private bool _disposedValue;

    ~HandlerDispatcher()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        Dispose(true);
        GC.SuppressFinalize(this);

        return ValueTask.CompletedTask;
    }

    public async ValueTask PushCommandAsync(
        DispatcherCommand command,
        CancellationToken cancellationToken = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);

        //var key = string.Join('-', new string[] { command.ResellerCode, command.LicenseeCode, command.BrandCode });
        var key = typeof(HandlerDispatcher).FullName!;
        if (!_counters.TryGetValue(key, out HandlerChannel? channel))
        {
            await InitAsync(
                key: key,
                cancellationToken: cts.Token)
                .ConfigureAwait(false);

            channel = _counters.GetValueOrDefault(key)!;
        }
        await channel.PushCommandAsync(command, cts.Token).ConfigureAwait(false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _cancellationTokenSource.Cancel();
            }

            _cancellationTokenSource.Dispose();
            _disposedValue = true;
        }
    }

    private ValueTask InitAsync(
                string key,
        CancellationToken cancellationToken = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);
        var counter = _counters.GetOrAdd(
        key: key,
        valueFactory: key =>
        {
            var scope = serviceProvider.CreateScope();
            var newCounter = ActivatorUtilities.CreateInstance<HandlerChannel>(
            scope.ServiceProvider,
            key);
            newCounter.Disposed += (_, _) =>
            {
                _ = _counters.TryRemove(newCounter.Key, out _);
                scope.Dispose();
            };

            return newCounter;
        });

        return counter.InitAsync(cts.Token);
    }
}
