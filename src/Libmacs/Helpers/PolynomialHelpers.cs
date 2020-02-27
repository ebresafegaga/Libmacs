using MathAPI.MathTypes;
using System;
using System.Collections.Generic;

namespace MathAPI.Helpers
{
    public static class PolynomialHelpers
    {
        public static Term AddcompatibleTerms(this Term term, Term other)
        {
            if (term.Degree != other.Degree)
                throw new ArgumentException("Terms must have the same degree.", nameof(other));

            Fraction cf = term.Coefficient + other.Coefficient;
            return new Term(term.Degree, cf);
        }

        public static int GetDegree(this IEnumerable<Term> terms)
        {
            int degree = 0;
            foreach (Term term in terms)
            {
                if (term.Degree > degree)
                {
                    degree = term.Degree;
                }
            }

            return degree;
        }

        public static int GetDegree(this Polynomial polynomial)
        {
            return polynomial.Terms.Values.GetDegree();
        }

        public static (Polynomial remainder, Polynomial result) Divide(in Polynomial A, in Polynomial B)
        {
            // A / B
            Polynomial quotient = A;
            Polynomial divisor = B;
            var divisorValue = divisor[0];
            PolynomialBuilder builder = new PolynomialBuilder();

            while (!quotient.IsConstant())
            {
                if (divisor.GetDegree() > quotient.GetDegree())
                {
                    break;
                }

                var quotientValue = quotient[0];
                var term = quotientValue / divisorValue;
                builder.Append(term);

                var product = term * divisor;
                quotient -= product;
            }
            var dividend = builder.Polynomial;

            return (quotient, dividend);
        }

        public static bool IsCompatible(this Term term, Term other)
        {
            if (term.Degree == other.Degree)
            {
                return true;
            }

            return false;
        }

        public static bool IsConstant(this Polynomial polynomial)
        {
            foreach (var degree in polynomial.Terms.Keys)
            {
                if (degree != 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
