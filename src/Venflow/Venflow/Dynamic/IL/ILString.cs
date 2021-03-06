﻿using System.Reflection.Emit;

namespace Venflow.Dynamic.IL
{

    internal struct ILString : IILBaseInst
    {
        private readonly OpCode _opCode;
        private readonly string _value;

        internal ILString(OpCode opCode, string value)
        {
            _opCode = opCode;
            _value = value;
        }

        public void WriteIL(ILGenerator ilGenerator)
        {
            ilGenerator.Emit(_opCode, _value);
        }
    }
}