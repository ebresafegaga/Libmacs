using MathAPI.MathTypes;
using System;
using System.Collections.Generic;

namespace MathAPI.Helpers
{
    public static class LinearSystemHelpers
    {
        // If return value is zero, then the row is zero.
        // Always check for null when using this method.
        public static int? FindFirstNonZeroIndex(this Fraction[] array)
        {
            int? index = null;

            for (int i = 0; i < array.Length; i++)
            {
                index = i;
                if (array[i] != 0)
                    break;
            }

            return index;
        }

        // This method is 'buggy' due to the assumption
        // Check GetFreeVariables2 for a more a more reliable behaviour.
        [Obsolete ("This is VERY buggy, use GetFreeVariables2() instead.")]
        public static LinearSystem.FreeVariables GetFreeVariables (this Matrix<Fraction> matrix)
        {
            var freeVars = new List<int> ();
            // TODO : Create suitable extension method/member methods to avoid the .ToArray () hassle.
            var firstRow = matrix.ToArray ().GetRow (0);
            
            // NOTE: Assuming x1 cannot be a free variable. Is that wrong?
            int? index = firstRow.FindFirstNonZeroIndex ();
            int firstItem = index ?? firstRow.Length - 1;

            for (int i = (firstItem + 1); i < firstRow.Length; i++)
            {
                if (firstRow[i] != 0)
                {
                    freeVars.Add (i);
                }
            }

            return new LinearSystem.FreeVariables (freeVars);
        }

        // The matrix has to be a well defined system of equation (i.e a square matrix with a column vector (of the same number of rows) appended to it),
        // or else you'll get an undefined behaviour.
        public static LinearSystem.FreeVariables GetFreeVariables2 (this Matrix<Fraction> matrix)
        {
            var freeVars = new List<int> ();
            var array = matrix.ToArray ();

            // Add all positions to the list.
            for (int i = 0; i < array.GetLength (1); i++)
                freeVars.Add (i);

            var (rows, columns) = (array.GetLength (0), array.GetLength (1));

            // Remove from the list based on the state of the matrix.
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if ((i == j) && array[i, j] != 0)
                    {
                        freeVars.Remove (j);
                        break;
                    }

                    // Check other values in the row.
                    int? index = Check ((i, j), array);
                    if (index.HasValue)
                    {
                        int realIndex = (int) index;
                        freeVars.Remove (realIndex);
                        break;
                    }
                    
                    // current row is zero
                    // so, continue searching for free variables in the next row...
                }
            }

            // Local function.
            // Checks values to the right of the current position int a row and returns the first non-zero j-index (i.e row index), 
            // If all to the right are zero then return null.
            int? Check ((int i, int j) position, Fraction[,] A)
            {
                var (rrows, ccolumns) = (A.GetLength (0), A.GetLength (1));
                // There is really no need for this outer loop.
                var (i, j) = position;
                for (int a = i; a < rrows; a++)
                {
                    // To the right, to the right, now...
                    for (int b = (j + 1); b < ccolumns; b++)
                    {
                        if (A[a, b] != 0)
                            return b;
                    }
                    // Break once we leave the row.
                    break;
                }
                return null;
            }
            
            return new LinearSystem.FreeVariables (freeVars);
        }

        public static List<Vector<Fraction>> MatrixToVectors(this Matrix<Fraction> matrix)
        {
            var list = new List<Vector<Fraction>> ();

            for (int j = 0; j < matrix.ToArray ().GetLength (1); j++)
            {
                list.Add (matrix.ToArray ().GetColumn (j));
            }

            return list;
        }

        public static LinearSystem.SolutionSet Solve(this Matrix<Fraction> equations)
        {
            return LinearSystem.Solve (equations);
        }
    }
}