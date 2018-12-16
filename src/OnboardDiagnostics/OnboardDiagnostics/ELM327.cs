using System;
using System.Linq;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OnboardDiagnostics
{
    public class ELM327
    {
        private const int ExecutionGracePeriod = 100;

        private const char ResponseTermination = '>';

        private readonly SerialPort _port;

        public ELM327(string portName)
        {
            _port = new SerialPort(portName, 38400)
            {
                NewLine = "\r",
                ReadTimeout = 1000,
                WriteTimeout = 1000
            };
        }

        public void Initialize()
        {
            if (!_port.IsOpen)
            {
                _port.Open();
            }

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

        public CommandResponse ExecuteCommand(ATCommand command)
        {
            var builder = new StringBuilder();

            if (!_port.IsOpen)
            {
                _port.Open();
            }

            try
            {
                _port.WriteLine(command);
            }
            catch (TimeoutException)
            {
                return new CommandResponse(string.Empty, command.Evaluator);
            }

            Thread.Sleep(ExecutionGracePeriod);

            try
            {
                int readCharacter;
                while ((readCharacter = _port.ReadChar()) != default)
                {
                    if (readCharacter == ResponseTermination)
                    {
                        break;
                    }

                    builder.Append((char)readCharacter);
                }

                var content = builder.ToString();

                return new CommandResponse(content, command.Evaluator);
            }
            catch (TimeoutException)
            {
                return new CommandResponse(string.Empty, command.Evaluator);
            }
        }
    }
}
