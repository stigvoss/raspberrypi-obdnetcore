using System;
using System.Linq;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace OnboardDiagnostics
{
    public class ELM327 : IDisposable
    {
        private const int ExecutionGracePeriod = 100;

        private const char ResponseTermination = '>';

        private readonly SerialPort _port;

        private readonly MemoryStream _serialStream = new MemoryStream();

        public ELM327(string portName)
        {
            _port = new SerialPort(portName)
            {
                NewLine = "\r"
            };
        }

        public void Initialize()
        {
            EnsureOpenConection();

            ClearPreviousResponses();

            ExecuteCommand(ATCommand.ResetDevice);

            if (ExecuteCommand(ATCommand.DisableEcho).Type != CommandResponseType.OK)
            {
                throw new Exception("Failed to disable echo.");
            }

            if (ExecuteCommand(ATCommand.DisableHeaders).Type != CommandResponseType.OK)
            {
                throw new Exception("Failed to failed to disable spaces.");
            }

            if (ExecuteCommand(ATCommand.AutoDetectProtocol).Type != CommandResponseType.OK)
            {
                throw new Exception("Failed to set auto mode.");
            }
        }

        private void EnsureOpenConection()
        {
            if (!_port.IsOpen)
            {
                _port.Open();

                if (!_port.IsOpen)
                {
                    throw new Exception();
                }
            }
        }

        private void ClearPreviousResponses()
        {
            _port.ReadExisting();
        }

        public CommandResponse ExecuteCommand(ATCommand command)
        {
            var builder = new StringBuilder();

            _port.WriteLine(command);

            Thread.Sleep(ExecutionGracePeriod);

            var character = default(int);
            while ((character = _port.ReadChar()) != ResponseTermination)
            {
                builder.Append((char)character);
            }

            var content = builder.ToString();

            return new CommandResponse(content, command.Evaluator);
        }

        public void Dispose()
        {
            ExecuteCommand(ATCommand.CloseProtocol);
            _port.Dispose();
        }
    }
}
