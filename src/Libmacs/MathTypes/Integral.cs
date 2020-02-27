using System;

namespace MathAPI.MathTypes
{
    public readonly struct Integral
    {
        public Integral (Polynomial initial, Polynomial current)
        {
            Initial = initial;
            Current = current;
        }

        public Polynomial Initial { get; }
        public Polynomial Current { get; }
        //public int Order { get; }
    }
}
