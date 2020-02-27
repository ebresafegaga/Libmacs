using MathAPI.MathTypes;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

#pragma warning disable CS0659 // Polynomial does not override Object.GetHashCode();
#pragma warning disable CS0660
#pragma warning disable CS0661

namespace Test
{
    public readonly struct Polynomial : IEnumerable<Term>, IEquatable<Polynomial>
    {
        public Polynomial (IEnumerable<Term> terms)
        {
            Terms = new Dictionary<int, Term>();
            t = default;
        }

        // Key: Degree, Value: Actual Term
        public IDictionary<int, Term> Terms { get; }

        public ISet<Term> t { get; }

        // Need to sort items!
        public Term this[int position] => Terms.Values.ToList()[position];

        public IEnumerator<Term> GetEnumerator()
        {
            foreach (Term item in Terms.Values)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator ();

        public bool Equals (Polynomial other) => Terms.SequenceEqual (other.Terms) ? true : false;

        public override bool Equals (object obj)
        {
            if (obj is Polynomial p)
            {
                return Equals(p);
            }

            return false;
        }

        public static bool operator == (Polynomial term, Polynomial other)
        {
            return term.Equals(other);
        }

        public static bool operator != (Polynomial term, Polynomial other)
        {
            return !(term == other);
        }

        public static Polynomial operator +(Polynomial polynomial, Polynomial other)
        {
            return default;
        }

        public static Polynomial operator - (Polynomial polynomial, Polynomial other)
        {
            return default;
        }

        public static Polynomial operator - (Polynomial polynomial)
        {
            return default;
        }

        public static Polynomial operator * (Polynomial polynomial, Polynomial other)
        {
            return default;
        }

        public static PolynomialFraction operator / (Polynomial polynomial, Polynomial other)
        {
            return default;
        }

        public static implicit operator Polynomial (Term term)
        {
            return default;
        }

        public static implicit operator Polynomial (int term)
        {
            return default;
        }
    }

    public readonly struct Term : IEnumerable<UnKnown>, IEquatable<Term>
    {
        public Term (Fraction coefficient, params UnKnown[] unKnowns)
        {
            UnKnowns = new SortedSet<UnKnown> (unKnowns.AsEnumerable(), UnKnown.DefaultComparer);
            Coefficient = coefficient;
        }

        public Fraction Coefficient { get; }
        public ISet<UnKnown> UnKnowns { get; }

        public bool Equals (Term other) => UnKnowns.SetEquals (other.UnKnowns);

        public IEnumerator<UnKnown> GetEnumerator () => UnKnowns.AsEnumerable ().GetEnumerator ();

        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();

        public static bool operator == (Term term, Term other)
        {
            return term.Equals (other);
        }

        public static bool operator != (Term term, Term other)
        {
            return !(term == other);
        }

        public static implicit operator Term (int number)
        {
            return new Term(number);
        }
        
        public static implicit operator Term (char value)
        {
            return new Term(1, value);
        }

        public static implicit operator Term (UnKnown unKnown)
        {
            return new Term(1, unKnown);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append (Coefficient.ToString());

            foreach (var unknown in UnKnowns)
                builder.Append (unknown.ToString());

            return builder.ToString();
        }
    }

    public readonly struct UnKnown : IEquatable<UnKnown>
    {
        public UnKnown (char value, int degree)
        {

            // Make sure that value is a letter
            if (!char.IsLetter(value))
                throw new ArgumentException (nameof(value), "Character must be a letter.");
            
            Value = char.ToLower(value);
            Degree = degree;
        }

        static UnKnown ()
        {
            DefaultComparer = new UnknownComparer ();
        }

        public char Value { get; }
        public int Degree { get; }

        public static UnknownComparer DefaultComparer { get; }

        public bool Equals(UnKnown other)
        {
            if (Value == other.Value && Degree == other.Degree)
                return true;

            return false;
        }

        public static implicit operator UnKnown(char value) => 
            new UnKnown(value, 1);

        public override string ToString() =>
            $"{Value}^{Degree}";
    }

    public class UnknownComparer : IComparer<UnKnown>
    {
        public UnknownComparer()
        {
       	}

        public UnknownComparer (Comparison<UnKnown> comparison)
        {
            Comparison = comparison;
        }

        public Comparison<UnKnown> Comparison { get; }
        public int Compare(UnKnown x, UnKnown y)
        {
            if (Comparison != null)
            {
                return Comparison(x, y);
            }
            else
            {
                return x.Value.CompareTo(y.Value);
            }
        }
    }
}
