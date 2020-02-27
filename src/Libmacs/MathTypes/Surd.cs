using MathAPI.Helpers;
using System;

namespace MathAPI.MathTypes
{
    // A Surd is a discriminated union of an integer value and an irrational number. 
    // Can we optimize the design of this structure, based on this? yes TODO!
    public readonly struct Surd : IComparable<Surd>, IEquatable<Surd>, IComparable
    {
        public static readonly Surd Empty = new Surd (1);

        // TODO: Make ctor reduce it.
        public Surd (Fraction constant, Fraction root)
        {
            Root = root;
            Constant = constant;
            Value = constant.Value * Math.Sqrt (root.Value);
            // this = Reduce(this) -- Oops, Stack Overflow for obvious reasons.
            // Leaving the above comment to show that, if the ctor if used to create a new Surd
            // it won't be reduced.
        }

        public Surd (Fraction root) : this (1, root)
        { }

        public static Surd CreateReduced (Fraction constant, Fraction root)
        {
            var surd = new Surd (root, constant);
            var reducedSurd = Reduce (surd);
            return reducedSurd;
        }

        public static Surd CreateReduced (Fraction root)
        {
            return CreateReduced (1, root);
        }

        private static Surd Reduce (Surd initialSurd)
        {
            int root = (int) initialSurd.Root.Numerator;
            int constant = (int) initialSurd.Constant.Numerator;

            var surd = new Surd (root, constant);

            while (surd.IsReduceable ())
            {
                var factors = ((int)surd.Root.Numerator).GetFactors ();
                factors.Remove (1);
                factors.Remove ((int)surd.Root.Numerator);

                Fraction perfect = default;
                Fraction other = default;
                int sqrt = default;

                // Perfect squares which don't have factors that are perfect squares.
                if (surd.Root.IsPerfectSquare ())
                {
                    sqrt = (int)Math.Sqrt (surd.Root.Value);
                    surd = new Surd (1, (surd.Constant * sqrt));
                    continue;
                }

                foreach (Fraction item in factors)
                {
                    // Get the first perfect square.
                    if (item.IsPerfectSquare ())
                    {
                        perfect = item;
                        other = root / perfect;
                        sqrt = (int)Math.Sqrt (perfect.Value);
                        break;
                    }
                }
                surd = new Surd (other, (surd.Constant * sqrt));
                root = (int)surd.Root.Numerator;
            }
            return surd;
        }

        public Fraction Root { get; }
        public Fraction Constant { get; }
        public double Value { get; }

        public Surd Rationalize () => this * this;

        #region Operators
        public int CompareTo (Surd other)
        {
            if (this == other)
                return 0;
            return Value.CompareTo (other);
        }

        public int CompareTo (object obj)
        {
            if (obj is Surd surd)
            {
                return this.CompareTo (surd);
            }
            else
            {
                return 1;
            }
        }

        public bool Equals (Surd other)
        {
            if (Root == other.Root &&
                Constant == other.Constant)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Equals (object obj)
        {
            if (obj is Surd surd)
            {
                return Equals (surd);
            }
            else
            {
                return false;
            }
        }

        public static bool operator == (Surd surd, Surd other)
        {
            return surd.Equals (other);
        }

        public override int GetHashCode ()
        {
            return Value.GetHashCode ();
        }

        public static bool operator != (Surd surd, Surd other)
        {
            return !surd.Equals (other);
        }

        public static SurdExpression operator + (Surd surd, Surd other)
        {
            return new SurdExpression (surd, other);
        }

        public static SurdExpression operator - (Surd surd, Surd other)
        {
            return new SurdExpression (surd, (-other));
        }

        public static Surd operator - (Surd surd)
        {
            return new Surd (surd.Root, (-surd.Constant)).Reduce ();
        }

        public static Surd operator * (Surd surd, Surd other)
        {
            var root = surd.Root * other.Root;
            var constant = surd.Constant * other.Constant;
            return new Surd (root, constant).Reduce ();
        }

        public static SurdFraction operator / (Surd surd, Surd other)
        {
            throw new NotImplementedException ();
        }

        public static implicit operator Surd (int value)
        {
            return new Surd (1, value).Reduce ();
        }

        public static implicit operator Surd (Fraction value)
        {
            return new Surd (1, value).Reduce ();
        }
        #endregion
    }
}
