using MathAPI.Helpers;
using MathAPI.MathTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;


namespace MathAPI.ConsoleDriver
{
    public static class MyLinqExtensions
    {
        public static void Iter<T> (this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action (item);
        }
    }
    
    public class Program
    {
        public static void Main ()
        {
            //            var f1 = new Fraction (-3, 2);
//            Matrix<Fraction> L = new[,]
//            {
//                {1, 0, 0},
//                {-3, 1, 0},
//                {2, f1, 1}
//            };
//
//            var z = Fraction.Zero;
//            Matrix<Fraction> U = new[,]
//            {
//                {1, 2, -3},
//                {0, 7, 4},
//                {z, 0, 7}
//            };
//
//            var product = (L * U).ToArray ().GetRowEchelonForm (false);
//            product.Print ();
//
//            Console.WriteLine (U[1, 2]);
            
            Matrix<Fraction> mat = new Fraction[,] 
            {
                { 3, 9, 4 },
                { 2, 1, 2 },
                { 2, 7, 5 }
            };

            var v = mat.GetEigenValues ();
            foreach (var value in mat.GetEigenValues ()) 
                Console.WriteLine (mat);

//            var fsharp = typeof (ArrayModule).Assembly;
//
//            FSharpList<int> empty = FSharpList<int>.Empty;
//            empty.Append (12).Append (34).Append (12);
//            
//            uint b = 0b11111010;
//            var pop = Popcnt.PopCount (b);
//            Console.WriteLine ($"{pop} set bits in {b:X}");
//
//            var assembly = typeof (string).Assembly;
//            assembly.GetTypes ().Where (type => type.GetCustomAttributes ()
//                                    .Any (a => a is CLSCompliantAttribute))
//                                    .Iter (type => Console.WriteLine (type.FullName));
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static unsafe void WriteGenericToPtr<T> (IntPtr destination, T value, int size) where T : struct
        {
            byte* bytePtr = (byte*) destination;

            TypedReference valueRef = __makeref (value);
            byte* valuePtr = (byte*) *((IntPtr*) &valueRef);

            for (int i = 0; i < size; i++)
            {
                bytePtr[i] = valuePtr[i];
            }
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static unsafe T ReadGenericFromPtr<T> (IntPtr source, int size) where T : struct
        {
            var bytePtr = (byte*) source;

            T value = default;
            var valueRef = __makeref (value);

            var temp = (byte*) (*(IntPtr*) &valueRef);

            for (int i = 0; i < size; i++)
            {
                temp[i] = bytePtr[i];
            }

            return value;
        }
    }
}