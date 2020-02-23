using MathAPI.Helpers;
using MathAPI.MathTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

#nullable enable 

namespace MathAPI.UnitTests.MathTypes
{
    public partial class FractionTests
    {
        private readonly ITestOutputHelper output;

        public FractionTests (ITestOutputHelper ot)
        {
            output = ot;
        }

        [Theory]
        [MemberData (nameof (GetFractionAbsTestData))]
        public void Abs_GivenNegativeFraction_ReturnsPositiveFraction (Fraction fraction)
        {
            // Arrange & Act
            var result = FractionHelpers.Abs (fraction);
            output.WriteLine ($"Abs ({fraction}) = {result}");

            // Assert
            Assert.True (result >= 0);
        }

        [Theory]
        [MemberData (nameof (GetFractionAddTestData))]
        public void Add_GivenTwoFractions_ReturnsTheSum (Fraction f1, Fraction f2, Fraction expected)
        {
            // Arrange & Act 
            var sum = FractionHelpers.Add (f1, f2);
            output.WriteLine ($"Add ({f1}, {f2}) = {sum}");

            // Assert
            Assert.Equal (expected, sum);
        }

        [Theory]
        [MemberData (nameof (GetFractionSubtractTestData))]
        public void Subtract_GivenTwoFractions_ReturnsTheDifference (Fraction f1, Fraction f2, Fraction expected)
        {
            // Arrange & Act 
            var sum = FractionHelpers.Subtract (f1, f2);
            output.WriteLine ($"Subtract ({f1}, {f2}) = {sum}");

            // Assert
            Assert.Equal (expected, sum);
        }

        [Theory]
        [InlineData (10, new long [] { 1, 2, 5, 10 })]
        [InlineData (20, new long[] { 1, 2, 4, 5, 10, 20 })]
        [InlineData (13, new long[] { 1, 13 })]
        [InlineData (16, new long[] { 1, 2, 4, 8, 16 })]
        [InlineData (35, new long[] { 1, 5, 7, 35 })]
        [InlineData (42, new long[] { 1, 2, 3, 6, 7, 14, 21, 42 })]
        public void GetFactors_Long_ByDefaults_ReturnsAListOfFactors (long number, long[] expected)
        {
            // Arrange & Act
            IStructuralEquatable factors = number.GetFactors ().ToArray();

            // Assert
            Assert.Equal ((IStructuralEquatable) expected, factors);
        }

        [Theory]
        [InlineData (10, new int[] { 2, 5, 10 })]
        [InlineData (20, new int[] { 2, 4, 5, 10, 20 })]
        [InlineData (13, new int[] { 13 })]
        [InlineData (16, new int[] { 2, 4, 8, 16 })]
        [InlineData (35, new int[] { 5, 7, 35 })]
        [InlineData (42, new int[] { 2, 3, 6, 7, 14, 21, 42 })]
        public void GetFactors_Int_ByDefaults_ReturnsAListOfFactors (int number, int[] expected)
        {
            // Arrange & Act
            IStructuralEquatable factors = number.GetFactors ().ToArray ();

            // Assert
            Assert.Equal ((IStructuralEquatable)expected, factors);
        }

        // NOTE: In a normal situation, private methods are not supposed to be tested 
        // as they are an implementation detail. Private methods are tested indirectly via the 
        // public methods. But I'm doing this just because I can 😁.
        [Theory]
        [InlineData (8, 4, 8)]
        [InlineData (13, 5, 65)]
        [InlineData (7, 3, 21)]
        [InlineData (8, 6, 24)]
        public void GetLCM_WhenCalled_ReturnsTheLCM (long a, long b, long expected)
        {
            // Arrange 
            var type = typeof (FractionHelpers);
            MethodInfo method = type.GetMethod ("GetLCM", BindingFlags.Static | 
                                                          BindingFlags.NonPublic | 
                                                          BindingFlags.InvokeMethod);
            // Act
            var result = (long) method.Invoke ("Ignored for static types yo!, but am I allocating? 🧐", new object[] { a, b });

            // Assert
            Assert.Equal (expected, result);
        }

        [Theory]
        [InlineData (10, 5, new long[] { 10, 20, 30, 40, 50 })]
        [InlineData (13, 7, new long[] { 13, 26, 3*13, 4*13, 5*13, 6*13, 7*13 })]
        [InlineData (17, 3, new long[] { 17, 34, 3*17 })]
        [InlineData (0, 5, new long[] { } )]
        [InlineData (1, 4, new long[] { 1, 2, 3, 4 })]
        public void GetNMultiples_WhenCalled_ReturnsTheFirstNMul (long number, long n, long[] expected)
        {
            // Arrange & Act
            var result = number.GetNMultiples (n);
            var equatable = result.ToArray () as IStructuralEquatable;

            // Assert
            Assert.Equal (expected.Length, result.Count);
            Assert.Equal ((IStructuralEquatable) expected, equatable);
        }

        public static IEnumerable<object[]> GetFractionSubtractTestData ()
        {
            yield return new object[] { new Fraction (1, 2), new Fraction (1, 2), Fraction.Zero };
            yield return new object[] { new Fraction (-1, 3), new Fraction (-1, 3), Fraction.Zero };
            yield return new object[] { Fraction.Unit, new Fraction (1, 2), new Fraction (1, 2) };
            yield return new object[] { new Fraction (2, 3), new Fraction (4, 3), new Fraction (-2, 3) };
            yield return new object[] { new Fraction (5, 6), new Fraction (1, 2), new Fraction (1, 3) };
            yield return new object[] { new Fraction (155, 5), new Fraction (1, 2), new Fraction (61, 2) };
            yield return new object[] { Fraction.Zero, new Fraction (56, 13), new Fraction (-56, 13) };
            yield return new object[] { 123, 16, 107 };
            yield return new object[] { new Fraction (-51, 2), new Fraction (51, 2),new Fraction (-102, 2) };
            yield return new object[] { -5, 5, -10 };
            yield return new object[] { -5, 3, -8 };
        }

        public static IEnumerable<object[]> GetFractionAddTestData ()
        {
            yield return new object[] { new Fraction (1, 2), new Fraction (1, 2), Fraction.Unit };
            yield return new object[] { new Fraction (-1, 3), new Fraction (-1, 3), new Fraction (-2, 3) };
            yield return new object[] { Fraction.Unit, new Fraction (1, 2), new Fraction (3, 2) };
            yield return new object[] { new Fraction (2, 3), new Fraction (4, 3), 2 };
            yield return new object[] { new Fraction (5, 6), new Fraction (1, 2), new Fraction (4, 3) };
            yield return new object[] { new Fraction (155, 5), new Fraction (1, 2), new Fraction (63, 2) };
            yield return new object[] { Fraction.Zero, new Fraction (56, 13), new Fraction (56, 13) };
            yield return new object[] { 123, 16, 139 };
            yield return new object[] { new Fraction (-51, 2), new Fraction (51, 2), Fraction.Zero };
            yield return new object[] { -5, 5, Fraction.Zero };
            yield return new object[] { -5, 3, -2 };
        }

        public static IEnumerable<object[]> GetFractionAbsTestData ()
        {
            yield return new object[] { new Fraction (-1, 3) };
            yield return new object[] { new Fraction (-6, 5) };
            yield return new object[] { new Fraction (-22, 7) };
            yield return new object[] { new Fraction (2, 13) };
            yield return new object[] { new Fraction (23, -323) };
            yield return new object[] { Fraction.Zero };
            yield return new object[] { Fraction.Unit };
        }
    }
}
