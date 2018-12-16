using Flee.CalcEngine.PublicTypes;
using Flee.PublicTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnboardDiagnostics
{
    public class ResponseEvaluator
    {
        private readonly ExpressionContext _context = new ExpressionContext();
        private readonly IGenericExpression<double> _expression;

        string[] _letters = new string[]
        {
                "A",
                "B",
                "C",
                "D",
                "E"
        };

        public ResponseEvaluator(string expression)
        {
            _context.Imports.AddType(typeof(Math));

            foreach (var letter in _letters)
            {
                _context.Variables[letter] = default(double);
            }

            _expression = _context.CompileGeneric<double>(expression.ToUpper());
        }

        public object Evaluate(params byte[] bytes)
        {
            for (var index = 0; index < bytes.Length; index++)
            {
                var letter = _letters[index];
                var value = bytes[index];

                _context.Variables[letter] = (double)value;
            }

            return _expression.Evaluate();
        }
    }
}