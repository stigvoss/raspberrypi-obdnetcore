using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OnboardDiagnostics
{
    public class CommandResponse
    {
        private readonly string _content;
        private readonly ResponseEvaluator _evaluator;

        public CommandResponse(string content, ResponseEvaluator evaluator)
        {
            _content = content.TrimEnd('>').Trim();
            _evaluator = evaluator;
        }

        public CommandResponse(byte[] content, ResponseEvaluator evaluator)
            : this(Encoding.Default.GetString(content), evaluator) { }

        public CommandResponse(Span<byte> content, ResponseEvaluator evaluator)
            : this(Encoding.Default.GetString(content), evaluator) { }

        public CommandResponseType Type
        {
            get
            {
                if (_content.Contains("OK"))
                {
                    return CommandResponseType.OK;
                }
                else if (Regex.IsMatch(_content, "^[0-9A-Z]{2}(?: [A-Z0-9]{2})+$", RegexOptions.IgnoreCase | RegexOptions.Compiled))
                {
                    return CommandResponseType.Bytes;
                }

                return CommandResponseType.UnknownCommand;
            }
        }

        public byte[] AsBytes()
        {
            if(Type != CommandResponseType.Bytes)
            {
                return default;
            }

            return _content.Split(' ').Select(e => Convert.ToByte(e, 16)).ToArray();
        }

        public object Value()
        {
            if(Type != CommandResponseType.Bytes)
            {
                return default;
            }

            var bytes = AsBytes();
            var content = bytes.AsSpan(2, bytes.Length - 2).ToArray();

            if(content.Length == 0)
            {
                return new Exception();
            }

            if(_evaluator is null)
            {
                return (int)content[0];
            }

            return _evaluator.Evaluate(content);
        }

        public static implicit operator byte[] (CommandResponse response)
        {
            return response.AsBytes();
        }
    }
}
