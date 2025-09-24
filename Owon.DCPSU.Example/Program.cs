using System;
using System.Threading.Tasks;
using Owon.DCPSU;
using Owon.DCPSU.Transports;

namespace Owon.DCPSU.Example
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("OWON DC PSU example");

            using var transport = new SerialScpiTransport(portName: "COM3", baudRate: 115200);
            var psu = new OwonDcPowerSupply(transport);

            await psu.ConnectAsync();


            var id = await psu.IdentifyAsync();
            Console.WriteLine($"*IDN? => {id}");

            await psu.SetRemoteAsync();
            await psu.SetVoltageAsync(20.0);
            await psu.SetCurrentAsync(7);
            await psu.SetOutputAsync(true);
            await Task.Delay(3000);

            //При работе в режиме постоянного тока, обратить внимание на максимальную мощность источника
            var (v, a, w) = await psu.MeasureAllAsync();
            Console.WriteLine($"Measure: {v} V, {a} A, {w} W");


            await psu.SetOutputAsync(false);
            await psu.SetLocalAsync();
        }
    }
}
