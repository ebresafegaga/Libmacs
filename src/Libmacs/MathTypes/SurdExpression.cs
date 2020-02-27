using MathAPI.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MathAPI.MathTypes
{
    public readonly struct SurdExpression : IComparable, IComparable<SurdExpression>, IEnumerable<Surd>, IEquatable<SurdExpression>
    {
        public static readonly SurdExpression Empty = new SurdExpression (1, 1);
        private readonly Surd[] _surds;
        private readonly List<double> _values;

        public SurdExpression (params Surd[] surds)
        {
            _values = new List<double> ();

            var surdList = surds.ToList ();
            var dictionary = new Dictionary<Fraction, Fraction> ();
            var temp = new List<Surd> ();

            // Add "commons"
            foreach (var s in surdList)
            {
                // Make sure surds are in reduced form.
                var sReduced = s.Reduce ();
                var r = sReduced.Root;
                var c = sReduced.Constant;

                if (!dictionary.ContainsKey (r))
                {
                    dictionary.Add (r, c);
                }
                else
                {
                    dictionary[r] += c;
                }
            }

            // Put it in surd form 
            foreach (var key in dictionary.Keys)
            {
                temp.Add (new Surd (key, dictionary[key]));
            }

            _surds = temp.ToArray ();

            foreach (var surd in _surds)
            {
                _values.Add (surd.Value);
            }

            Value = _values.Sum ();
            IsBinary = _surds.Length == 2 ? true : false;
            Length = _surds.Length;
        }

        public SurdExpression (IEnumerable<Surd> surds) : this (surds.ToArray ())
        { }

        public SurdExpression (Fraction root) : this (new Surd (root))
        { }

        public Surd this[int index] => _surds[index];
        public double Value { get; }
        public bool IsBinary { get; }
        public int Length { get; }

        public SurdExpression Slice (int index, int length)
        {
            // TODO
            throw new NotImplementedException ();
        }

        // It is NOT possible to implement list initializers as this type is readonly.
        //
        // public void Add (Surd surd)
        // {
        //    List<Surd> s =_surds.ToList ();
        //    s.Add (surd);
        //    _surds = s.ToArray ();
        // }

        #region IComparable
        public int CompareTo (object obj)
        {
            if (obj is SurdExpression surds)
            {
                return this.CompareTo (surds);
            }
            else
            {
                return 1;
            }
        }

        public int CompareTo (SurdExpression other)
        {
            if (this == other)
                return 0;
            return Value.CompareTo (other.Value);
        }
        #endregion

        #region IEquatable
        public bool Equals (SurdExpression other)
        {
            int count = 0;
            bool flag = true;
            foreach (Surd item in other)
            {
                if (this[count] == item)
                {
                    // flag = true;
                }
                else
                {
                    flag = false;
                    break;
                }
                ++count;
            }
            return flag;
        }

        public override bool Equals (object obj)
        {
            if (obj is SurdExpression surds)
            {
                return Equals (surds);
            }
            else
            {
                return false;

            }
        }

        public override int GetHashCode ()
        {
            return Value.GetHashCode ();
        }
        #endregion

        #region IEnumerable
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return (IEnumerator)GetEnumerator ();
        }

        public IEnumerator<Surd> GetEnumerator ()
        {
            foreach (Surd surd in _surds)
            {
                yield return surd;
            }
        }
        #endregion

        public SurdExpression Rationalize ()
        {
            throw new NotImplementedException ();
        }

        public int? Find (Surd surd)
        {
            int count = 0;
            foreach (var item in this)
            {
                if (surd == item)
                {
                    return count;
                }
                count++;
            }
            return null;
        }

        public int? FindRootIfPresent (Fraction root)
        {
            int count = 0;
            foreach (var item in this)
            {
                if (root == item.Root)
                {
                    return count;
                }
                count++;
            }
            return null;
        }

        public static bool operator == (SurdExpression surds, SurdExpression other)
        {
            return surds.Equals (other);
        }

        public static bool operator != (SurdExpression surds, SurdExpression other)
        {
            return !surds.Equals (other);
        }

        public static SurdExpression operator + (SurdExpression surds, SurdExpression other)
        {
            if (surds._surds is null && other._surds != null)
                return other;
            if (other._surds is null && surds._surds != null)
                return surds;
            if (surds._surds is null && other._surds is null)
                return new SurdExpression (Surd.Empty);

            var _surds = surds.ToList ();
            var _other = other.ToList ();

            List<Fraction> _common = new List<Fraction> ();
            Dictionary<Fraction, Fraction> surdToConstant = new Dictionary<Fraction, Fraction> ();

            List<Surd> result = new List<Surd> ();

            // Get common Surd Root.
            foreach (var surd in other)
            {
                int? id = surds.FindRootIfPresent (surd.Root);
                if (id != null)
                {
                    _common.Add (surd.Root);

                    var tempR1 = (int)surds.FindRootIfPresent (surd.Root);
                    var tempR2 = (int)other.FindRootIfPresent (surd.Root);
                    // Remove "commons"
                    var r1 = _surds[(int)surds.FindRootIfPresent (surd.Root)];
                    //var r2H = _other[(int)other.FindRootIfPresent(surd.Root)];
                    var r2 = surd;
                    //Debug.Assert(r2H == r2);
                    _surds.Remove (r1);
                    _other.Remove (r2);
                }
            }

            // Add the contants of the "common" surds
            foreach (var root in _common)
            {
                var s1 = (int)surds.FindRootIfPresent (root);
                var s2 = (int)other.FindRootIfPresent (root);

                var c1 = surds[s1].Constant;
                var c2 = other[s2].Constant;
                var sum = c1 + c2;
                surdToConstant.Add (root, sum);
            }

            // Add the "un-commons" to result
            result.AddRange (_surds);
            result.AddRange (_other);

            // Add the "Commons" to the result.
            foreach (var item in surdToConstant.Keys)
            {
                var surdRoot = item;
                var surdConstant = surdToConstant[item];
                var surd = new Surd (surdRoot, surdConstant);
                result.Add (surd);
            }

            return new SurdExpression (result.ToArray ());
        }

        public static SurdExpression operator + (SurdExpression surds, Fraction fraction)
        {
            return surds + new SurdExpression (fraction);
        }

        public static SurdExpression operator - (SurdExpression surds)
        {
            var _surds = new List<Surd> ();

            foreach (var item in surds)
            {
                _surds.Add ((-item));
            }

            return new SurdExpression (_surds);
        }

        public static SurdExpression operator - (SurdExpression surds, SurdExpression other)
        {
            return surds + (-other);
        }

        public static SurdExpression operator - (SurdExpression surds, Fraction fraction)
        {
            return surds - new SurdExpression (fraction);
        }

        public static SurdExpression operator * (SurdExpression surds, SurdExpression other)
        {
            var _sum = new List<SurdExpression> ();
            SurdExpression realSum = default;

            for (int i = 0; i < surds.Length; i++)
            {
                var sum = new SurdExpression ();
                for (int j = 0; j < other.Length; j++)
                {
                    var temp = surds[i] * other[j];
                    sum += temp;
                }
                _sum.Add (sum);
            }

            foreach (var item in _sum)
            {
                realSum += item;
            }

            return realSum;
        }

        // TODO: Implement this method
        //public static SurdExpression operator *(Fraction fraction, SurdExpression other)
        //{

        //}

        public static implicit operator SurdExpression (Surd surd)
        {
            return new SurdExpression (surd);
        }

        public static implicit operator SurdExpression (Surd[] surds)
        {
            return new SurdExpression (surds);
        }

        public static implicit operator SurdExpression (int[] surds)
        {
            var _surds = new List<Surd> ();

            foreach (var item in surds)
            {
                _surds.Add (new Surd (item));
            }

            return new SurdExpression (_surds);
        }
    }
}
