using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Diagnostics;
using MathAPI.MathTypes;
using System.Runtime.CompilerServices;


namespace MathAPI.Helpers
{
    public static class MatrixHelpers
    {
        static MatrixHelpers ()
        {
            Fraction[,] _i2 = new Fraction[2, 2];
            for (int i = 0; i < _i2.GetLength (0); i++)
            {
                for (int j = 0; j < _i2.GetLength (1); j++)
                {
                    // A pivot.
                    if (i == j)
                    {
                        _i2[i, j] = 1;
                    }
                }
            }

            Fraction[,] _i3 = new Fraction[3, 3];
            for (int i = 0; i < _i3.GetLength (0); i++)
            {
                for (int j = 0; j < _i3.GetLength (1); j++)
                {
                    // A pivot.
                    if (i == j)
                    {
                        _i3[i, j] = 1;
                    }
                }
            }

            I2by2 = _i2;
            I3by3 = _i3;
        }

        public static int GetRank (this Matrix<Fraction> matrix)
        {
            var mat = matrix.ToArray ();
            mat.GetRowEchelonForm ();
            return mat.CountNonZeroRows ();
        }

        public static int CountNonZeroRows (this Fraction[,] matrix)
        {
            var (rows, _) = (matrix.GetLength (0), matrix.GetLength (1));

            int count = 0;
            for (int i = 0; i < rows; i++)
            {
                if (!RowIsZeroOrEmpty (i, matrix))
                {
                    count++;
                }
            }

            return count;
        }

        public static Matrix<Fraction> AppendRow (this Matrix<Fraction> matrix, int value = 0)
        {
            var (rows, columns) = matrix.Rank;

            Fraction val = value;
            Fraction[,] mat = new Fraction[(rows + 1), columns];
            var (rRows, cColumns) = mat.AsMatrix ().Rank;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    mat[i, j] = matrix[i, j];
                }
            }

            for (int i = 0; i < rRows; i++)
            {
                for (int j = 0; j < cColumns; j++)
                {
                    if (i == (rRows - 1))
                    {
                        mat[i, j] = val;
                    }
                }
            }

