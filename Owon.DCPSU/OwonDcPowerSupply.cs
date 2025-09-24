using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Owon.DCPSU.Transports;

namespace Owon.DCPSU
{
    /// <summary>
    /// Provides a high-level interface for controlling OWON DC Power Supply units via SCPI commands.
    /// </summary>
    public sealed class OwonDcPowerSupply
    {
        private readonly IScpiTransport _transport;

        /// <summary>
        /// Initializes a new instance of the OwonDcPowerSupply class.
        /// </summary>
        /// <param name="transport">The SCPI transport interface for communication.</param>
        public OwonDcPowerSupply(IScpiTransport transport)
        {
            _transport = transport;
        }

        /// <summary>
        /// Gets a value indicating whether the power supply is connected.
        /// </summary>
        public bool IsConnected => _transport.IsConnected;

        /// <summary>
        /// Establishes connection to the power supply.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the connection operation.</returns>
        public Task ConnectAsync(CancellationToken cancellationToken = default) => _transport.ConnectAsync(cancellationToken);

        /// <summary>
        /// Queries the instrument identification string (*IDN?).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The identification string in format: OWON,model,serial,FV:version</returns>
        public async Task<string> IdentifyAsync(CancellationToken cancellationToken = default)
        {
            return await _transport.QueryAsync("*IDN?", cancellationToken);
        }

        /// <summary>
        /// Resets the power supply to factory default settings (*RST).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the reset operation.</returns>
        public Task ResetAsync(CancellationToken cancellationToken = default)
        {
            return _transport.WriteAsync("*RST", cancellationToken);
        }

        // Output control
        /// <summary>
        /// Enables or disables the power supply output (OUTP ON/OFF).
        /// </summary>
        /// <param name="enabled">True to enable output, false to disable.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the operation.</returns>
        public Task SetOutputAsync(bool enabled, CancellationToken cancellationToken = default)
            => _transport.WriteAsync($"OUTP {(enabled ? "ON" : "OFF")}", cancellationToken);

        /// <summary>
        /// Queries the current output state (OUTP?).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if output is enabled, false if disabled.</returns>
        public async Task<bool> GetOutputAsync(CancellationToken cancellationToken = default)
        {
            var resp = await _transport.QueryAsync("OUTP?", cancellationToken);
            return NormalizeBool(resp);
        }

        // Voltage
        /// <summary>
        /// Sets the output voltage level (VOLT value).
        /// </summary>
        /// <param name="volts">Voltage value in volts.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the operation.</returns>
        public Task SetVoltageAsync(double volts, CancellationToken cancellationToken = default)
            => _transport.WriteAsync($"VOLT {FormatNumber(volts)}", cancellationToken);

        /// <summary>
        /// Queries the current voltage setting (VOLT?).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Voltage setting in volts.</returns>
        public async Task<double> GetVoltageAsync(CancellationToken cancellationToken = default)
            => ParseDouble(await _transport.QueryAsync("VOLT?", cancellationToken));

        /// <summary>
        /// Sets the overvoltage protection (OVP) limit (VOLT:LIM value).
        /// </summary>
        /// <param name="volts">OVP limit in volts.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the operation.</returns>
        public Task SetVoltageLimitAsync(double volts, CancellationToken cancellationToken = default)
            => _transport.WriteAsync($"VOLT:LIM {FormatNumber(volts)}", cancellationToken);

        /// <summary>
        /// Queries the overvoltage protection limit (VOLT:LIM?).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>OVP limit in volts.</returns>
        public async Task<double> GetVoltageLimitAsync(CancellationToken cancellationToken = default)
            => ParseDouble(await _transport.QueryAsync("VOLT:LIM?", cancellationToken));

        // Current
        /// <summary>
        /// Sets the output current level (CURR value).
        /// </summary>
        /// <param name="amps">Current value in amperes.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the operation.</returns>
        public Task SetCurrentAsync(double amps, CancellationToken cancellationToken = default)
            => _transport.WriteAsync($"CURR {FormatNumber(amps)}", cancellationToken);

        /// <summary>
        /// Queries the current current setting (CURR?).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Current setting in amperes.</returns>
        public async Task<double> GetCurrentAsync(CancellationToken cancellationToken = default)
            => ParseDouble(await _transport.QueryAsync("CURR?", cancellationToken));

