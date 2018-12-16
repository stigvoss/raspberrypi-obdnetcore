using System;

namespace OnboardDiagnostics
{
    public class ATCommand
    {
        public static readonly ATCommand ResetDevice = new ATCommand("ATZ");
        public static readonly ATCommand DisableEcho = new ATCommand("ATE0");
        public static readonly ATCommand DisableHeaders = new ATCommand("ATH0");
        public static readonly ATCommand DisableSpaces = new ATCommand("ATS0");
        public static readonly ATCommand AutoDetectProtocol = new ATCommand("ATSP0");

        public static readonly ATCommand Speed = new ATCommand(Mode.Current, PID.Speed);
        public static readonly ATCommand Rpm = new ATCommand(Mode.Current, PID.Rpm, "(a * 256 + b) / 4");
        public static readonly ATCommand EngineCoolantTemperature = new ATCommand(Mode.Current, PID.EngineCoolantTemperature, "a - 40");
        public static readonly ATCommand EngineLoad = new ATCommand(Mode.Current, PID.EngineLoad, "100 / 255 * a");
        public static readonly ATCommand FuelPressure = new ATCommand(Mode.Current, PID.FuelPressure, "3 * a");
        public static readonly ATCommand IntakeManifoldPressure = new ATCommand(Mode.Current, PID.IntakeManifoldPressure);
        public static readonly ATCommand TimingAdvance = new ATCommand(Mode.Current, PID.TimingAdvance, "a / 2 - 64");
        public static readonly ATCommand IntakeAirTemperature = new ATCommand(Mode.Current, PID.IntakeAirTemperature, "a - 40");
        public static readonly ATCommand ThrottlePosition = new ATCommand(Mode.Current, PID.ThrottlePosition, "a / 255 * 100");
        public static readonly ATCommand EngineRunTime = new ATCommand(Mode.Current, PID.EngineRunTime, "256 * a + b");
        public static readonly ATCommand FuelTankLevel = new ATCommand(Mode.Current, PID.FuelTankLevel, "100 / 255 * a");
        public static readonly ATCommand EngineOilTemperature = new ATCommand(Mode.Current, PID.EngineOilTemperature, "a - 40");
        public static readonly ATCommand MassAirFlowRate = new ATCommand(Mode.Current, PID.MassAirFlowRate, "(256 * a + b) / 100");
        public string CommandText { get; }

        public  ResponseEvaluator Evaluator { get; }

        public enum Mode
        {
            Current = 0x01
        }

        public enum PID
        {
            Rpm = 0x0C,
            Speed = 0x0D,
            EngineCoolantTemperature = 0x05,
            EngineLoad = 0x04,
            FuelPressure = 0x0A,
            IntakeManifoldPressure = 0x0B,
            TimingAdvance = 0x0E,
            IntakeAirTemperature = 0x0F,
            ThrottlePosition = 0x11,
            EngineRunTime = 0x1F,
            FuelTankLevel = 0x2F,
            EngineOilTemperature = 0x5C,
            MassAirFlowRate = 0x10
        }

        public ATCommand(string command, string equation = null)
        {
            CommandText = command;
            Evaluator = equation is object ? new ResponseEvaluator(equation) : null;
        }

        public ATCommand(PID id, string equation = null)
            : this(Mode.Current, id, equation)
        { }

        public ATCommand(Mode mode, PID id, string equation = null)
        {
            var encodedMode = Convert.ToUInt32(mode).ToString("X2");
            var encodedId = Convert.ToUInt32(id).ToString("X2");

            CommandText = $"{encodedMode}{encodedId}";
            Evaluator = equation is object ? new ResponseEvaluator(equation) : null;
        }

        public override string ToString()
        {
            return CommandText;
        }

        public static implicit operator string(ATCommand command)
        {
            return command.CommandText;
        }
    }
}
