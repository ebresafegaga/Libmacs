using System;
using MathAPI.Helpers;

namespace MathAPI.MathTypes
{
    public readonly struct PolynomialFraction
    {
        public PolynomialFraction(Polynomial num, Polynomial denum)
        {
            Numerator = num;
            Denuminator = denum;
        }

        public Polynomial Numerator { get; }
        public Polynomial Denuminator { get; }

        public (Polynomial remainder, Polynomial result) Divide()
        {
            var result = PolynomialHelpers.Divide(Numerator, Denuminator);
            return result;
        }
    }
}
