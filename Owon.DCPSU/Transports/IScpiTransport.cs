using System;
using System.Threading;
using System.Threading.Tasks;

namespace Owon.DCPSU.Transports
{
    public interface IScpiTransport : IAsyncDisposable, IDisposable
    {
        Task ConnectAsync(CancellationToken cancellationToken = default);
        Task<string> QueryAsync(string command, CancellationToken cancellationToken = default);
        Task WriteAsync(string command, CancellationToken cancellationToken = default);
        bool IsConnected { get; }
    }
}


