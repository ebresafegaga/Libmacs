using System;
using static MathAPI.Helpers.SurdHelpers;

#pragma warning disable CS0660
#pragma warning disable CS0661

namespace MathAPI.MathTypes
{
    public sealed class QuadraticEquation : Equation
    {
        private (Fraction a, Fraction b, Fraction c) Terms { get; }
        public QuadraticEquation (Fraction a, Fraction b, Fraction c, Fraction rhs)
        {
            _rhs = rhs;
            _constant = c;
            Terms = (a, b, c);

            // Assuming rhs == 0, is true always.
            _terms.Add(a);
            _terms.Add(b);
            _terms.Add(c);
        }

        public (Fraction x1, Fraction x2) Solve()
        {
            Fraction x1 = default, x2 = default;
            Fraction D = (Terms.b * Terms.b) - (4 * Terms.a * Terms.c);
            if (D < 0)
            {
                // Imaginary roots
            }
            else
            {
                if (!D.IsPerfectSquare())
                {
                    Console.WriteLine("Surdic Quadratic!");
                    // TODO: solve as surds.
                    return (0, 0);
                }

                Fraction rootD = (Fraction) Math.Sqrt(D.Value);
                x1 = ((-Terms.b) + rootD) / (2 * Terms.a);
                x2 = ((-Terms.b) - rootD) / (2 * Terms.a);
            }

            return (x1, x2);
        }
    }
}
