using MathAPI.MathTypes;
using System;

namespace MathAPI.Helpers
{
    /// <summary>
    /// Contains methods to perform the basic row operations of a matrix.
    /// </summary>
    public static class MatrixRowOperations
    {
        /// <summary>
        /// Exchange two rows in a given matrix.
        /// </summary>
        /// <param name="i">Row 1</param>
        /// <param name="j">Row 2</param>
        /// <param name="matrix">The matrix to perform the row operation on</param>
        /// <returns>Returns a new matrix</returns>
        public static int[,] ExchangeRows(int i, int j, in int[,] matrix)
        {
            int row1 = i - 1;
            int row2 = j - 1;

            int[,] copy = matrix;

            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);
            
            int[] buffer = new int[columns];

            for (int a = 0; a < columns; a++)
            {
                buffer[a] = copy[row1, a];
                copy[row1, a] = copy[row2, a];
                copy[row2, a] = buffer[a];
            }
            
            return copy;
        }
        /// <summary>
        /// Exchange two rows in a given matrix.
        /// </summary>
        /// <param name="i">Row 1</param>
        /// <param name="j">Row 2</param>
        /// <param name="matrix">The matrix to perform the row operation on</param>
        /// <param name="z">Optional and pretty useless argument (for a hack!)</param>
        public static void ExchangeRows(int i, int j, ref int[,] matrix, int? z = null)
        {
            int row1 = i - 1;
            int row2 = j - 1;

            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            try
            {
                int[] buffer = new int[columns];

                for (int a = 0; a < columns; a++)
                {
                    buffer[a] = matrix[row1, a];
                    matrix[row1, a] = matrix[row2, a];
                    matrix[row2, a] = buffer[a];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static void ExchangeRows(int i, int j, ref Fraction[,] matrix, int? z = null)
        {
            int row1 = i - 1;
            int row2 = j - 1;

            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            try
            {
                Fraction[] buffer = new Fraction[columns];

                for (int a = 0; a < columns; a++)
                {
                    buffer[a] = matrix[row1, a];
                    matrix[row1, a] = matrix[row2, a];
                    matrix[row2, a] = buffer[a];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Multiplies a given row of a matrix by a constant.
        /// </summary>
        /// <param name="constant">Constant to muliply a row by</param>
        /// <param name="i">The row to perform the multiplication</param>
        /// <param name="matrix">The matrix to perform the multiplication</param>
        /// <returns>Returns a new matrix with the operation performed.</returns>
        public static int[,] MultiplyRowByK(int constant, int i, in int[,] matrix)
        {
            int[,] copy = matrix;
            int row = i - 1;

            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            try
            {
                for (int a = 0; a < columns; a++)
                {
                    copy[row, a] *= constant;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return copy;
        }
        /// <summary>
        /// Multiplies a given row of a matrix by a constant.
        /// </summary>
        /// <param name="constant">Constant to muliply a row by</param>
        /// <param name="i">The row to perform the multiplication</param>
        /// <param name="matrix">The matrix to perform the multiplication</param>
        /// <param name="z">Optional and pretty useless argument (for a hack!)</param>
        public static void MultiplyRowByK(int constant, int i, ref int[,] matrix, int? z = null)
        {
            int row = i - 1;

            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            try
            {
                for (int a = 0; a < columns; a++)
                {
                    matrix[row, a] *= constant;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static void MultiplyRowByK(Fraction constant, int i, ref Fraction[,] matrix, int? z = null)
        {
            int row = i - 1;

            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            try
            {
                for (int a = 0; a < columns; a++)
                {
                    matrix[row, a] *= constant;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Multiplies a row by a constant and adds it to another row.
        /// </summary>
        /// <param name="constant">The constant to multiply by</param>
        /// <param name="i">Row 1</param>
        /// <param name="j">Row 2</param>
        /// <param name="matrix">The matrix to perform the operation on</param>
        /// <returns>Returns a new matrix with the operations performed</returns>
        public static int[,] MultiplyIByKThenAddToJ(int constant, int i, int j, in int[,] matrix)
        {
            int[,] copy = matrix;
            int rowFrom = i - 1;
            int rowTo = j - 1;

            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            try
            {
                int[,] bufferMatrix = MultiplyRowByK(constant, i, matrix);
                for (int a = 0; a < columns; a++)
                {
                    copy[rowTo, a] += bufferMatrix[rowFrom, a];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return copy;
        }
        /// <summary>
        /// Multiplies a row by a constant and adds it to another row.
        /// </summary>
        /// <param name="constant">The constant to multiply </param>
        /// <param name="i">Row 1</param>
        /// <param name="j">Row 2</param>
        /// <param name="matrix">The matrix to perform the operation on</param>
        /// <param name="z">Optional and pretty useless argument (for a hack!) </param>
        public static void MultiplyIByKThenAddToJ(int constant, int i, int j, ref int[,] matrix, int? z = null)
        {
            int[,] copy = (int[,]) matrix.Clone();
            int rowFrom = i - 1;
            int rowTo = j - 1;

            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            try
            {
                int[,] bufferMatrix = MultiplyRowByK(constant, i, copy!);
                for (int a = 0; a < columns; a++)
                {
                    matrix[rowTo, a] += bufferMatrix[rowFrom, a];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static void MapFunction(int constantPivot, int contantElement, 
            int i, int j, ref int[,] matrix, MapOperation operation)
        {
            (int rows, int columns) = (matrix.GetLength(0), matrix.GetLength(1));
            int rowFrom = i - 1;
            int rowTo = j - 1;
            int[,] buffer = (int[,]) matrix.Clone();

            MultiplyRowByK(constantPivot, i, ref buffer!);
            MultiplyRowByK(contantElement, j, ref buffer);

            switch (operation)
            {
                case MapOperation.Add:
                    AddRowsToJ(i, j, ref buffer);
                    TrensferRow(j, j, buffer, ref matrix);
                    break;
                case MapOperation.Subtract:
                    SubtractRowsToJ(i, j, ref buffer);
                    TrensferRow(j, j, buffer, ref matrix);
                    break;
                case MapOperation.Skip:
                    // Skip
                    break;
            }
        }

        public static void TrensferRow(int i, int j, in int[,] matrixI, ref int[,] matrixJ)
        {
            if (matrixI.Rank != matrixJ.Rank)
            {
                throw new ArgumentException("Matrices must be of the same rank");
            }
            (int rows, int columns) = (matrixI.GetLength(0), matrixI.GetLength(1));
            int rowFrom = i - 1;
            int rowTo = j - 1;

            for (int a = 0; a < columns; a++)
            {
                matrixJ[rowTo, a] = matrixI[rowFrom, a];
            }
        }

        public static void AddRowsToJ(int i, int j, ref int[,] matrix)
        {
            (int rows, int columns) = (matrix.GetLength(0), matrix.GetLength(1));
            int rowFrom = i - 1;
            int rowTo = j - 1;

            for (int a = 0; a < columns; a++)
            {
                matrix[rowTo, a] += matrix[rowFrom, a];
            }
        }

        public static void AddRowsToJWithK(int i, int j, ref int[,] matrix, int constant)
        {
            (int rows, int columns) = (matrix.GetLength(0), matrix.GetLength(1));
            int rowFrom = i - 1;
            int rowTo = j - 1;

            for (int a = 0; a < columns; a++)
            {
                matrix[rowTo, a] += matrix[rowFrom, a] * constant;
            }
        }

        public static void AddRowsToJWithK(int i, int j, ref Fraction[,] matrix, Fraction constant)
        {
            (int rows, int columns) = (matrix.GetLength(0), matrix.GetLength(1));
            int rowFrom = i - 1;
            int rowTo = j - 1;

            for (int a = 0; a < columns; a++)
            {
                matrix[rowTo, a] += matrix[rowFrom, a] * constant;
            }
        }

        public static void SubtractRowsToJ(int i, int j, ref int[,] matrix)
        {
            (int rows, int columns) = (matrix.GetLength(0), matrix.GetLength(1));
            int rowFrom = i - 1;
            int rowTo = j - 1;

            for (int a = 0; a < columns; a++)
            {
                matrix[rowTo, a] = matrix[rowFrom, a] - matrix[rowTo, a];
            }
        }
    }
}
