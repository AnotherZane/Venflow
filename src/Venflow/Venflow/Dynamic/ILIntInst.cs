﻿using Npgsql.Schema;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Venflow.Modeling;
using Venflow.Enums;

namespace Venflow.Dynamic
{

    internal struct ILIntInst : IILBaseInst
    {
        private readonly OpCode _opCode;
        private readonly int _value;

        internal ILIntInst(OpCode opCode, int value)
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