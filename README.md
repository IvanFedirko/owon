# OWON DC Power Supply Control Library

A .NET library for controlling OWON DC Power Supply units via SCPI commands over serial connections.

## Features

- **High-level API** for controlling OWON DC power supplies
- **Serial communication**: RS-232/RS-485 support
- **Async/await support** for all operations
- **Comprehensive SCPI command coverage**:
  - Output control (enable/disable)
  - Voltage and current setting/measurement
  - Overvoltage (OVP) and overcurrent (OCP) protection
  - Power measurement
  - System control (local/remote mode)
  - Instrument identification

## Supported Operations

### Output Control
- Enable/disable power supply output
- Query output status

### Voltage Control
- Set output voltage
- Query voltage setting
- Set/query overvoltage protection (OVP) limit
- Measure actual output voltage

### Current Control
- Set output current
- Query current setting
- Set/query overcurrent protection (OCP) limit
- Measure actual output current

### Measurements
- Measure voltage, current, and power simultaneously
- Extended measurements with fault status and operating mode
- Individual voltage, current, and power measurements

### System Control
- Instrument identification (*IDN?)
- Reset to factory defaults (*RST)
- Switch between local and remote operation modes

## Installation

### NuGet Package
```bash
dotnet add package Owon.DCPSU
```

### From Source
```bash
git clone https://github.com/your-repo/owon-dcpsu.git
cd owon-dcpsu
dotnet build
```

## Quick Start

```csharp
using Owon.DCPSU;
using Owon.DCPSU.Transports;

// Create serial transport
using var transport = new SerialScpiTransport(portName: "COM3", baudRate: 115200);
var psu = new OwonDcPowerSupply(transport);

// Connect and control
await psu.ConnectAsync();
var id = await psu.IdentifyAsync();
Console.WriteLine($"Device: {id}");

// Set voltage and current
await psu.SetRemoteAsync();
await psu.SetVoltageAsync(12.0);  // 12V
await psu.SetCurrentAsync(2.0);    // 2A
await psu.SetOutputAsync(true);    // Enable output

// Measure output
var (voltage, current, power) = await psu.MeasureAllAsync();
Console.WriteLine($"Output: {voltage}V, {current}A, {power}W");

// Disable output and return to local mode
await psu.SetOutputAsync(false);
await psu.SetLocalAsync();
```

## Transport Configuration

```csharp
var transport = new SerialScpiTransport(
    portName: "COM3",           // Serial port name
    baudRate: 115200,           // Baud rate (default: 115200)
    parity: Parity.None,        // Parity (default: None)
    dataBits: 8,                // Data bits (default: 8)
    stopBits: StopBits.One      // Stop bits (default: One)
);
```

## API Reference

### Core Methods

| Method | Description | Parameters |
|--------|-------------|------------|
| `ConnectAsync()` | Establish connection to power supply | `CancellationToken` |
| `IdentifyAsync()` | Get instrument identification | `CancellationToken` |
| `ResetAsync()` | Reset to factory defaults | `CancellationToken` |

### Output Control

| Method | Description | Parameters |
|--------|-------------|------------|
| `SetOutputAsync(bool)` | Enable/disable output | `enabled`, `CancellationToken` |
| `GetOutputAsync()` | Query output status | `CancellationToken` |

### Voltage Control

| Method | Description | Parameters |
|--------|-------------|------------|
| `SetVoltageAsync(double)` | Set output voltage | `volts`, `CancellationToken` |
| `GetVoltageAsync()` | Query voltage setting | `CancellationToken` |
| `SetVoltageLimitAsync(double)` | Set OVP limit | `volts`, `CancellationToken` |
| `GetVoltageLimitAsync()` | Query OVP limit | `CancellationToken` |
| `MeasureVoltageAsync()` | Measure actual voltage | `CancellationToken` |

### Current Control

| Method | Description | Parameters |
|--------|-------------|------------|
| `SetCurrentAsync(double)` | Set output current | `amps`, `CancellationToken` |
| `GetCurrentAsync()` | Query current setting | `CancellationToken` |
| `SetCurrentLimitAsync(double)` | Set OCP limit | `amps`, `CancellationToken` |
| `GetCurrentLimitAsync()` | Query OCP limit | `CancellationToken` |
| `MeasureCurrentAsync()` | Measure actual current | `CancellationToken` |

### Measurements

| Method | Description | Returns |
|--------|-------------|---------|
| `MeasurePowerAsync()` | Measure output power | `double` (watts) |
| `MeasureAllAsync()` | Measure V, I, P simultaneously | `(volts, amps, watts)` |
| `MeasureAllInfoAsync()` | Extended measurement with status | `(volts, amps, watts, ovp, ocp, otp, mode)` |

### System Control

| Method | Description | Parameters |
|--------|-------------|------------|
| `SetLocalAsync()` | Switch to local mode | `CancellationToken` |
| `SetRemoteAsync()` | Switch to remote mode | `CancellationToken` |

## Operating Modes

The power supply operates in different modes:

- **0**: Standby mode
- **1**: Constant Voltage (CV) mode
- **2**: Constant Current (CC) mode  
- **3**: Failure mode

Use `MeasureAllInfoAsync()` to query the current operating mode and fault status.

## Requirements

- .NET 8.0 or later
- For serial communication: `System.IO.Ports` package (included)
- Compatible OWON DC power supply with SCPI support

## Example Project

The repository includes a complete example project (`Owon.DCPSU.Example`) demonstrating:

- Serial connection setup
- Basic power supply control
- Voltage and current setting
- Output measurement
- Proper cleanup and local mode restoration

Run the example:
```bash
cd Owon.DCPSU.Example
dotnet run
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Support

For issues and questions, please open an issue on the GitHub repository.
