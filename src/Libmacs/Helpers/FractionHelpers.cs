using MathAPI.MathTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo ("Libmacs.UnitTests")]
namespace MathAPI.Helpers
{
    internal static class FractionHelpers
    {
        public static Fraction Abs (this Fraction fraction)
        {
            // Please note that I am only comparing the numerator here. This is because in the .ctor of Fraction,
            // I transfer the sign from the denuminator to the numerator, if any.
            if (fraction.Numerator < 0)
                return new Fraction (Math.Abs (fraction.Numerator), fraction.Denuminator);
            return fraction;
        }

        public static IList<int> GetFactors (this int number)
        {
            List<int> factors = new List<int> ();

            for (int i = 1; i <= number; i++)
            {
                // NOTE: I don't want to include 1's as the Fraction type depends on this behaviour.
                if (number % i == 0 && i != 1)
                {
                    factors.Add (i);
                }
            }
            return factors;
        }

        public static IList<long> GetFactors (this long number)
        {
            Debug.Assert (number > 0);
            List<long> _factors = new List<long> ();
            checked
            {
                for (int i = 1; i <= number; i++)
                {
                    if (number % i == 0)
                    {
                        _factors.Add (i);
                    }
                }
            }
            return _factors;
        }

        public static Fraction Add (Fraction f1, Fraction f2)
        {
            // Fast path
            if (f1.Denuminator == 1 && f2.Denuminator == 1)
                return f1.Numerator + f2.Numerator;
            if (f1 == Fraction.Zero && f2 == Fraction.Zero)
                return Fraction.Zero;
            if (f1 == Fraction.Zero)
                return f2;
            if (f2 == Fraction.Zero)
                return f1;
            if (f1 == f2.Inverse ())
                return Fraction.Zero;

            // Slow path
            Fraction fraction = default;
            if (f1.Denuminator == f2.Denuminator)
            {
                fraction = new Fraction ((f1.Numerator + f2.Numerator), f1.Denuminator);
            }
            else
            {
                long lcm = GetLCM (f1.Denuminator, f2.Denuminator);
                fraction = new Fraction (
                    ((lcm / f1.Denuminator) * f1.Numerator) + 
                    ((lcm / f2.Denuminator) * f2.Numerator), 
                    lcm);
            }

            return fraction;
        }

        public static Fraction Subtract (Fraction f1, Fraction f2)
        {
            // Fast path
            if (f1.Value == 0)
                return -f2;
            if (f2.Value == 0)
                return f1;

            // Slow path
            Fraction fraction = default;
            if (f1.Denuminator == f2.Denuminator)
            {
                fraction = new Fraction ((f1.Numerator - f2.Numerator), f1.Denuminator);
            }
            else
            {
                long lcm = GetLCM (f1.Denuminator, f2.Denuminator);
                fraction = new Fraction (
                    ((lcm / f1.Denuminator) * f1.Numerator) - 
                    ((lcm / f2.Denuminator) * f2.Numerator), 
                    lcm);
            }
            return fraction;
        }

        private static long GetLCM (long a, long b)
        {
            long lcm = 1;

            var aMultiples = a.GetNMultiples (b);
            var bMultiples = b.GetNMultiples (a);

            SortedSet<long> set = new SortedSet<long> (aMultiples);
            set.IntersectWith (bMultiples);
            lcm = GetSmallest (set);

            long GetSmallest (IEnumerable<long> items)
            {
                long smallest = long.MaxValue;
                foreach (var item in items)
                {
                    if (item < smallest)
                    {
                        smallest = item;
                    }
                }
                return smallest;
            }

            return lcm;
        }

        public static IList<long> GetNMultiples (this long a, long n)
        {
            if (a == 0)
                return Array.Empty<long> ().ToList ();

            List<long> _multiples = new List<long> ();

            for (long i = 1; i <= n; i++)
            {
                long value = a * i;
                _multiples.Add (value);
            }
            return _multiples;
        }
    }
}
