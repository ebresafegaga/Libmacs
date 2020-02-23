using System;
using System.Collections.Generic;
using System.Text;

namespace MathAPI.MathTypes
{
    public readonly struct Derivative
    {
        public Polynomial Initial { get; }
        public Polynomial Current { get; }
        public int Order { get; }
    }
}
