using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text;
using static MathAPI.Helpers.MatrixHelpers;
using System.Collections;

#pragma warning disable CS0661
#pragma warning disable CS0659

namespace MathAPI.MathTypes
{
    public readonly struct Vector<T> : IEnumerable<T>, IEquatable<Vector<T>> where T : IEquatable<T>
    {
        private readonly T[] _store;

        public Vector (T[] vector)
        {
            _store = vector;
            Length = _store.Length;
        }

        public Vector (Vector<T> vector) 
            : this (vector._store)
        {
        }

        public T this[int index] => _store[index];
        public int Length { get; }

        public bool Equals (Vector<T> other)
        {
            if (Length != other.Length)
                return false;

            for (int i = 0; i < other.Length; i++)
            {
                var param = Expression.Parameter(typeof(T));
                var param2 = param;
                var expression = Expression.Equal(param, param2);
                var equal = Expression.Lambda<Func<T, T, bool>>(expression, param, param2).Compile();

                if (!equal(this[i], other[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals (object obj)
        {
            if (obj is Vector<T> vector)
            {
                return Equals(vector);
            }
            else
            {
                return false;
            }
        }

        public static bool operator == (Vector<T> vector, Vector<T> other)
        {
            return vector.Equals (other);
        }

        public static bool operator != (Vector<T> vector, Vector<T> other)
        {
            return !(vector == other);
        }

        public static Vector<T> operator + (Vector<T> vector, Vector<T> other)
        {
            if (vector.Length != other.Length)
                throw new ArgumentException("Vectors are not compatible", nameof(other));

            T[] vec = new T[vector.Length];
            for (int i = 0; i < vector.Length; i++)
            {
                var param = Expression.Parameter (typeof(T));
                var param2 = param;
                var expression = Expression.Add (param, param2);
                var add = Expression.Lambda<Func<T, T, T>> (expression, param, param2).Compile();

                vec[i] = add (vector[i], other[i]);
            }

            return vec;
        }

        public static Vector<T> operator - (Vector<T> vector, Vector<T> other)
        {
            if (vector.Length != other.Length)
                throw new ArgumentException ("Vectors are not compatible", nameof(other));

            T[] vec = new T[vector.Length];
            for (int i = 0; i < vector.Length; i++)
            {
                var param = Expression.Parameter(typeof(T));
                var param2 = param;
                var expression = Expression.Subtract(param, param2);
                var subtract = Expression.Lambda<Func<T, T, T>>(expression, param, param2).Compile();

                vec[i] = subtract(vector[i], other[i]);
            }

            return vec;
        }

        public static Vector<T> operator -(Vector<T> vector)
        {
            T[] vec = new T[vector.Length];

            for (int i = 0; i < vector.Length; i++)
            {
                var param = Expression.Parameter(typeof(T));
                var expr = Expression.Negate(param);
                var negate = Expression.Lambda<Func<T, T>>(expr, param).Compile();

                vec[i] = negate(vector[i]);
            }

            return vec;
        }

        public static implicit operator Vector<T>(T[] vector)
        {
            return new Vector<T>(vector);
        }

        public static implicit operator Vector<T>((T, T, T) items)
        {
            return new T[] { items.Item1, items.Item2, items.Item3 };
        }

        public static implicit operator Vector<T>((T, T) items)
        {
            return new T[] { items.Item1, items.Item2 };
        }

        public Matrix<T> ToMatrix()
        {
            T[,] mat = new T[1, Length];

            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    mat[i, j] = this[j];
                }
            }

            return mat;
        }

        public T[] ToArray()
        {
            return _store;
        }

        public override string ToString()
        {
            // This is temporary. Works for only _store.Length <= 3 
            return _store.ArrayToTuple().ToString();
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in _store)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
