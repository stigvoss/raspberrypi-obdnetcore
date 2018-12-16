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
        private readonly SerialPort _port;

        public ELM327(string portName)
        {
            _port = new SerialPort(portName, 38400)
            {
                NewLine = "\r"
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
                Debug.WriteLine($"Opening port...");

                _port.Open();

                Debug.WriteLine($"Port opened.");
            }

            Debug.WriteLine($"Command To Execute: {command.CommandText}");

            _port.WriteLine(command);

            Thread.Sleep(ExecutionGracePeriod);

            char readCharacter;
            while ((readCharacter = (char)_port.ReadChar()) != default(char))
            {
                if(readCharacter == '>')
                {
                    break;
                }

                builder.Append(readCharacter);
            }

            var content = builder.ToString();

            Debug.WriteLine($"Command Response: {content}");

            return new CommandResponse(content, command.Evaluator);
        }
    }
}
