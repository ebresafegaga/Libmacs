using MathAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MathAPI.MathTypes
{
    public readonly struct Fraction : IComparable, IComparable<Fraction>, IEquatable<Fraction>
    {
        public static Fraction Zero { get; } = new Fraction (0, 1);
        public static Fraction Unit { get; } = new Fraction (1, 1);
 
        public Fraction (long num, long denum)
        {
            numerator = num;
            denuminator = denum;

            if (numerator == 0)
            {
                ImproperFraction = false;
                _value = 0;
                return;
            }

            if (denum == 0)
                throw new ArgumentException
                    ("denuminator cannot be Zero", nameof (denum));

            if (denum < 0)
            {
                denuminator *= -1;
                numerator *= -1;
            }

            _value = (double)numerator / (double)denuminator;
            if (Math.Abs (numerator) > Math.Abs (denuminator))
            {
                ImproperFraction = true;
            }
            else
            {
                ImproperFraction = false;
            }
            GetLowestTerms (out long n, out long d);
            numerator = n;
            denuminator = d;
        }

        // Gaga: "THIS .CTOR IS CRYING OUT FOR HELP 😂😂😂"
        // Gaga : "Maybe I should just delete it...🤔"
        // Fraction::.ctor (f64) : "PLEASE GAGA FIX ME 😭😭😭😭😭😭"
        // Gaga : "WTF, you talkkkkkk? Like talk talk? 🧐🧐"
        // Fraction::.ctor (f64) : "Yeah bro... just fix me 😔"
        // Gaga: "Nahhhhh broski, you're not that important 😘😘"
        // Fraction::.ctor (f64) : "bro please stop 😒"
        // Gaga : "..."
        // Fraction::.ctor (f64) : "I'm done with you, just delete me already 😡"
        // Fraction::.ctor (f64) : "BTW I've put bugs everywhere..."
        // Gaga : "Wait what??????????????? 🙄🙄🙄"
        // Fraction::.ctor (f64) : "E go be..."
        public Fraction (double value)
        {
            _value = value;
            StringBuilder denum = new StringBuilder ("1");
            string valueString = _value.ToString ();

            // This assumes that zero is the only number before the decimal point. 
            // In this form : 0.xxxxxxxxxxx...
            string[] deciamlValue = valueString.Split ('.');

            long decimalInt = Convert.ToInt64 (deciamlValue[1]);
            int noZero = deciamlValue[1].Length;

            foreach (char item in deciamlValue[1])
            {
                denum.Append ("0");
            }
            numerator = decimalInt;
            denuminator = Convert.ToInt64 (denum.ToString ());
            if (_value < 0)
            {
                numerator *= -1;
            }
            if (Math.Abs (numerator) > Math.Abs (denuminator))
            {
                ImproperFraction = true;
            }
            else
            {
                ImproperFraction = false;
            }
            GetLowestTerms (out long n, out long d);
            numerator = n;
            denuminator = d;
        }

        public static bool PrintAsMixed { get; set; } = false;
        
        // TODO: This should be an extension method.
        public bool ImproperFraction { get; }

        private readonly long numerator;
        public long Numerator => numerator;

        private readonly long denuminator;
        public long Denuminator => denuminator;

        private readonly double _value;
        public double Value => _value;

        // Shouldn't be fuzzier than Equals (Fraction other).
        public int CompareTo (Fraction other) => Equals (other) ? 0 : Value.CompareTo (other.Value);
        
        // Can be fuzzier than CompareTo (Fraction other), but not vice versa.
        public bool Equals (Fraction other)
        {
            if (Value == 0 && other.Value == 0)
                return true;
            return Value == other.Value;
        }

        // TODO: Still need to work on this.
        public override string ToString ()
        {
            if (denuminator == 1)
            {
                return $"{numerator}";
            }

            if (Value.CompareTo (0.0) == 0)
            {
                return "0";
            }

            if (PrintAsMixed && ImproperFraction)
            {
                long remainder = numerator % denuminator;
                long wholeNum = numerator / denuminator;
                return $"{wholeNum}.{remainder}/{denuminator}";
            }

            return $"{numerator}/{denuminator}";
        }

        private void GetFraction (out long num, out long denum)
        {
            StringBuilder denuminator = new StringBuilder ("1");
            string valueString = _value.ToString ();
            string[] deciamlValue = valueString.Split ('.');
            long decimalInt = Convert.ToInt64 (deciamlValue[1]);
            int noZero = deciamlValue[1].Length;

            foreach (char item in deciamlValue[1])
            {
                denuminator.Append ("0");
            }

            num = decimalInt;
            denum = Convert.ToInt64 (denuminator.ToString ());

            /* What's the difference?
            denuminator = long.Parse(denuminator.ToString());
            denuminator = Convert.ToInt64(denuminator.ToString());
            Convert.ToInt64 handles null and DOES NOT throws an exception, while long.Parse throws an ArgumentNullException */
        }

        // Add more implicit operators.
        public static implicit operator Fraction (long integer) =>
            new Fraction (integer, 1);

        public static implicit operator Fraction (int integer) =>
            new Fraction (integer, 1);

        // NOTE: NOT STABLE 
        public static explicit operator Fraction (double value)
        {
            decimal _value = (decimal)value;
            StringBuilder denum = new StringBuilder ("1");
            string valueString = _value.ToString ();
            if (!valueString.Contains ("."))
            {
                return new Fraction (Convert.ToInt32 (value), 1);
            }
            string[] deciamlValue = valueString.Split ('.');
            int wholeNumber = Convert.ToInt32 (deciamlValue[0]);
            long decimalInt = Convert.ToInt64 (deciamlValue[1]);
            int noZero = deciamlValue[1].Length;

            foreach (char item in deciamlValue[1])
            {
                denum.Append ("0");
            }
            long numerator = decimalInt;
            long denuminator = Convert.ToInt64 (denum.ToString ());
            if (_value < 0)
            {
                numerator *= -1;
            }
            return new Fraction ((denuminator * wholeNumber) + (numerator), denuminator);
        }

        // NOTE: NOT STABLE 
        public static explicit operator Fraction (decimal value)
        {
            decimal _value = value;
            StringBuilder denum = new StringBuilder ("1");
            string valueString = _value.ToString ();
            string[] deciamlValue = valueString.Split ('.');
            long decimalInt = Convert.ToInt64 (deciamlValue[1]);
            int noZero = deciamlValue[1].Length;

            foreach (char item in deciamlValue[1])
            {
                denum.Append ("0");
            }
            long numerator = decimalInt;
            long denuminator = Convert.ToInt64 (denum.ToString ());
            if (_value < 0)
            {
                numerator *= -1;
            }
            return new Fraction (numerator, denuminator);
        }

        public static Fraction operator + (Fraction fraction, Fraction other)
        {
            return FractionHelpers.Add (fraction, other);
        }

        public static Fraction operator - (Fraction fraction, Fraction other)
        {
            return FractionHelpers.Subtract (fraction, other);
        }

        public static Fraction operator - (Fraction fraction)
        {
            return new Fraction (-fraction.Numerator, fraction.Denuminator);
        }

        public static Fraction operator * (Fraction fraction, Fraction other)
        {
            if (fraction.Value == 0 || other.Value == 0)
                return 0;
            return new Fraction (fraction.numerator * other.numerator, 
                                 fraction.denuminator * other.denuminator);
        }

        public static Fraction operator * (Fraction fraction, int other)
        {
            return fraction * new Fraction (other, 1);
        }

        public static Fraction operator / (Fraction fraction, Fraction other)
        {
            return fraction * other.Inverse ();
        }

        public static Fraction operator / (Fraction fraction, int other)
        {
            return fraction / new Fraction (other, 1);
        }

        // NOTE: This operator should not exist normally.
        // Hack just so my MaxtrixHelper.IsDivisible(Fraction a, Fraction b) can work.
        //public static Fraction operator % (Fraction fraction, Fraction other)
        //{
        //    if ((fraction / other) == 1)
        //    {
        //        return 0;
        //    }
        //    else
        //    {
        //        var product = fraction * other;
        //        if (product.ImproperFraction)
        //        {
        //            // Get the fractinal part and return it.
        //            long remainder = product.numerator % product.denuminator;
        //            long wholeNum = product.numerator / product.denuminator;
        //            return new Fraction(remainder, product.denuminator);
        //        }
        //        else
        //        {
        //            return product;
        //        }

        //        // return 1;
        //    }
        //}

        public static bool operator == (Fraction fraction, Fraction other) => fraction.Equals (other);

        public static bool operator != (Fraction fraction, Fraction other) => !fraction.Equals (other);

        public static bool operator > (Fraction fraction, Fraction other) => fraction.CompareTo (other) > 0;

        public static bool operator < (Fraction fraction, Fraction other) => fraction.CompareTo (other) < 0;

        public static bool operator >= (Fraction fraction, Fraction other) => fraction.CompareTo (other) >= 0;

        public static bool operator <= (Fraction fraction, Fraction other) => fraction.CompareTo (other) <= 0;

        public override bool Equals (object other)
        {
            if (other is Fraction fraction)
                return Equals (fraction);
            return false;
        }

        // 😂😂😂😂😂
        public override int GetHashCode () => (int)(((int)(Numerator * 17) + (int)(Denuminator * 23) << 13) | 0xFFDEAC45);
        // hash = hash * 31 + Denuminator.GetHashCode();
        // hash = hash * 31 + Value.GetHashCode();
        // return hash;

        //public static ref int GCD (ref int a, ref int b)
        //{
        //    if (a == 0)
        //        return ref b;
        //    var x = b % a;
        //    return ref GCD (ref x, ref b);
        //}

        private void GetLowestTerms (out long num, out long denum)
        {
            num = numerator;
            denum = denuminator;

            if (numerator < 0)
            {
                long numeratorBuffer = numerator * -1;

                IList<long> factorsNum = numeratorBuffer.GetFactors ();
                IList<long> factorsDenum = denuminator.GetFactors ();

                ISet<long> set = new SortedSet<long> (factorsNum);
                set.IntersectWith (factorsDenum);

                long largest = GetLarget (set);

                num /= largest;
                denum /= largest;

                return;
            }
            if (numerator == denuminator)
            {
                num = 1;
                denum = 1;
            }
            else
            {
                IList<long> factorsNum = numerator.GetFactors ();
                IList<long> factorsDenum = denuminator.GetFactors ();

                SortedSet<long> set = new SortedSet<long> (factorsNum);
                set.IntersectWith (factorsDenum);

                Debug.Assert (set.Count () > 0);

                long largest = GetLarget (set);

                num /= largest;
                denum /= largest;
            }

            long GetLarget (IEnumerable<long> set)
            {
                long larget = set.First ();

                foreach (long number in set)
                {
                    if (number > larget)
                    {
                        larget = number;
                    }
                }
                return larget;
            }
        }

        public Fraction Inverse ()
        {
            if (numerator == 0)
                return 0;
            return new Fraction (denuminator, numerator);
        }

        int IComparable.CompareTo (object other)
        {
            if (other is Fraction fraction)
                return CompareTo (fraction);

            throw new InvalidOperationException ("other is not a Fraction");
        }
    }
}
