using MathAPI.MathTypes;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MathAPI.UnitTests.MathTypes
{
    public partial class FractionTests
    {
        [Fact]
        public void Ctor_LongLong_WhenDenumimatorIsZero_ThrowsArgumentException ()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentException> ("denum", 
                () => new Fraction (5, 0));
        }

        [Fact]
        public void Ctor_LongLong_WhenNumeratorIsZero_ValueIsZero ()
        {
            // Arrange
            var fraction = new Fraction (0, 5);

            // Act 
            var actual = fraction.Value;

            // Assert
            Assert.Equal (0, actual);
        }

        [Theory]
        [InlineData (1, -3, -1, 3)]
        [InlineData (23, -32, -23, 32)]
        [InlineData (2341, -10000, -2341, 10000)]
        public void Ctor_LongLong_WhenDenuminatorIsNegative_TransfersSignToNumerator (int num, int denum, int exNum, int exDenum)
        {
            // Arrange 
            var fraction = new Fraction (num, denum);

            // Act
            var actualNum = fraction.Numerator;
            var actualDenum = fraction.Denuminator;

            // Assert
            Assert.Equal (exNum, actualNum);
            Assert.Equal (exDenum, actualDenum);
        }

        [Theory]
        [InlineData (6, 18, 1, 3)]
        [InlineData (6, -8, -3, 4)]
        [InlineData (16, 72, 2, 9)]
        [InlineData (55, 25, 11, 5)]
        [InlineData (4, 25, 4, 25)]
        public void Ctor_LongLong_ByDefault_ReducesNumAndDenumToLowestTerms (int num, int denum, int exNum, int exDenum)
        {
            // Arrange 
            var fraction = new Fraction (num, denum);

            // Act
            var actualNum = fraction.Numerator;
            var actualDenum = fraction.Denuminator;

            // Assert
            Assert.Equal (exNum, actualNum);
            Assert.Equal (exDenum, actualDenum);
        }
        
        [Theory]
        [InlineData (1, 2, 2, 1)]
        [InlineData (4, 5, 5, 4)]
        [InlineData (-22, 6, 6, -22)]
        [InlineData (1, 1, 1, 1)]
        public void Inverse_WhenCalled_ReturnsInverseOfFraction(int num, int denum, int exNum, int exDenum)
        {
            // Arrange 
            var fraction = new Fraction (num, denum);
            var expected = new Fraction (exNum, exDenum);
            
            // Act
            var inverse = fraction.Inverse ();

            // Assert
            Assert.Equal (expected, inverse);
        }

        [Theory]
        [MemberData (nameof (GetFractionAddTestData))]
        public void OpAddition_WhenCalled_ReturnsSum (Fraction f1, Fraction f2, Fraction expected)
        {
            // Act
            var actual = f1 + f2;
            
            // Assert
            Assert.Equal (expected, actual);
        }

        [Theory]
        [InlineData (1, 2, 1, 2, true)]
        [InlineData (1, 2, 1, 4, false)]
        [InlineData (4, 5, 4, 5, true)]
        [InlineData (13, 2, 1, 3, false)]
        public void Equals_ByDefault_ReturnsTrueIfEqualOrFalseOtherwise (int n1, int d1, int n2, int d2, bool result)
        {
            // Arrange 
            var f1 = new Fraction (n1, d1);
            var f2 = new Fraction (n2, d2);
            
            // Act
            var actual = f1.Equals (f2);

            // Assert
            Assert.Equal (result, actual);
        }
    }
}