        /// <summary>
        /// Sets the overcurrent protection (OCP) limit (CURR:LIM value).
        /// </summary>
        /// <param name="amps">OCP limit in amperes.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the operation.</returns>
        public Task SetCurrentLimitAsync(double amps, CancellationToken cancellationToken = default)
            => _transport.WriteAsync($"CURR:LIM {FormatNumber(amps)}", cancellationToken);

        /// <summary>
        /// Queries the overcurrent protection limit (CURR:LIM?).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>OCP limit in amperes.</returns>
        public async Task<double> GetCurrentLimitAsync(CancellationToken cancellationToken = default)
            => ParseDouble(await _transport.QueryAsync("CURR:LIM?", cancellationToken));

        // Measurements
        /// <summary>
        /// Measures the actual output voltage (MEAS:VOLT?).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Measured voltage in volts.</returns>
        public async Task<double> MeasureVoltageAsync(CancellationToken cancellationToken = default)
            => ParseDouble(await _transport.QueryAsync("MEAS:VOLT?", cancellationToken));

        /// <summary>
        /// Measures the actual output current (MEAS:CURR?).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Measured current in amperes.</returns>
        public async Task<double> MeasureCurrentAsync(CancellationToken cancellationToken = default)
            => ParseDouble(await _transport.QueryAsync("MEAS:CURR?", cancellationToken));

        /// <summary>
        /// Measures the actual output power (MEAS:POW?).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Measured power in watts.</returns>
        public async Task<double> MeasurePowerAsync(CancellationToken cancellationToken = default)
            => ParseDouble(await _transport.QueryAsync("MEAS:POW?", cancellationToken));

        /// <summary>
        /// Measures voltage, current, and power simultaneously (MEAS:ALL?).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Tuple containing (voltage, current, power).</returns>
        public async Task<(double volts, double amps, double watts)> MeasureAllAsync(CancellationToken cancellationToken = default)
        {
            var resp = await _transport.QueryAsync("MEAS:ALL?", cancellationToken);
            var parts = resp.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if(parts.Length<2) throw new InvalidOperationException("Unexpected response for MEAS:ALL?");
            if (parts.Length < 3) return (ParseDouble(parts[0]), ParseDouble(parts[1]), 0);
            return (ParseDouble(parts[0]), ParseDouble(parts[1]), ParseDouble(parts[2]));
        }

        /// <summary>
        /// Measures voltage, current, power, and status information (MEAS:ALL:INFO?).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Tuple containing (voltage, current, power, ovp_fault, ocp_fault, otp_fault, operating_mode).</returns>
        /// <remarks>
        /// Operating modes: 0=standby, 1=CV mode, 2=CC mode, 3=failure mode.
        /// </remarks>
        public async Task<(double volts, double amps, double watts, bool ovp, bool ocp, bool otp, int mode)> MeasureAllInfoAsync(CancellationToken cancellationToken = default)
        {
            var resp = await _transport.QueryAsync("MEAS:ALL:INFO?", cancellationToken);
            var parts = resp.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 7) throw new InvalidOperationException("Unexpected response for MEAS:ALL:INFO?");
            return (
                ParseDouble(parts[0]),
                ParseDouble(parts[1]),
                ParseDouble(parts[2]),
                NormalizeBool(parts[3]),
                NormalizeBool(parts[4]),
                NormalizeBool(parts[5]),
                int.Parse(parts[6], CultureInfo.InvariantCulture)
            );
        }

        // System control
        /// <summary>
        /// Sets the power supply to local operation mode (SYST:LOC).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the operation.</returns>
        public Task SetLocalAsync(CancellationToken cancellationToken = default)
            => _transport.WriteAsync("SYST:LOC", cancellationToken);

        /// <summary>
        /// Sets the power supply to remote operation mode (SYST:REM).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the operation.</returns>
        public Task SetRemoteAsync(CancellationToken cancellationToken = default)
            => _transport.WriteAsync("SYST:REM", cancellationToken);

        private static string FormatNumber(double value)
            => value.ToString("0.###", CultureInfo.InvariantCulture);

        private static double ParseDouble(string s)
            => double.Parse(s.Trim(), CultureInfo.InvariantCulture);

        private static bool NormalizeBool(string s)
        {
            var t = s.Trim().ToUpperInvariant();
            return t == "1" || t == "ON" || t == "TRUE";
        }
    }
}