            return mat;
        }

        public static Matrix<Fraction> AppendColumn (this Matrix<Fraction> matrix, int value = 0)
        {
            var (rows, columns) = matrix.Rank;

            Fraction val = value;
            Fraction[,] mat = new Fraction[rows, (columns + 1)];
            var (rRows, cColumns) = mat.AsMatrix ().Rank;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    mat[i, j] = matrix[i, j];
                }
            }

            for (int i = 0; i < rRows; i++)
            {
                for (int j = 0; j < cColumns; j++)
                {
                    if (j == (cColumns - 1))
                    {
                        mat[i, j] = val;
                    }
                }
            }

            return mat;
        }

        public static Fraction[,] I2by2 { get; }
        public static Fraction[,] I3by3 { get; }

        public static IEnumerable<Fraction> GetEigenValues (this Matrix<Fraction> matrix)
        {
            QuadraticEquation quadraticEqu;
            CubicEquation cubicEqu;

            var expression = matrix.AsExpressionMatrix ();
            var identityExpression = matrix.I
                                    .AsMatrix ()
                                    .AsIdentityExpressionMatrix ();

            var subtraction = expression - identityExpression;
            var values = new List<Fraction> ();

            var det = subtraction.GetDeterminant ();

            if (matrix.Rank == (2, 2))
            {
                quadraticEqu = (QuadraticEquation)det.AsEquation ();
                var (x1, x2) = quadraticEqu.Solve ();

                var set = new HashSet<Fraction> (new[] { x1, x2 });

                foreach (var item in set)
                {
                    values.Add (item);
                }

                return values;
            }
            else if (matrix.Rank == (3, 3))
            {
                cubicEqu = (CubicEquation)det.AsEquation ();
                var (x1, x2, x3) = cubicEqu.Solve ();

                HashSet<Fraction> set = new HashSet<Fraction> (new[] { x1, x2, x3 });
                foreach (var item in set)
                {
                    values.Add (item);
                }

                return values;
            }

            // 😘
            throw new NotImplementedException ("Eigen Values for matrices of Rank >= (4, 4) has not yet being implemented");
        }

        public static IEnumerable<LinearSystem.SolutionSet> GetEigenVectors (this Matrix<Fraction> matrix)
        {
            QuadraticEquation quadraticEqu;
            CubicEquation cubicEqu;

            var expression = matrix.AsExpressionMatrix ();
            var identityExpression = matrix.I.AsMatrix ().AsIdentityExpressionMatrix ();
            var subtraction = expression - identityExpression;

            var det = subtraction.GetDeterminant ();
            var solution = new List<LinearSystem.SolutionSet> ();

            if (matrix.Rank == (2, 2))
            {
                quadraticEqu = (QuadraticEquation)det.AsEquation ();
                var (x1, x2) = quadraticEqu.Solve ();

                HashSet<Fraction> set = new HashSet<Fraction> (new Fraction[] { x1, x2 });
                foreach (var item in set)
                {
                    var mat = subtraction.SubstituteExpressionToConstant (item).AppendColumn (0);

                    solution.Add (mat.Solve ());
                }

                return solution;
            }
            else if (matrix.Rank == (3, 3))
            {
                cubicEqu = (CubicEquation)det.AsEquation ();
                var (x1, x2, x3) = cubicEqu.Solve ();

                HashSet<Fraction> set = new HashSet<Fraction> (new Fraction[] { x1, x2, x3 });
                foreach (var item in set)
                {
                    var mat = subtraction.SubstituteExpressionToConstant (item).AppendColumn (0);

                    solution.Add (mat.Solve ());
                }

                return solution;
            }

            throw new NotImplementedException ("Eigen Vectors for matrices of Rank >= (4, 4) has not yet being implemented");
        }

        public static (T, T, T) ArrayToTuple<T> (this T[] array)
        {
            if (array.Length == 1)
            {
                return (array[0], default, default);
            }

            if (array.Length == 2)
            {
                return (array[0], array[1], default);
            }

            if (array.Length == 3)
            {
                return (array[0], array[1], array[2]);
            }

            throw new NotSupportedException ();
        }

        public static bool IsZeroVector (this Fraction[] vector)
        {
            for (int i = 0; i < vector.Length; i++)
            {
                if (vector[i] != 0)
                {
                    return false;
                }
            }
            return true;
        }

        public static Fraction[] GetRow (this Fraction[,] matrix, int row)
        {
            var rows = matrix.GetLength (0);
            var columns = matrix.GetLength (1);
            var list = new List<Fraction> ();
            var startAdding = false;

            if (row > (rows - 1))
            {
                throw new IndexOutOfRangeException ("Row is out of bounds");
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (!startAdding)
                    {
                        // Monitor on each iter.
                        if (row == i)
                        {
                            // Start getting values.
                            startAdding = true;
                        }
                    }
                    if (startAdding)
                    {
                        // Check if we have left the row.
                        if (row != i)
                        {
                            // break if we have passed the desired row.
                            break;
                        }
                        list.Add (matrix[i, j]);
                    }
                }
            }

            return list.ToArray ();
        }

        public static Fraction[] GetColumn (this Fraction[,] matrix, int column)
        {
            int rows = matrix.GetLength (0);
            int columns = matrix.GetLength (1);
            var list = new List<Fraction> ();

            if (column > (columns - 1))
            {
                throw new IndexOutOfRangeException ("Column is out of bounds");
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (column == j)
                    {
                        list.Add (matrix[i, j]);
                    }
                }
            }

            return list.ToArray ();
        }

        public static Matrix<Fraction> SubstituteExpressionToConstant (this Matrix<ExpressionTerms> matrix, Fraction value)
        {
            var newMatrix = new Fraction[matrix.Rank.Item1, matrix.Rank.Item2];

            for (int i = 0; i < matrix.Rank.Item1; i++)
            {
                for (int j = 0; j < matrix.Rank.Item2; j++)
                {
                    newMatrix[i, j] = (((LinearExpression)matrix[i, j]).Terms.a * value) +
                        ((LinearExpression)matrix[i, j]).Terms.b;
                }
            }

            return newMatrix;
        }

        public static Matrix<ExpressionTerms> AsIdentityExpressionMatrix (this Matrix<Fraction> identity)
        {
            var newMatrix = new ExpressionTerms[identity.Rank.Item1, identity.Rank.Item2];

            for (int i = 0; i < identity.Rank.Item1; i++)
            {
                for (int j = 0; j < identity.Rank.Item2; j++)
                {
                    if (i == j)
                    {
                        newMatrix[i, j] = new LinearExpression (1, 0);
                    }
                    else
                    {
                        newMatrix[i, j] = new LinearExpression (0, 0);
                    }
                }
            }

            return newMatrix;
        }

        public static Matrix<ExpressionTerms> AsExpressionMatrix (this Matrix<Fraction> matrix)
        {
            ExpressionTerms[,] newMatrix = new ExpressionTerms[matrix.Rank.Item1, matrix.Rank.Item2];

            for (int i = 0; i < matrix.Rank.Item1; i++)
            {
                for (int j = 0; j < matrix.Rank.Item2; j++)
                {
                    if (matrix[i, j] == 0)
                    {
                        // To remove nulls
                        newMatrix[i, j] = new LinearExpression (0, 0);
                    }
                    else
                    {
                        newMatrix[i, j] = new LinearExpression (0, (matrix[i, j]));
                    }
                }
            }

            return newMatrix;
        }

        public static Matrix<T> AsMatrix<T> (this T[,] matrix) where T : IEquatable<T>
        {
            return new Matrix<T> (matrix);
        }
        
        public static (Fraction[,] L, Fraction[,] U) LUDecompose (this Fraction[,] matrix)
        {
            int rows = matrix.GetLength (0);
            int columns = matrix.GetLength (1);

            if (rows != columns)
            {
                throw new ArgumentException
                    ("Matrix must be a square matrix.", nameof (matrix));
            }
            
            // If you reduce matrix to echelon form, you get U.
            // L = matrix of multipliers, -m. where m (i, j) = -a (i, j) / a (i, i).
            // And 1's as the diagonals and zero above. Got it?
            // You're essentially finding the row echelon form and keeping the constants we used to reduce that row in the pivot below 
            // R (j) -> 2R (j-1) + R (j). Let the pivot location in R (j) = (x, y), then L (x, y) = -2.

            for (int i = 1; i < rows-1; i++)
            {
                for (int j = i + 1; j < columns; j++)
                {
                    var m = matrix[i, j] / matrix[i, i];
                }
            }

            var l = matrix.GetRowEchelonForm (false);
            var u = new Fraction[rows, columns];
            
            Debug.Assert (l.AsMatrix () * u.AsMatrix () == matrix);

            return (l, u);
        }

        public static Fraction[,] GetClassicalAdjoint (Fraction[,] matrix)
        {
            int rows = matrix.GetLength (0);
            int columns = matrix.GetLength (1);

            Fraction[,] cofactors = new Fraction[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    cofactors[i, j] = GetSignedMinor (i, j, matrix);
                }
            }

            var adjoint = Transpose (cofactors);
            return adjoint;
        }

        public static Fraction GetDeterminant (this Fraction[,] matrix)
        {
            int rows = matrix.GetLength (0);
            int columns = matrix.GetLength (1);
            Fraction det = default;

            if (rows != columns)
            {
                throw new ArgumentException ("Matrix must be a square matrix", nameof (matrix));
            }

            if (rows == 2)
            {
                det = (matrix[0, 0] * matrix[1, 1]) - (matrix[0, 1] * matrix[1, 0]);
            }
            else
            {
                Fraction sum = default;
                for (int j = 0; j < columns; j++)
                {
                    sum += matrix[0, j] * GetSignedMinor (0, j, matrix);
                }
                det = sum;
            }

            return det;
        }

        public static T GetDeterminant<T> (this Matrix<T> matrix) where T : IEquatable<T>
        {
            return matrix.ToArray ().GetDeterminant ();
        }

        // Use this method with CAUTION.
        // +, - and * operators have to be defined on T.
        public static T GetDeterminant<T> (this T[,] matrix)
        {
            int rows = matrix.GetLength (0);
            int columns = matrix.GetLength (1);
            
            if (rows != columns)
            {
                throw new ArgumentException ("Matrix must be a square matrix", nameof (matrix));
            }
            
            T det = default;

            ParameterExpression parameter1 = Expression.Parameter (typeof (T)),
                parameter2 = Expression.Parameter (typeof (T));

            var expressionMultiply = Expression.Multiply (parameter1, parameter2);
            var expressionSubtract = Expression.Subtract (parameter1, parameter2);
            var expressionAdd = Expression.Add (parameter1, parameter2);

            Func<T, T, T> multiply =
                Expression.Lambda<Func<T, T, T>> (expressionMultiply, parameter1, parameter2).Compile ();
            Func<T, T, T> subtract =
                Expression.Lambda<Func<T, T, T>> (expressionSubtract, parameter1, parameter2).Compile ();
            Func<T, T, T> add =
                Expression.Lambda<Func<T, T, T>> (expressionAdd, parameter1, parameter2).Compile ();

            if (rows == 2)
            {
                det = subtract (
                    (multiply (matrix[0, 0], matrix[1, 1])),
                    (multiply (matrix[0, 1], matrix[1, 0])));
            }
            else
            {
                //
                // This could be null.
                //
                T sum = default!;
                for (int j = 0; j < columns; j++)
                {
                    sum = add (sum, multiply (matrix[0, j], GetSignedMinor (0, j, matrix)));
                }
                det = sum;
            }

            return det;
        }

        public static T GetMinor<T> (int x, int y, T[,] matrix)
        {
            T[,] clone = (T[,])matrix.Clone ();
            RemoveColumn (y, ref clone);
            RemoveRow (x, ref clone);
            
            return GetDeterminant (clone);
        }

        public static T GetSignedMinor<T> (int x, int y, T[,] matrix)
        {
            int flag = x + y;
            ParameterExpression parameter = Expression.Parameter (typeof (T));

            var negateExpr = Expression.Negate (parameter);
            var negate = Expression.Lambda<Func<T, T>> (negateExpr, parameter).Compile ();
            

            if (flag.IsEven ()) return GetMinor (x, y, matrix);
            
            return negate (GetMinor (x, y, matrix));
        }

        public static Fraction GetMinor (int x, int y, Fraction[,] matrix)
        {
            var clone = (Fraction[,]) matrix.Clone ();
            RemoveColumn (y, ref clone);
            RemoveRow (x, ref clone);

            return GetDeterminant (clone);
        }

        // Co-factors 
        public static Fraction GetSignedMinor (int x, int y, Fraction[,] matrix) =>
            Convert.ToInt32 (Math.Pow (-1.0, x + y)) * GetMinor (x, y, matrix);

        public static T[,] Transpose<T> (in T[,] matrix)
        {
            int rows = matrix.GetLength (0);
            int columns = matrix.GetLength (1);

            T[,] transpose = new T[columns, rows];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    transpose[j, i] = matrix[i, j];
                }
            }
            return transpose;
        }

        public static bool IsEven (this int number)
        {
            if ((number % 2) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Just use this wisely! I removed the generic constraints (where T: struct, IEquatable<T> and IComparable<T>) 
        // to avoid complications.
        // Well, they might might not even be complications; it might just be how it's meant to be normally.
        // For example, a reference type might implement a * and + operator, which will perfectly work!
        // Except for a the null around line xxx (the default for reference types is null), so proper caution 
        // (and structures to deal with the nulls) should be taken when consuming this method. 
        // Again, this wouldn't happen with ValueTypes as their default value is not null.
        public static T[,] MultplyMatrix<T> (T[,] matrix, T[,] other)
        {
            if (matrix.GetLength (1) != other.GetLength (0))
                throw new ArgumentException ("Matrices are not compatible");

            int rows = matrix.GetLength (0);
            int columns = matrix.GetLength (1);

            int rows2 = other.GetLength (0);
            int columns2 = other.GetLength (1);

            T[,] result = new T[matrix.GetLength (0), other.GetLength (1)];

            for (int i = 0; i < rows; i++)
            {
                for (int b = 0; b < columns2; b++)
                {
                    //
                    // NOTE: THIS VALUE WILL BE NULL FOR REFERENCE TYPES AND CAN LEAD TO A NULL REFERENCE EXCEPTION (OR WRONG RESULTS)!
                    //
                    T sum = default!;

                    for (int j = 0; j < columns; j++)
                    {
                        // Multiply
                        ParameterExpression parameter1 = Expression.Parameter (typeof (T), nameof (matrix)),
                            parameter2 = Expression.Parameter (typeof (T), nameof (other));
                        BinaryExpression expression = Expression.Multiply (parameter1, parameter2);
                        Func<T, T, T> multiply = Expression.Lambda<Func<T, T, T>> (expression, parameter1, parameter2).Compile ();

                        // Here so that my nameof(product) can work 😩 (it's for consistency though!)
                        T product = multiply (matrix[i, j], other[j, b]);

                        // Add
                        ParameterExpression parameter3 = Expression.Parameter (typeof (T), nameof (sum)),
                            parameter4 = Expression.Parameter (typeof (T), nameof (product));
                        BinaryExpression body = Expression.Add (parameter3, parameter4);
                        Func<T, T, T> add = Expression.Lambda<Func<T, T, T>> (body, parameter3, parameter4).Compile ();

                        sum = add (sum, product);
                    }
                    result[i, b] = sum;
                }
            }
            return result;
        }

        // Int32 implementation
        public static int[,] MultplyMatrix (int[,] matrix, int[,] other)
        {
            if (matrix.GetLength (1) != other.GetLength (0))
                throw new ArgumentException ("Matrices are not compatible");

            int rows = matrix.GetLength (0);
            int columns = matrix.GetLength (1);

            int columns2 = other.GetLength (1);

            int[,] result = new int[matrix.GetLength (0), other.GetLength (1)];

            // foreach row in matrix
            for (int i = 0; i < rows; i++)
            {
                // And each column in other
                for (int b = 0; b < columns2; b++)
                {
                    // Multiply corresponding index (and sum all)
                    int sum = 0;
                    for (int j = 0; j < columns; j++)
                    {
                        sum += matrix[i, j] * other[j, b];
                    }
                    result[i, b] = sum;
                }
            }

            return result;
        }

        // Fraction implementation
        public static Fraction[,] MultplyMatrix (in Fraction[,] matrix, in Fraction[,] other)
        {
            if (matrix.GetLength (1) != other.GetLength (0))
                throw new ArgumentException ("Matrices are not compatible");

            int rows = matrix.GetLength (0);
            int columns = matrix.GetLength (1);

            int rows2 = other.GetLength (0);
            int columns2 = other.GetLength (1);

            Fraction[,] result = new Fraction[matrix.GetLength (0), other.GetLength (1)];

            for (int i = 0; i < rows; i++)
            {
                for (int b = 0; b < columns2; b++)
                {
                    Fraction sum = 0;
                    for (int j = 0; j < columns; j++)
                    {
                        sum += matrix[i, j] * other[j, b];
                    }
                    result[i, b] = sum;
                }
            }
            return result;
        }

        public static void Print (int[,] matrix)
        {
            int rows = matrix.GetLength (0);
            int columns = matrix.GetLength (1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Console.Write ($"{matrix[i, j]}  ");
                }
                Console.WriteLine ();
                Console.WriteLine ();
            }
        }

        //static T[] Slice<T> (this T[] array, int start = 0)
        //{
            
            
        //}

        public static void Print2 (int[,] matrix)
        {
            int rows = matrix.GetLength (0);
            int columns = matrix.GetLength (1);
            
            
        }

        public static void Print (this Fraction[,] matrix)
        {
            int rows = matrix.GetLength (0);
            int columns = matrix.GetLength (1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Console.Write ($"{matrix[i, j]}  ");
                }
                Console.WriteLine ();
                Console.WriteLine ();
            }
        }

        public static void Print (double[,] matrix)
        {
            double rows = matrix.GetLength (0);
            double columns = matrix.GetLength (1);

            for (double i = 0; i < rows; i++)
            {
                for (double j = 0; j < columns; j++)
                {
                    Console.Write ($"{matrix[(int)i, (int)j]}  ");
                }
                Console.WriteLine ();
                Console.WriteLine ();
            }
        }

        private static bool AllRowsBelowAreZero (int row, int column, int[,] matrix)
        {
            (int rows, int columns) = (matrix.GetLength (0), matrix.GetLength (1));
            int usableRows = rows - (row + 1);
            int count = 0;

            for (int i = 1; i <= usableRows; i++)
            {
                if (matrix[row + i, column] == 0)
                {
                    ++count;
                }
            }

            if (count == usableRows)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool AllRowsBelowAreZero (int row, int column, in Fraction[,] matrix)
        {
            (int rows, int columns) = (matrix.GetLength (0), matrix.GetLength (1));
            int usableRows = rows - (row + 1);
            int count = 0;

            for (int i = 1; i <= usableRows; i++)
            {
                if (matrix[row + i, column] == 0)
                {
                    ++count;
                }
            }

            if (count == usableRows)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void RemoveColumn<T> (int column, ref T[,] matrix)
        {
            (int rows, int columns) = (matrix.GetLength (0), matrix.GetLength (1));

            if (column >= columns)
                throw new ArgumentException ("The specified column is out of bound", nameof (column));

            T[,] newMatrix = new T[rows, columns - 1];
            bool flag = false;

            for (int j = 0; j < columns; j++)
            {
                if (j == column)
                {
                    flag = true;
                    continue;
                }

                for (int i = 0; i < rows; i++)
                {
                    if (flag)
                    {
                        int jCopy = j - 1;
                        newMatrix[i, jCopy] = matrix[i, j];
                    }
                    else
                    {
                        newMatrix[i, j] = matrix[i, j];
                    }
                }
            }
            try
            {
                matrix = newMatrix;
            }
            catch (Exception ex)
            {
                Console.WriteLine ($"Error: {ex?.Message}");
            }
        }

        public static void RemoveRow<T> (int row, ref T[,] matrix)
        {
            (int rows, int columns) = (matrix.GetLength (0), matrix.GetLength (1));

            if (row >= rows)
                throw new ArgumentException ("The specified row is out of bound", nameof (row));

            T[,] newMatrix = new T[rows - 1, columns];
            bool flag = false;

            for (int i = 0; i < rows; i++)
            {
                if (i == row)
                {
                    flag = true;
                    continue;
                }

                for (int j = 0; j < columns; j++)
                {
                    if (flag)
                    {
                        int iCopy = i - 1;
                        newMatrix[iCopy, j] = matrix[i, j];
                    }
                    else
                    {
                        newMatrix[i, j] = matrix[i, j];
                    }
                }
            }
            try
            {
                matrix = newMatrix;
            }
            catch (Exception ex)
            {
                Console.WriteLine ($"Error: {ex.Message}");
            }
        }

        public static bool RowIsZeroOrEmpty (int row, Fraction[,] matrix)
        {
            (int rows, int columns) = (matrix.GetLength (0), matrix.GetLength (1));

            for (int j = 0; j < columns; j++)
            {
                if (matrix[row, j] != 0)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool RowIsZero (int row, in int[,] matrix)
        {
            (int rows, int columns) = (matrix.GetLength (0), matrix.GetLength (1));

            for (int j = 0; j < columns; j++)
            {
                if (matrix[row, j] != 0)
                {
                    return false;
                }
            }
            return true;
        }

        private static List<(int x, int y)> GetPivots (ref int[,] matrix)
        {
            (int row, int column) = (matrix.GetLength (0), matrix.GetLength (1));
            List<(int x, int y)> _pivots = new List<(int x, int y)> ();

            int countX = 0, countY = 0;

            while ((countX < row) && (countY < column))
            {
                bool flag = false;
                (int x, int y) currentPivot = (countX, countY);
                while (matrix[currentPivot.x, currentPivot.y] == 0)
                {
                    bool allZero = AllRowsBelowAreZero (currentPivot.x, currentPivot.y, matrix);
                    Console.WriteLine ("Pivot is Zero");
                    Console.WriteLine (allZero);
                    if (allZero)
                    {
                        if (currentPivot.y + 1 < column)
                        {
                            currentPivot.y += 1;
                        }
                        else
                        {
                            // Row is Zero.
                            //Console.WriteLine("Row is Zero");
                            // Remove row
                            //Console.WriteLine(currentPivot.x);
                            //RemoveRow(currentPivot.x, ref matrix);

                            if ((currentPivot.x == (matrix.GetLength (0) - 1)) &&
                                (currentPivot.y == (matrix.GetLength (1) - 1)) &&
                                !RowIsZero (currentPivot.x, matrix))
                            {
                                //Console.WriteLine("here");
                                // Last row is not zero, but the last pivot is zero.
                                // Add it to the pivots anyway because it might (most likely) not be zero after row operations.
                                break;
                            }

                            flag = true;
                            break;
                        }
                    }
                    else
                    {
                        MakePivotSmallest (currentPivot, ref matrix);
                        break;
                    }
                }

                if (!flag)
                {
                    _pivots.Add (currentPivot);
                }
                countX = currentPivot.x + 1;
                countY = currentPivot.y + 1;
                Print (matrix);
                Console.WriteLine ();
            }
            return _pivots;
        }

        private static List<(int x, int y)> GetPivots (ref Fraction[,] matrix)
        {
            (int row, int column) = (matrix.GetLength (0), matrix.GetLength (1));
            List<(int x, int y)> _pivots = new List<(int x, int y)> ();

            int countX = 0, countY = 0;

            while (countX < row && countY < column)
            {
                bool flag = false;
                (int x, int y) currentPivot = (countX, countY);
                while (matrix[currentPivot.x, currentPivot.y] == 0)
                {
                    bool allBelowRowsZero = AllRowsBelowAreZero (currentPivot.x, currentPivot.y, matrix);
                    //Console.WriteLine("Pivot is Zero");
                    //Console.WriteLine(allBelowRowsZero);
                    if (allBelowRowsZero)
                    {
                        if (currentPivot.y + 1 < column)
                        {
                            currentPivot.y += 1;
                        }
                        else
                        {
                            // Row is Zero.
                            //Console.WriteLine("Row is Zero");
                            // Remove row
                            //Console.WriteLine(currentPivot.x);
                            //RemoveRow(currentPivot.x, ref matrix);

                            if ((currentPivot.x == (matrix.GetLength (0) - 1)) &&
                                (currentPivot.y == (matrix.GetLength (1) - 1)) &&
                                !RowIsZeroOrEmpty (currentPivot.x, matrix))
                            {
                                //Console.WriteLine("here");
                                // Last row is not zero, but the last pivot is zero.
                                // Add it to the pivots anyway because it might (most likely!) not be zero after row operations.
                                // What if it's not zero after row operations?
                                break;
                            }

                            flag = true;
                            break;
                        }
                    }
                    else
                    {
                        MakePivotSmallest (currentPivot, ref matrix);
                        break;
                    }
                }

                if (!flag)
                {
                    _pivots.Add (currentPivot);
                }
                countX = currentPivot.x + 1;
                countY = currentPivot.y + 1;
                //Print(matrix);
                //Console.WriteLine();
            }
            return _pivots;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static bool IsWholeNumber (object value) => value is int || value is long;

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private static bool IsDivisible (int a, int b) => a % b == 0;

        // NOTE: This doesn't make sense exactly. Better naming?
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private static bool IsDivisible (Fraction fraction, Fraction other)
        {
            // NOTE: This implementation has not yet being tested, but I believe it should work fine.
            var result = fraction / other;
            
            return result.Denuminator == 1;
        }

        private static void MakePivotSmallest ((int i, int j) pivot, ref int[,] matrix)
        {
            (int rows, int columns) = (matrix.GetLength (0), matrix.GetLength (1));
            int usableRows = rows - (pivot.i + 1);

            if (matrix[pivot.i, pivot.j] == 0)
            {
                for (int a = 1; a <= usableRows; a++)
                {
                    if (matrix[pivot.i + a, pivot.j] != 0)
                    {
                        MatrixRowOperations.ExchangeRows ((pivot.i + 1), (pivot.i + a + 1), ref matrix);
                        Console.WriteLine ($"Pivot is 0, so I'll swap with row {(pivot.i + a + 1)}");
                        // Exchange pivot with first element.
                        break;
                    }
                }
            }

            Console.WriteLine ($"Usable Rows: {usableRows}");
            for (int a = 1; a <= usableRows; a++)
            {
                Console.WriteLine ("Here!");
                if (matrix[pivot.i, pivot.j] > matrix[pivot.i + a, pivot.j] &&
                    matrix[pivot.i + a, pivot.j] != 0)
                {
                    if (matrix[pivot.i + a, pivot.j] < 0 &&
                        matrix[pivot.i, pivot.j] != 0)
                    {
                        continue;
                    }
                    MatrixRowOperations.ExchangeRows ((pivot.i + 1), (pivot.i + a + 1), ref matrix);
                    Console.WriteLine ($"Exchange: R{(pivot.i + 1)} with R{(pivot.i + a + 1)}");
                }
            }
            if (matrix[pivot.i, pivot.j] < 0)
                MakePivotNonNegative ((pivot.i, pivot.j), ref matrix);

            Print (matrix);
            Console.WriteLine ();
        }

        private static void MakePivotSmallest ((int i, int j) pivot, ref Fraction[,] matrix)
        {
            (int rows, int columns) = (matrix.GetLength (0), matrix.GetLength (1));
            int usableRows = rows - (pivot.i + 1);

            if (matrix[pivot.i, pivot.j] == 0)
            {
                for (int a = 1; a <= usableRows; a++)
                {
                    if (matrix[pivot.i + a, pivot.j] != 0)
                    {
                        MatrixRowOperations.ExchangeRows ((pivot.i + 1), (pivot.i + a + 1), ref matrix);
                        // Console.WriteLine ($"Pivot is 0, so I'll swap with row {(pivot.i + a + 1)}");
                        // Exchange pivot with first element.
                        break;
                    }
                }
            }

            Console.WriteLine ($"Usable Rows: {usableRows}");
            for (int a = 1; a <= usableRows; a++)
            {
                Console.WriteLine ("Here!");
                if (matrix[pivot.i, pivot.j] > matrix[pivot.i + a, pivot.j] &&
                    matrix[pivot.i + a, pivot.j] != 0)
                {
                    if (matrix[pivot.i + a, pivot.j] < 0 &&
                        matrix[pivot.i, pivot.j] != 0)
                    {
                        continue;
                    }
                    MatrixRowOperations.ExchangeRows ((pivot.i + 1), (pivot.i + a + 1), ref matrix);
                    Console.WriteLine ($"Exchange: R{(pivot.i + 1)} with R{(pivot.i + a + 1)}");
                }
            }
            if (matrix[pivot.i, pivot.j] < 0)
                MakePivotNonNegative ((pivot.i, pivot.j), ref matrix);

            Print (matrix);
            Console.WriteLine ();
        }

        private static void MakePivotsOne (IList<(int x, int y)> pivots, ref Fraction[,] matrix)
        {
            foreach (var (x, y) in pivots)
            {
                MakePivotNonNegative ((x, y), ref matrix);
                Fraction constant = matrix[x, y].Inverse ();
                MatrixRowOperations.MultiplyRowByK (constant, x + 1, ref matrix);
            }
        }

        private static void MakePivotNonNegative ((int i, int j) pivot, ref int[,] matrix)
        {
            if (matrix[pivot.i, pivot.j] < 0)
            {
                MatrixRowOperations.MultiplyRowByK (-1, pivot.i + 1, ref matrix);
            }
        }

        private static void MakePivotNonNegative ((int i, int j) pivot, ref Fraction[,] matrix)
        {
            if (matrix[pivot.i, pivot.j] < 0)
            {
                MatrixRowOperations.MultiplyRowByK (-1, pivot.i + 1, ref matrix);
            }
        }

        // TODO: Wrong somehow, have to check this.
        /* public static int[,] Copy(in int[,] matrix)
        {
            (int rows, int columns) = (matrix.GetLength(0), matrix.GetLength(1));
            int[,] copy = new int[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    copy[i, j] = matrix[1, j];
                }
            }
            return copy;
        } */

        private static MapOperation GetMapOperation (int pivot, int element)
        {
            if (element < 0)
            {
                return MapOperation.Add;
            }
            else if (element == 0)
            {
                return MapOperation.Skip;
            }
            else
            {
                return MapOperation.Subtract;
            }
        }

        public static Fraction[,] CloneToFraction (this int[,] matrix)
        {
            (int rows, int columns) = (matrix.GetLength (0), matrix.GetLength (1));
            Fraction[,] fraction = new Fraction[rows, columns];

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    fraction[row, column] = matrix[row, column];
                }
            }
            return fraction;
        }

        private static Sign GetSign (int pivot, int element)
        {
            if ((element < 0 && pivot < 0) || (element > 0 && pivot > 0))
            {
                return Sign.Negative;
            }
            else if (element == 0)
            {
                return Sign.Zero;
            }
            else
            {
                // element == pivot
                // element > 0
                return Sign.Negative;
            }
        }

        private static Sign GetSign (Fraction pivot, Fraction element)
        {
            if ((element < 0 && pivot < 0) || (element > 0 && pivot > 0))
            {
                return Sign.Negative;
            }
            else if (element == 0)
            {
                return Sign.Zero;
            }
            else
            {
                // element == pivot
                // element > 0
                return Sign.Negative;
            }
        }

        private static (int pivotK, int elementK) GetConstant (int pivot, int element)
        {
            int constantPivot = 1, constantElement = 1;
            if (element == 0)
                return (1, 1);

            if (element > pivot)
            {
                int k = element / pivot;
                bool isWholeNumber = IsDivisible (element, pivot);

                if (isWholeNumber)
                {
                    constantPivot = k;
                }
                else
                {
                    constantPivot = element;
                    constantElement = pivot;
                }
            }
            else if (element == pivot)
            {
                // Do nothing, K = 1 (both).
            }
            else
            {
                constantPivot = element;
                constantElement = pivot;
            }
            return (constantPivot, constantElement);
        }

        private static (Fraction pivotK, Fraction elementK) GetConstant (Fraction pivot, Fraction element)
        {
            Fraction constantPivot = 1, constantElement = 1;
            if (element == 0)
                return (1, 1);

            if (element > pivot)
            {
                Fraction k = element / pivot;
                bool isWholeNumber = IsDivisible (element, pivot); //element == pivot;

                if (isWholeNumber)
                {
                    constantPivot = k;
                }
                else
                {
                    constantPivot = element;
                    constantElement = pivot;
                }
            }
            else if (element == pivot)
            {
                // Do nothing, K = 1 (both).
            }
            else
            {
                constantPivot = element;
                constantElement = pivot;
            }
            var fraction = constantPivot / constantElement;
            //return (fraction.Numerator, fraction.Denuminator);
            return (constantPivot, constantElement);
        }

        public static void GetRowEchelonForm (ref int[,] matrix)
        {
            (int rows, int columns) = (matrix.GetLength (0), matrix.GetLength (1));
            pivot: var pivots = GetPivots (ref matrix);

            foreach (var (x, y) in pivots)
            {
                //MakePivotSmallest((x, y), ref matrix);
                if (matrix[x, y] == 0)
                {
                    MakePivotSmallest ((x, y), ref matrix);
                }
                if (matrix[x, y] == 0)
                {
                    // If pivot still == zero after switching rows,
                    // Move pivot to the next column
                    goto pivot;
                }

                if (matrix[x, y] < 0)
                {
                    MakePivotNonNegative ((x, y), ref matrix);
                }

                int elements = rows - (x + 1);
                if (elements > 0)
                {
                    for (int i = 0; i < elements; i++)
                    {
                        int pivot = matrix[x, y];
                        int element = matrix[x + i + 1, y];

                        var sign = GetSign (pivot, element);
                        var (pivotK, elementK) = GetConstant (pivot, element);
                        Console.WriteLine ($"{sign}: {pivotK}*{pivot} + {elementK}*{element})");

                        switch (sign)
                        {
                            case Sign.Positive:
                                MatrixRowOperations.MultiplyRowByK (elementK, x + i + 2, ref matrix);
                                MatrixRowOperations.AddRowsToJWithK (x + 1, x + i + 2, ref matrix, pivotK);
                                //RowOperations.MultiplyIByKThenAddToJ(constants.pivotK, x + 1, x + i + 1, ref matrix);
                                Print (matrix);
                                Console.WriteLine ();
                                break;
                            case Sign.Negative:
                                MatrixRowOperations.MultiplyRowByK (-(elementK), x + i + 2, ref matrix);
                                MatrixRowOperations.AddRowsToJWithK (x + 1, x + i + 2, ref matrix, pivotK);
                                //RowOperations.MultiplyIByKThenAddToJ(-(constants.pivotK), x + 1, x + i + 1, ref matrix);
                                Print (matrix);
                                Console.WriteLine ();
                                break;
                            case Sign.Zero:
                                // Skip
                                break;
                        }
                    }
                }
            }
        }

        public static void GetRowEchelonForm (ref Fraction[,] matrix)
        {
            (int rows, int columns) = (matrix.GetLength (0), matrix.GetLength (1));
            pivot: var pivots = GetPivots (ref matrix);

            foreach (var (x, y) in pivots)
            {
                //MakePivotSmallest((x, y), ref matrix);
                if (matrix[x, y] == 0)
                {
                    MakePivotSmallest ((x, y), ref matrix);
                }
                if (matrix[x, y] == 0)
                {
                    // If pivot still == zero after switching rows,
                    // Move pivot to the next column
                    // GetRowEchelonForm (ref matrix);
                    // return;
                    goto pivot;
                }

                if (matrix[x, y] < 0)
                {
                    MakePivotNonNegative ((x, y), ref matrix);
                }

                int elements = rows - (x + 1);
                if (elements > 0)
                {
                    for (int i = 0; i < elements; i++)
                    {
                        Fraction pivot = matrix[x, y];
                        Fraction element = matrix[x + i + 1, y];

                        var sign = GetSign (pivot, element);
                        var (pivotK, elementK) = GetConstant (pivot, element);
                        Console.WriteLine ($"{sign}: {pivotK}*{pivot} + {elementK}*{element})");

                        switch (sign)
                        {
                            case Sign.Positive:
                                MatrixRowOperations.MultiplyRowByK (elementK, x + i + 2, ref matrix);
                                MatrixRowOperations.AddRowsToJWithK (x + 1, x + i + 2, ref matrix, pivotK);
                                //RowOperations.MultiplyIByKThenAddToJ(constants.pivotK, x + 1, x + i + 1, ref matrix);
                                Print (matrix);
                                Console.WriteLine ();
                                break;
                            case Sign.Negative:
                                MatrixRowOperations.MultiplyRowByK (-(elementK), x + i + 2, ref matrix);
                                MatrixRowOperations.AddRowsToJWithK (x + 1, x + i + 2, ref matrix, pivotK);
                                //RowOperations.MultiplyIByKThenAddToJ(-(constants.pivotK), x + 1, x + i + 1, ref matrix);
                                Print (matrix);
                                Console.WriteLine ();
                                break;
                            case Sign.Zero:
                                // Skip
                                break;
                        }
                    }
                }
            }
            MakePivotsOne (pivots, ref matrix);
        }

        public static Fraction[,] GetRowEchelonForm (this Fraction[,] matrix, bool makePivotOne = true)
        {
            (int rows, int columns) = (matrix.GetLength (0), matrix.GetLength (1));
            pivot: var pivots = GetPivots (ref matrix);

            foreach (var (x, y) in pivots)
            {
                //MakePivotSmallest((x, y), ref matrix);
                if (matrix[x, y] == 0)
                {
                    MakePivotSmallest ((x, y), ref matrix);
                }
                if (matrix[x, y] == 0)
                {
                    // If pivot still == zero after switching rows,
                    // Move pivot to the next column
                    goto pivot;
                }

                if (matrix[x, y] < 0)
                {
                    MakePivotNonNegative ((x, y), ref matrix);
                }

                int elements = rows - (x + 1);
                if (elements > 0)
                {
                    for (int i = 0; i < elements; i++)
                    {
                        Fraction pivot = matrix[x, y];
                        Fraction element = matrix[x + i + 1, y];

                        var sign = GetSign (pivot, element);
                        var (pivotK, elementK) = GetConstant (pivot, element);
                        // Console.WriteLine ($"{sign}: {pivotK}*{pivot} + {elementK}*{element})");

                        switch (sign)
                        {
                            case Sign.Positive:
                                MatrixRowOperations.MultiplyRowByK (elementK, x + i + 2, ref matrix);
                                MatrixRowOperations.AddRowsToJWithK (x + 1, x + i + 2, ref matrix, pivotK);
                                // RowOperations.MultiplyIByKThenAddToJ(constants.pivotK, x + 1, x + i + 1, ref matrix);
                                Print (matrix);
                                Console.WriteLine ();
                                break;
                            case Sign.Negative:
                                MatrixRowOperations.MultiplyRowByK (-(elementK), x + i + 2, ref matrix);
                                MatrixRowOperations.AddRowsToJWithK (x + 1, x + i + 2, ref matrix, pivotK);
                                //RowOperations.MultiplyIByKThenAddToJ(-(constants.pivotK), x + 1, x + i + 1, ref matrix);
                                Print (matrix);
                                Console.WriteLine ();
                                break;
                            case Sign.Zero:
                                // Skip
                                break;
                        }
                    }
                }
            }
            if (makePivotOne)
                MakePivotsOne (pivots, ref matrix);

            return matrix;
        }

        public static void GetReducedRowEchelonForm (ref Fraction[,] matrix)
        {
            (int rows, int columns) = (matrix.GetLength (0), matrix.GetLength (1));
            pivot: var pivots = GetPivots (ref matrix);

            foreach (var (x, y) in pivots)
            {
                //MakePivotSmallest((x, y), ref matrix);
                if (matrix[x, y] == 0)
                {
                    MakePivotSmallest ((x, y), ref matrix);
                }
                if (matrix[x, y] == 0)
                {
                    // If pivot still == zero after switching rows,
                    // Move pivot to the next column, same row
                    goto pivot;
                }

                if (matrix[x, y] < 0)
                {
                    MakePivotNonNegative ((x, y), ref matrix);
                }

                int elements = rows - (x + 1);
                if (elements > 0)
                {
                    for (int i = 0; i < elements; i++)
                    {
                        Fraction pivot = matrix[x, y];
                        Fraction element = matrix[x + i + 1, y];

                        var sign = GetSign (pivot, element);
                        var (pivotK, elementK) = GetConstant (pivot, element);
                        //Console.WriteLine($"({sign}){pivotK}*{pivot} + {elementK}*{element}");

                        switch (sign)
                        {
                            case Sign.Positive:
                                MatrixRowOperations.MultiplyRowByK (elementK, x + i + 2, ref matrix);
                                MatrixRowOperations.AddRowsToJWithK (x + 1, x + i + 2, ref matrix, pivotK);
                                //RowOperations.MultiplyIByKThenAddToJ(constants.pivotK, x + 1, x + i + 1, ref matrix);
                                //Print(matrix);
                                //Console.WriteLine();
                                break;
                            case Sign.Negative:
                                MatrixRowOperations.MultiplyRowByK (-(elementK), x + i + 2, ref matrix);
                                MatrixRowOperations.AddRowsToJWithK (x + 1, x + i + 2, ref matrix, pivotK);
                                //RowOperations.MultiplyIByKThenAddToJ(-(constants.pivotK), x + 1, x + i + 1, ref matrix);
                                //Print(matrix);
                                //Console.WriteLine();
                                break;
                            case Sign.Zero:
                                // Skip
                                break;
                        }
                    }
                }
            }
            MakePivotsOne (pivots, ref matrix);

            foreach (var (x, y) in pivots)
            {
                int elements = x;

                for (int i = 0; i < elements; i++)
                {
                    Fraction element = matrix[x - (i + 1), y];
                    Fraction pivot = matrix[x, y];

                    var sign = GetSign (pivot, element);
                    var (pivotK, elementK) = GetConstant (pivot, element);

                    switch (sign)
                    {
                        case Sign.Positive:
                            MatrixRowOperations.MultiplyRowByK (elementK, x - (i + 1) + 1, ref matrix);
                            MatrixRowOperations.AddRowsToJWithK (x + 1, x - (i + 1) + 1, ref matrix, pivotK);
                            break;
                        case Sign.Negative:
                            MatrixRowOperations.MultiplyRowByK (-(elementK), x - (i + 1) + 1, ref matrix);
                            MatrixRowOperations.AddRowsToJWithK (x + 1, x - (i + 1) + 1, ref matrix, pivotK);
                            break;
                        case Sign.Zero:
                            // Row is already Zero
                            break;
                    }
                    //Console.WriteLine("MakePivotNonNegative((x, y), ref matrix);");
                }
            }
            foreach (var (a, b) in pivots)
            {
                MakePivotNonNegative ((a, b), ref matrix);
            }
        }

        public static Fraction[,] GetReducedRowEchelonForm (this Fraction[,] matrix)
        {
            (int rows, int columns) = (matrix.GetLength (0), matrix.GetLength (1));
            pivot: var pivots = GetPivots (ref matrix);

            foreach (var (x, y) in pivots)
            {
                //MakePivotSmallest((x, y), ref matrix);
                if (matrix[x, y] == 0)
                {
                    MakePivotSmallest ((x, y), ref matrix);
                }
                if (matrix[x, y] == 0)
                {
                    // If pivot still == zero after switching rows,
                    // Move pivot to the next column.
                    // This is due to reducing a whole column to zero while manipulating.
                    goto pivot;
                }

                if (matrix[x, y] < 0)
                {
                    MakePivotNonNegative ((x, y), ref matrix);
                }

                int elements = rows - (x + 1);
                if (elements > 0)
                {
                    for (int i = 0; i < elements; i++)
                    {
                        Fraction pivot = matrix[x, y];
                        Fraction element = matrix[x + i + 1, y];

                        var sign = GetSign (pivot, element);

                        // TODO: OPTIMIZE FOR LARGE NUMBERS.
                        var (pivotK, elementK) = GetConstant (pivot, element);
                        //Console.WriteLine($"({sign}){pivotK}*{pivot} + {elementK}*{element}");

                        switch (sign)
                        {
                            case Sign.Positive:
                                MatrixRowOperations.MultiplyRowByK (elementK, x + i + 2, ref matrix);
                                MatrixRowOperations.AddRowsToJWithK (x + 1, x + i + 2, ref matrix, pivotK);
                                //RowOperations.MultiplyIByKThenAddToJ(constants.pivotK, x + 1, x + i + 1, ref matrix);
                                //Print(matrix);
                                //Console.WriteLine();
                                break;
                            case Sign.Negative:
                                MatrixRowOperations.MultiplyRowByK (-(elementK), x + i + 2, ref matrix);
                                MatrixRowOperations.AddRowsToJWithK (x + 1, x + i + 2, ref matrix, pivotK);
                                //RowOperations.MultiplyIByKThenAddToJ(-(constants.pivotK), x + 1, x + i + 1, ref matrix);
                                //Print(matrix);
                                //Console.WriteLine();
                                break;
                            case Sign.Zero:
                                // Skip
                                break;
                        }
                    }
                }
            }
            MakePivotsOne (pivots, ref matrix);

            foreach (var (x, y) in pivots)
            {
                int elements = x;

                for (int i = 0; i < elements; i++)
                {
                    Fraction element = matrix[x - (i + 1), y];
                    Fraction pivot = matrix[x, y];

                    var sign = GetSign (pivot, element);
                    var (pivotK, elementK) = GetConstant (pivot, element);

                    switch (sign)
                    {
                        case Sign.Positive:
                            MatrixRowOperations.MultiplyRowByK (elementK, x - (i + 1) + 1, ref matrix);
                            MatrixRowOperations.AddRowsToJWithK (x + 1, x - (i + 1) + 1, ref matrix, pivotK);
                            break;
                        case Sign.Negative:
                            MatrixRowOperations.MultiplyRowByK (-(elementK), x - (i + 1) + 1, ref matrix);
                            MatrixRowOperations.AddRowsToJWithK (x + 1, x - (i + 1) + 1, ref matrix, pivotK);
                            break;
                        case Sign.Zero:
                            // Row is already Zero
                            break;
                    }
                    //Console.WriteLine("MakePivotNonNegative((x, y), ref matrix);");
                }
            }
            foreach (var (a, b) in pivots)
            {
                MakePivotNonNegative ((a, b), ref matrix);
            }

            return matrix;
        }

        private static Fraction[,] JoinMatrixForInverse (Fraction[,] matrix)
        {
            (int rows, int columns) = (matrix.GetLength (0), matrix.GetLength (1));
            if (rows != columns)
                throw new ArgumentException ("Must be a square matrix", nameof (matrix));

            Fraction[,] jointMatrix = new Fraction[rows, 2 * rows];
            List<(int x, int y)> pivots = new List<(int x, int y)> ();

            int countX = 0;
            int countY = rows;

            while (countX <= rows &&
                   countY <= ((rows * 2) - 1))
            {
                pivots.Add ((countX, countY));
                countX++;
                countY++;
            }

            foreach (var (x, y) in pivots)
            {
                jointMatrix[x, y] = 1;
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    jointMatrix[i, j] = matrix[i, j];
                }
            }
            return jointMatrix;
        }

        private static Fraction[,] RemoveMatrixForInverse (Fraction[,] matrix)
        {
            (int rows, int columns) = (matrix.GetLength (0), matrix.GetLength (1));
            if (columns != 2 * rows)
                throw new ArgumentException (nameof (matrix));

            Fraction[,] removedMatrix = new Fraction[rows, rows];


            //int countX = 0;
            //int countY = rows;
            //while (countX <= rows &&
            //       countY <= ((rows * 2) - 1))
            //{
            //    removedMatrix[countX, Math.Abs((columns / 2) - countY)] = matrix[countX, countY];
            //    countX++;
            //    countY++;
            //}

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if ((j >= rows) && (j <= (columns - 1)))
                    {
                        removedMatrix[i, Math.Abs ((columns / 2) - j)] = matrix[i, j];
                    }
                }
            }
            return removedMatrix;
        }

        public static Fraction[,] GetInverse (this Fraction[,] matrix)
        {
            (int rows, int columns) = (matrix.GetLength (0), matrix.GetLength (1));

            if (rows != columns)
                throw new ArgumentException ("Must be a square matrix", nameof (matrix));

            var joint = JoinMatrixForInverse (matrix);

            GetReducedRowEchelonForm (ref joint);
            // joint is now in Reduced Row Echelon Form (Hopefully 😂)

            // Remove inverse from joint matrix.
            var inverse = RemoveMatrixForInverse (joint);

            return inverse;
        }
    }
}