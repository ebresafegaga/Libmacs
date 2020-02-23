using System;

namespace MathAPI.MathTypes
{
    public struct Complex<T> : IEquatable<Complex<T>>, IComparable<Complex<T>>, IComparable where T : IEquatable<T>, IComparable<T>
    {
        public int CompareTo(Complex<T> other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public bool Equals(Complex<T> other)
        {
            throw new NotImplementedException();
        }
    }
}
