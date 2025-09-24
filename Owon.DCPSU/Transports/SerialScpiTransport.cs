using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Owon.DCPSU.Transports
{
    public sealed class SerialScpiTransport : IScpiTransport
    {
        private readonly string _portName;
        private readonly int _baudRate;
        private readonly Parity _parity;
        private readonly int _dataBits;
        private readonly StopBits _stopBits;
        private SerialPort? _port;
        private readonly Encoding _encoding = Encoding.ASCII;

        public SerialScpiTransport(
            string portName,
            int baudRate = 115200,
            Parity parity = Parity.None,
            int dataBits = 8,
            StopBits stopBits = StopBits.One)
        {
            _portName = portName;
            _baudRate = baudRate;
            _parity = parity;
            _dataBits = dataBits;
            _stopBits = stopBits;
        }

        public bool IsConnected => _port != null && _port.IsOpen;

        public Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            _port = new SerialPort(_portName, _baudRate, _parity, _dataBits, _stopBits)
            {
                NewLine = "\n",
                ReadTimeout = 5000,
                WriteTimeout = 5000,
                Encoding = _encoding
            };
            _port.Open();
            return Task.CompletedTask;
        }

        public async Task<string> QueryAsync(string command, CancellationToken cancellationToken = default)
        {
            await WriteAsync(command, cancellationToken);
            return await ReadLineAsync(cancellationToken);
        }

        public Task WriteAsync(string command, CancellationToken cancellationToken = default)
        {
            if (!IsConnected || _port == null)
            {
                throw new InvalidOperationException("Transport is not connected.");
            }
            _port.WriteLine(command.Trim());
            return Task.CompletedTask;
        }

        private Task<string> ReadLineAsync(CancellationToken cancellationToken)
        {
            if (!IsConnected || _port == null)
            {
                throw new InvalidOperationException("Transport is not connected.");
            }
            string line = _port.ReadLine();
            return Task.FromResult(line.Trim());
        }

        public void Dispose()
        {
            if (_port != null)
            {
                try { _port.Dispose(); } catch { }
            }
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }
    }
}


