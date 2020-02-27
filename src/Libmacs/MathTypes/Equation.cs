using System;
using System.Collections.Generic;

#pragma warning disable CS0660
#pragma warning disable CS0661

namespace MathAPI.MathTypes
{
    // TODO: Implement GetHashCode() and Object.Equals(); 👀
    public abstract class Equation : IEquatable<Equation>
    {
        private protected List<Fraction> _terms = new List<Fraction>();
        private protected Fraction _rhs = default;
        private protected Fraction _constant = default;
        private protected Fraction _coefficient = default;

        public static char CoefficientChar { get; set; } = 'x';

        public bool Equals (Equation other)
        {
            int count = 0;
            bool flag = true;
            foreach (Fraction item in other._terms)
            {
                if (_terms[count] != item)
                {
                    flag = false;
                    break;
                }
                count++;
            }
            return flag;
        }

        public static bool operator ==(Equation equation, Equation other) =>
            equation.Equals(other);
        public static bool operator !=(Equation equation, Equation other) =>
            !equation.Equals(other);
    }
}
