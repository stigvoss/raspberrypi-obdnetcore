using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace OnboardDiagnostics
{
    class Program
    {
        static void Main(string[] args)
        {
            var portName = args.FirstOrDefault(e => e.StartsWith("--port-name=") || e.StartsWith("-p="))?.Split('=')[1];
            var output = args.FirstOrDefault(e => e.StartsWith("--output-file=") || e.StartsWith("-o="))?.Split('=')[1];

            var logFile = new FileInfo(output);

            var obd = new ELM327(portName);

            Console.WriteLine("Initializing...");

            obd.Initialize();
            
            var commands = new ATCommand[]
            {
                ATCommand.Speed,
                ATCommand.Rpm,
                ATCommand.EngineLoad,
                ATCommand.EngineRunTime,
                ATCommand.IntakeManifoldPressure,
                ATCommand.IntakeAirTemperature,
                ATCommand.EngineCoolantTemperature,
                ATCommand.MassAirFlowRate
            };


            Console.WriteLine("Scanning...");

            while (true)
            {
                using (var writer = File.AppendText(logFile.FullName))
                {
                    writer.WriteLine($"SCAN {DateTime.Now.ToString("o")}");

                    foreach (var command in commands)
                    {
                        var response = obd.ExecuteCommand(command);

                        if(response.Type == CommandResponseType.Bytes)
                        {
                            writer.WriteLine($"{command.CommandText} {response.Value()}");
                        }
                    }
                }

                Thread.Sleep(1000);
            }
        }
    }
}
