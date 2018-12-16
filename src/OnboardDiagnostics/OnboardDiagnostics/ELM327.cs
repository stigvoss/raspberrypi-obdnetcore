using System;
using System.Linq;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            _port.WriteLine(command);

            Thread.Sleep(ExecutionGracePeriod);

            var buffer = new byte[1024];
            var readBytes = _port.Read(buffer, 0, buffer.Length);

            var content = buffer.AsSpan(0, readBytes);
            
            return new CommandResponse(content, command.Evaluator);
        }
    }
}
