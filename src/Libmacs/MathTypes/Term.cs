using MathAPI.MathTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using static MathAPI.Helpers.PolynomialHelpers;

namespace MathAPI
{
    public readonly struct Term : IEquatable<Term>
    {
        public Term(int degree, Fraction cf)
        {
            UnKnown = 'X';
            Degree = degree;
            Coefficient = cf;
            IsConstant = false;

            if (Degree == 0)
            {
                IsConstant = true;
            }
        }

        public int Degree { get; }
        public Fraction Coefficient { get; }
        public char UnKnown { get; }
        public bool IsConstant { get; }

        public bool Equals(Term other)
        {
            if (Degree == other.Degree &&
                Coefficient == other.Coefficient &&
                UnKnown == other.UnKnown)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Term term)
                Equals(term);
            return false;
        }

        public override int GetHashCode()
        {
            // Revisit
            int hash = 17 * (Degree << (int)Coefficient.Denuminator);
            hash |= 43 ^ hash ^ (hash * 23);
            return hash;
        }

        public static bool operator ==(Term term, Term other)
        {
            return term.Equals(other);
        }

        public static bool operator !=(Term term, Term other)
        {
            return !(term == other);
        }

        public static Polynomial operator +(Term term, Term other)
        {
            if (term.UnKnown != other.UnKnown)
                throw new ArgumentException("Terms must have the same unknowns", nameof(other));

            if (term.IsCompatible(other))
            {
                Fraction cf = term.Coefficient + other.Coefficient;
                int dg = term.Degree;

                return new Term(dg, cf);
            }

            List<Term> list = (new Term[] { term, other }).ToList();
            return new Polynomial(list);
        }

        public static Polynomial operator -(Term term, Term other)
        {
            if (term.UnKnown != other.UnKnown)
                throw new ArgumentException("Terms must have the same unknowns", nameof(other));

            if (term.IsCompatible(other))
            {
                Fraction cf = term.Coefficient - other.Coefficient;
                int dg = term.Degree;

                return new Term(dg, cf);
            }

            List<Term> list = (new Term[] { term, -other }).ToList();
            return new Polynomial(list);
        }

        public static Term operator -(Term term)
        {
            var cf = -term.Coefficient;
            return new Term(term.Degree, cf);
        }

        public static Term operator *(Term term, Term other)
        {
            if (term.UnKnown != other.UnKnown)
                throw new ArgumentException("Terms must have the same unknowns", nameof(other));

            var dg = term.Degree + other.Degree;
            var cf = term.Coefficient * other.Coefficient;

            return new Term(dg, cf);
        }

        public static Polynomial operator *(Term term, Polynomial polynomial)
        {
            return (Polynomial)term * polynomial; 
        }

        public static Term operator /(Term term, Term other)
        {
            if (term.UnKnown != other.UnKnown)
                throw new ArgumentException("Terms must have the same unknowns", nameof(other));

            var dg = term.Degree - other.Degree;
            var cf = term.Coefficient / other.Coefficient;

            return new Term(dg, cf);
        }

        public static implicit operator Term(int number)
        {
            return new Term(0, number);
        }

        public override string ToString()
        {
            return $"{Coefficient}{UnKnown}**{Degree} ";
        }
    }
}
