using MathAPI.MathTypes;
using System;
using System.Collections.Generic;

namespace MathAPI.Helpers
{
    /*
    public static class Echelon
    {
        public static void GetRowEchelonForm(ref int[,] matrix)
        {
            (int rows, int columns) = (matrix.GetLength(0), matrix.GetLength(1));
            List<(int x, int y)> _pivots = Helpers.GetPivots(ref matrix);

            foreach (var (x, y) in _pivots)
            {
                Console.WriteLine($"We have {_pivots.Count} pivots");
                Helpers.MakePivotSmallest((x, y), ref matrix);
                Helpers.MakePivotNonNegative((x, y), ref matrix);
                int elements = rows - (x + 1);

                for (int i = 1; i <= elements; i++)
                {
                    int elementRow = x + i;
                    int pivot = matrix[x, y];
                    int element = matrix[elementRow, y];

                    var wrappedConstant = GetConstant(pivot, element);

                    int pivotK = wrappedConstant.pivotK;
                    int elementK = wrappedConstant.elementK;

                    var operation = Helpers.GetMapOperation(pivot, element);
                    RowOperations.MapFunction(pivotK, elementK, x + 1, elementRow + 1, ref matrix, operation);
                }
            }
        }

        private static (int pivotK, int elementK) GetConstant(dynamic pivot, dynamic element)
        {
            int constantPivot = 1, constantElement = 1;
            bool exchange = false;
            if (element == 0)
                return (1, 1);

            if (element > pivot)
            {
                dynamic k = element / pivot;
                bool isWholeNumber = Helpers.IsDivisible(element, pivot);

                if (isWholeNumber)
                {
                    constantPivot = k;
                    Console.WriteLine($"integer: {element} / {pivot}");
                }
                else
                {
                    constantPivot = element;
                    constantElement = pivot;
                    Console.WriteLine("Decimal");
                }
            }
            else if (element == pivot)
            {
                // Do nothing, K = 1 (both).
                Console.WriteLine("Equal");
            }
            else
            {
                exchange = true;
                Console.WriteLine("elemtn < pivot");
            }
            return (constantPivot, constantElement);
        }
    }
} */
    //public static SurdExpression operator *(SurdExpression surds, SurdExpression other)
    //{
    //    var _sum = new List<SurdExpression>();
    //    SurdExpression realSum = default;

    //    for (int i = 0; i < surds.Length; i++)
    //    {
    //        SurdExpression sum = new SurdExpression();
    //        for (int j = 0; j < other.Length; j++)
    //        {
    //            var temp = surds[i] * other[j];
    //            sum += temp;
    //        }
    //        _sum.Add(sum);
    //    }

    //    foreach (var item in _sum)
    //    {
    //        realSum += item;
    //    }

    //    return realSum;
    //}
}
