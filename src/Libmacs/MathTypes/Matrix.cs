using MathAPI.Helpers;
using System;
using System.Linq.Expressions;

#pragma warning disable CS0660
#pragma warning disable CS0661

namespace MathAPI.MathTypes
{
    public readonly struct Matrix<T> : IEquatable<Matrix<T>> where T : IEquatable<T>
    {
        private readonly T[,] _store;
        private readonly bool _flag;

        public Matrix(T[,] matrix)
        {
            _store = matrix;
            Rank = (_store.GetLength(0), _store.GetLength(1));

            if (Rank == (2, 2))
            {
                _flag = false;
            }
            else if (Rank == (3, 3))
            {
                _flag = false;
            }
            else
            {
                // Identity Matrix is for only for these matrices : Rank (2, 2) and (3, 3).
                _flag = true;
            }
        }

        public Matrix (Matrix<T> matrix) : this (matrix._store)
        {
        }

        public Fraction[,] I
        {
            get
            {
                if (_flag)
                {
                    throw new InvalidOperationException ("Only square matrices have an identity matrix");
                }
                else
                {
                    if (Rank == (2, 2))
                    {
                        // 2 by 2
                        return MatrixHelpers.I2by2;
                    }
                    // 3 by 3
                    return MatrixHelpers.I3by3;
                    
                }
            }
        }

        public T this[int i, int j] => _store[i, j];
        public (int, int) Rank { get; }

        public T[,] ToArray()
        {
            return this._store;
        }

        public Vector<T> ToVector(int column = 0)
        {
            (int rows, int columns) = Rank;

            if (column > (columns - 1))
            {
                throw new IndexOutOfRangeException("Column was out of bound of the matrix");
            }

            T[] vector = new T[rows];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (j == column)
                    {
                        vector[i] = this[i, j];
                    }
                }
            }

            return vector;
        }

        public bool Equals(Matrix<T> other)
        {
            bool flag = true;

            for (int i = 0; i < _store.GetLength(0); i++)
            {
                for (int j = 0; j < _store.GetLength(1); j++)
                {
                    ParameterExpression parameter1 = Expression.Parameter(typeof(T), "this"),
                        parameter2 = Expression.Parameter(typeof(T), "other");
                    BinaryExpression expression = Expression.Equal(parameter1, parameter2);
                    Func<T, T, bool> compare = Expression.Lambda<Func<T, T, bool>>(expression, parameter1, parameter2).Compile();

                    if (!compare(this[i, j], other[i, j]))
                    {
                        flag = false;
                        break;
                    }
                }
            }

            return flag;
        }

        public static Matrix<T> operator +(Matrix<T> matrix, Matrix<T> other)
        {
            if (matrix.Rank != other.Rank)
            {
                throw new ArgumentException("Arguments are not of the same rank", nameof(other));
            }

            T[,] temp = new T[matrix.Rank.Item1, matrix.Rank.Item2];

            for (int i = 0; i < matrix.Rank.Item1; i++)
            {
                for (int j = 0; j < matrix.Rank.Item2; j++)
                {
                    ParameterExpression parameter1 = Expression.Parameter(typeof(T), "matrix"),
                        parameter2 = Expression.Parameter(typeof(T), "other");
                    BinaryExpression expression = Expression.Add(parameter1, parameter2);
                    Func<T, T, T> add = Expression.Lambda<Func<T, T, T>>(expression, parameter1, parameter2).Compile();

                    temp[i, j] = add(matrix[i, j], other[i, j]);
                }
            }

            return new Matrix<T>(temp);
        }

        public static Matrix<T> operator -(Matrix<T> matrix, Matrix<T> other)
        {
            if (matrix.Rank != other.Rank)
            {
                throw new ArgumentException("Arguments are not of the same rank", nameof(other));
            }

            T[,] temp = new T[matrix.Rank.Item1, matrix.Rank.Item2];

            for (int i = 0; i < matrix.Rank.Item1; i++)
            {
                for (int j = 0; j < matrix.Rank.Item2; j++)
                {
                    ParameterExpression parameter1 = Expression.Parameter(typeof(T), "matrix"),
                        parameter2 = Expression.Parameter(typeof(T), "other");
                    BinaryExpression expression = Expression.Subtract(parameter1, parameter2);
                    Func<T, T, T> sub = Expression.Lambda<Func<T, T, T>>(expression, parameter1, parameter2).Compile();

                    temp[i, j] = sub(matrix[i, j], other[i, j]);
                }
            }

            return new Matrix<T>(temp);
        }
        
        public static Matrix<T> operator -(Matrix<T> matrix)
        {
            // TODO
            return default;
        }

        public static Matrix<T> operator *(Matrix<T> matrix, Matrix<T> other)
        {
            T[,] temp = MatrixHelpers.MultplyMatrix<T>(matrix._store, other._store);
            return new Matrix<T>(temp);
        }

        public static Matrix<T> operator *(T value, Matrix<T> other)
        {
            T[,] temp = new T[other.Rank.Item1, other.Rank.Item2];

            for (int i = 0; i < other.Rank.Item1; i++)
            {
                for (int j = 0; j < other.Rank.Item2; j++)
                {
                    ParameterExpression parameter1 = Expression.Parameter(typeof(T), "matrix"),
                        parameter2 = Expression.Parameter(typeof(T), "other");
                    BinaryExpression expression = Expression.Multiply(parameter1, parameter2);
                    Func<T, T, T> mul = Expression.Lambda<Func<T, T, T>>(expression, parameter1, parameter2).Compile();

                    temp[i, j] = mul(value, other[i, j]);
                }
            }

            return new Matrix<T>(temp);
        }

        public static bool operator ==(Matrix<T> matrix, Matrix<T> other)
        {
            return matrix.Equals(other);
        }

        public static bool operator !=(Matrix<T> matrix, Matrix<T> other)
        {
            return !(matrix == other);
        }

        public static implicit operator Matrix<T>(T[,] matrix)
        {
            // TODO: This should not take a reference to the matrix given by the caller.
            // TODO: We should perform a deep copy of matrix._store to this._store. 
            // What are the pros? No unexpected behaviour if the caller uses
            // that reference to that matrix in other functions, because we didn't mutate the elements.
            // cons?  Unexpected behaviour and allocation on heap.
            return new Matrix<T>(matrix);
        }
    }
}
