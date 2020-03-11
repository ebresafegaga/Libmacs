using MathAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS0660
#pragma warning disable CS0661

namespace MathAPI.MathTypes
{
    // TODO: Implement GetHashCode() and object.Equals();
    public class ExpressionTerms : IEquatable<ExpressionTerms>
    {
        private protected List<Fraction> _terms = new List<Fraction> ();

        private protected ExpressionTerms (Fraction A, Fraction B, Fraction? C, Fraction? D) =>
            Terms = (A, B, C, D);

        public (Fraction a, Fraction b, Fraction? c, Fraction? d) Terms { get; }
        public Fraction Constant { get; set; }


        public static char CoefficientChar { get; set; } = 'x';

        public bool Equals (ExpressionTerms other)
        {
            int count = 0;
            bool flag = true;
            foreach (Fraction item in other._terms)
            {
                if (_terms[count] == item)
                {
                    // Do nothing, flag = true
                }
                else
                {
                    flag = false;
                    break;
                }
                count++;
            }
            return flag;
        }

        public static bool operator == (ExpressionTerms expression, ExpressionTerms other) =>
            expression.Equals (other);
        public static bool operator != (ExpressionTerms expression, ExpressionTerms other) =>
            !(expression == other);
        public static ExpressionTerms operator + (ExpressionTerms expression, ExpressionTerms other)
        {
            //
            // Could be null
            // 
            ExpressionTerms expr = default!;

            //
            // Hack: To handle null's in MultplyMatrix<T>(T[,] matrix, T[,] other)
            //
            if ((object)expression == null)
            {
                //
                // TODO: Use switch case for pattern matching
                //  
                if (other is LinearExpression)
                {
                    expression = new LinearExpression (0, 0);
                }
                else if (other is QuadraticExpression)
                {
                    expression = new QuadraticExpression (0, 0, 0);
                }
                else if (other is CubicExpression)
                {
                    expression = new CubicExpression (0, 0, 0, 0);
                }
            }

            // TODO: add more null checks

            // Linear combinations
            if (expression is LinearExpression a && other is LinearExpression b)
            {
                expr = a + b;
            }
            else if (expression is LinearExpression a2 && other is QuadraticExpression b2)
            {
                expr = a2 + b2;
            }
            else if (expression is LinearExpression a3 && other is CubicExpression b3)
            {
                expr = a3 + b3;
            }
            // Quadratic combinations
            else if (expression is QuadraticExpression a4 && other is QuadraticExpression b4)
            {
                expr = a4 + b4;
            }
            else if (expression is QuadraticExpression a5 && other is LinearExpression b5)
            {
                expr = b5 + a5;
            }
            else if (expression is QuadraticExpression a6 && other is CubicExpression b6)
            {
                expr = a6 + b6;
            }
            // Cubic combinations
            else if (expression is CubicExpression a7 && other is CubicExpression b7)
            {
                expr = a7 + b7;
            }
            else if (expression is CubicExpression a8 && other is LinearExpression b8)
            {
                expr = b8 + a8;
            }
            else if (expression is CubicExpression a9 && other is QuadraticExpression b9)
            {
                expr = b9 + a9;
            }

            return expr;
        }

        public static ExpressionTerms operator - (ExpressionTerms expression, ExpressionTerms other)
        {
            return expression + (-other);
        }

        public static ExpressionTerms operator - (ExpressionTerms expression)
        {
            ExpressionTerms ex = default!;
            if (expression is LinearExpression linear)
            {
                ex = (-linear);
            }
            else if (expression is QuadraticExpression quadratic)
            {
                ex = (-quadratic);
            }
            else if (expression is CubicExpression cubic)
            {
                ex = (-cubic);
            }

            return ex;
        }

        public static ExpressionTerms operator * (ExpressionTerms expression, ExpressionTerms other)
        {
            ExpressionTerms ex = default!;

            // Linear combinations
            if (expression is LinearExpression a && other is LinearExpression b)
            {
                ex = a * b;
            }
            else if (expression is LinearExpression a2 && other is QuadraticExpression b2)
            {
                ex = a2 * b2;
            }
            // Quadratic combinations
            else if (expression is QuadraticExpression a5 && other is LinearExpression b5)
            {
                ex = b5 * a5;
            }
            else
            {
                // LinearExpression * CubicExpression

                // QuadraticExpression * QuadraticExpression
                // QuadraticExpression * CubicExpression

                // CubicExpression * CubicExpression
                // CubicExpression * LinearExpression
                // CubicExpression * QuadraticExpression

                throw new NotImplementedException ("Polynomial of degree 4 and above have not yet been implemented");
            }

            return ex;
        }


        //
        // C# 8 for the win!
        //
        public Equation AsEquation () => this switch 
        {
            LinearExpression a1    => new LinearEquation (0, a1.Terms.b, a1.Terms.a), 
            QuadraticExpression a2 => new QuadraticEquation (a2.Terms.a, a2.Terms.b, (Fraction)a2.Terms.c!, 0), 
            CubicExpression a3     => new CubicEquation (a3.Terms.a, a3.Terms.b, (Fraction)a3.Terms.c!, (Fraction)a3.Terms.d!, 0),
            _                      => new LinearEquation (0, 0, 0) // I'll take this as null.
        };

        // Depreciated code.
        // public Equation AsEquation ()
        // {
        //     if (this is LinearExpression a1)
        //     {
        //         return new LinearEquation (0, a1.Terms.b, a1.Terms.a);
        //     }
        //     else if (this is QuadraticExpression a2)
        //     {
        //         return new QuadraticEquation (a2.Terms.a, a2.Terms.b, (Fraction)a2.Terms.c!, 0);
        //     }
        //     else if (this is CubicExpression a3)
        //     {
        //         return new CubicEquation (a3.Terms.a, a3.Terms.b, (Fraction)a3.Terms.c!, (Fraction)a3.Terms.d!, 0);
        //     }

        //     // We should never get here.
        //     Debug.Assert (false);
        //     return null;
        // }
    }

    public class LinearExpression : ExpressionTerms, IEquatable<LinearExpression>
    {
        public LinearExpression (Fraction a, Fraction b) : base (a, b, null, null)
        {
            Constant = b;
        }

        public static LinearExpression operator + (LinearExpression expression, LinearExpression other)
        {
            var a = expression.Terms.a + other.Terms.a;
            var b = expression.Terms.b + other.Terms.b;
            return new LinearExpression (a, b);
        }

        public static QuadraticExpression operator + (LinearExpression expression, QuadraticExpression other)
        {
            var a = other.Terms.a;
            var b = expression.Terms.a + other.Terms.b;
            var c = expression.Terms.b + (Fraction)other.Terms.c!;

            return new QuadraticExpression (a, b, c);
        }

        public static CubicExpression operator + (LinearExpression expression, CubicExpression other)
        {
            var a = other.Terms.a;
            var b = other.Terms.b;
            var c = expression.Terms.a + (Fraction)other.Terms.c!;
            var d = expression.Terms.b + (Fraction)other.Terms.d!;

            return new CubicExpression (a, b, c, d);
        }

        public static LinearExpression operator - (LinearExpression expression, LinearExpression other)
        {
            var a = expression.Terms.a - other.Terms.a;
            var b = expression.Terms.b - other.Terms.b;
            return new LinearExpression (a, b);
        }

        public static LinearExpression operator - (LinearExpression expression)
        {
            var a = (-expression.Terms.a);
            var b = (-expression.Terms.b);

            return new LinearExpression (a, b);
        }

        public static QuadraticExpression operator * (LinearExpression expression, LinearExpression other)
        {
            var a = expression.Terms.a * other.Terms.a;
            var b = (expression.Terms.a * other.Terms.b) + (expression.Terms.b * other.Terms.a);
            var c = expression.Terms.b * other.Terms.b;
            return new QuadraticExpression (a, b, c);
        }

        public static CubicExpression operator * (LinearExpression expression, QuadraticExpression other)
        {
            var a = expression.Terms.a * other.Terms.a;
            var b = (expression.Terms.a * other.Terms.b) + (other.Terms.a * expression.Terms.b);
            var c = (Fraction)((expression.Terms.a * other.Terms.c) + (expression.Terms.b * other.Terms.b))!;
            var d = (Fraction)(expression.Terms.b * other.Terms.c)!;
            return new CubicExpression (a, b, c, d);
        }

        public static implicit operator LinearExpression (Fraction value)
        {
            return new LinearExpression (0, value);
        }

        public static implicit operator LinearExpression (int value)
        {
            return new LinearExpression (0, value);
        }

        public override string ToString ()
        {
            return $"({Terms.a}{CoefficientChar} {Terms.b})";
        }

        public bool Equals (LinearExpression other)
        {
            return base.Equals (other);
        }
    }

    public class QuadraticExpression : ExpressionTerms
    {
        public QuadraticExpression (Fraction a, Fraction b, Fraction c) : base (a, b, c, null)
        {
            Constant = c;
        }

        public static QuadraticExpression operator + (QuadraticExpression expression, QuadraticExpression other)
        {
            var a = expression.Terms.a + other.Terms.a;
            var b = expression.Terms.b + other.Terms.b;
            var c = (Fraction)(expression.Terms.c + other.Terms.c)!;
            return new QuadraticExpression (a, b, c);
        }

        public static CubicExpression operator + (QuadraticExpression expression, CubicExpression other)
        {
            var a = other.Terms.a;
            var b = expression.Terms.a + other.Terms.b;
            var c = expression.Terms.b + (Fraction)other.Terms.c!;
            var d = (Fraction)expression.Terms.c! + (Fraction)other.Terms.d!;

            return new CubicExpression (a, b, c, d);
        }

        public static QuadraticExpression operator - (QuadraticExpression expression, QuadraticExpression other)
        {
            var a = expression.Terms.a - other.Terms.a;
            var b = expression.Terms.b - other.Terms.b;
            var c = (Fraction)(expression.Terms.c - other.Terms.c)!;
            return new QuadraticExpression (a, b, c);
        }

        public static QuadraticExpression operator - (QuadraticExpression expression)
        {
            var a = (-expression.Terms.a);
            var b = (-expression.Terms.b);
            var c = (-((Fraction)expression.Terms.c!));

            return new QuadraticExpression (a, b, c);
        }

        public static QuadraticExpression operator - (QuadraticExpression expression, LinearExpression other)
        {
            var a = expression.Terms.a - other.Terms.a;
            var b = expression.Terms.b - other.Terms.b;
            var c = (Fraction)expression.Terms.c!;
            return new QuadraticExpression (a, b, c);
        }

        // TODO: operator *
        //public static QuadraticExpression operator *(LinearExpression expression, LinearExpression other)
        //{
        //    throw new NotImplementedException();
        //}

        public static CubicExpression operator * (QuadraticExpression expression, LinearExpression other)
        {
            return other * expression;
        }

        public static (LinearExpression quotient, Fraction remainder) operator /
            (QuadraticExpression expression, LinearExpression other)
        {
            Fraction a = default;
            Fraction b = default;
            LinearExpression expression2 = default!; // Cleary, this could be null

            //
            // This polynmial division is primitive. A more roboust solution can be found at PolynomialHelpers.cs
            //
            int count = 1;
            do
            {
                switch (count)
                {
                    case 1:
                        a = expression.Terms.a / other.Terms.a;
                        QuadraticExpression semi1 = new QuadraticExpression (a * other.Terms.a, a * other.Terms.b, 0);
                        expression -= semi1;
                        expression2 = new LinearExpression (expression.Terms.b, (Fraction)expression.Terms.c!);
                        break;
                    case 2:
                        b = expression2.Terms.a / other.Terms.a;
                        LinearExpression semi2 = new LinearExpression (b * other.Terms.a, b * other.Terms.b);
                        expression2 -= semi2;
                        break;
                    default:
                        // should never get here.
                        Debug.Assert (true);
                        break;
                }
                count++;
            }
            while (!expression2.IsConstant ());

            return (new LinearExpression (a, b), expression2.Constant);
        }

        public static implicit operator QuadraticExpression (Fraction fraction)
        {
            return new QuadraticExpression (0, 0, fraction);
        }

        public static implicit operator QuadraticExpression (int value)
        {
            return new QuadraticExpression (0, 0, value);
        }

        public override string ToString ()
        {
            //
            // TODO:
            //
            if (Terms.b > 0)
            {

            }
            return $"{Terms.a}{CoefficientChar}2 {Terms.b}{CoefficientChar} {Terms.c}";
        }
    }

    public class CubicExpression : ExpressionTerms
    {
        public CubicExpression (Fraction a, Fraction b, Fraction c, Fraction d) : base (a, b, c, d)
        {
            Constant = d;
        }

        public static CubicExpression operator + (CubicExpression expression, CubicExpression other)
        {
            var a = expression.Terms.a + other.Terms.a;
            var b = expression.Terms.b + other.Terms.b;
            var c = (Fraction)(expression.Terms.c! + other.Terms.c!);
            var d = (Fraction)(expression.Terms.d! + other.Terms.d!);
            return new CubicExpression (a, b, c, d);
        }

        public static CubicExpression operator - (CubicExpression expression, CubicExpression other)
        {
            var a = expression.Terms.a - other.Terms.a;
            var b = expression.Terms.b - other.Terms.b;
            var c = (Fraction)(expression.Terms.c! - other.Terms.c!);
            var d = (Fraction)(expression.Terms.d! - other.Terms.d!);
            return new CubicExpression (a, b, c, d);
        }

        public static CubicExpression operator - (CubicExpression expression)
        {
            var a = (-expression.Terms.a);
            var b = (-expression.Terms.b);
            var c = (-((Fraction)expression.Terms.c!));
            var d = (-((Fraction)expression.Terms.d!));

            return new CubicExpression (a, b, c, d);
        }

        public static CubicExpression operator - (CubicExpression expression, QuadraticExpression other)
        {
            var a = expression.Terms.a - other.Terms.a;
            var b = expression.Terms.b - other.Terms.b;
            var c = (Fraction)(expression.Terms.c! - other.Terms.c!);
            var d = (Fraction)expression.Terms.d!;
            return new CubicExpression (a, b, c, d);
        }

        // TODO: operator *
        //public static QuadraticExpression operator *(LinearExpression expression, LinearExpression other)
        //{
        //    throw new NotImplementedException();
        //}

        public static (QuadraticExpression quotient, Fraction reminder) operator /
            (CubicExpression expression, LinearExpression other)
        {
            Fraction a = default;
            Fraction b = default;
            Fraction c = default;
            QuadraticExpression expression2 = default!;
            // Just so the while loop can work
            LinearExpression expression3 = new LinearExpression (1, 2);

            int count = 1;
            do
            {
                switch (count)
                {
                    case 1:
                        a = expression.Terms.a / other.Terms.a;
                        CubicExpression semi1 = new CubicExpression (a * other.Terms.a, a * other.Terms.b, 0, 0);
                        expression -= semi1;
                        expression2 = new QuadraticExpression (expression.Terms.b, (Fraction)expression.Terms.c!, (Fraction)expression.Terms.d!);
                        break;
                    case 2:
                        b = expression2.Terms.a / other.Terms.a;
                        LinearExpression semi2 = new LinearExpression (b * other.Terms.a, b * other.Terms.b);
                        expression2 -= semi2;
                        expression3 = new LinearExpression (expression2.Terms.b, (Fraction)expression2.Terms.c!);
                        break;
                    case 3:
                        c = expression3.Terms.a / other.Terms.a;
                        LinearExpression semi3 = new LinearExpression (c * other.Terms.a, c * other.Terms.b);
                        expression3 -= semi3;
                        break;
                    default:
                        // Should never get here.
                        Debug.Assert (true);
                        break;
                }
                count++;
            }
            while (!expression3.IsConstant ());

            return (new QuadraticExpression (a, b, c), expression3.Constant);
        }

        public Fraction Substitute (Fraction x)
        {
            var cube = (int)Math.Pow (x.Numerator, 3) * (int)Terms.a.Numerator;
            var square = (int)Math.Pow (x.Numerator, 2) * (int)Terms.b.Numerator;

            return cube + square + (Fraction)(x * Terms.c!) + (Fraction)Terms.d!;
        }

        public static implicit operator CubicExpression (Fraction fraction)
        {
            return new CubicExpression (0, 0, 0, fraction);
        }

        public static implicit operator CubicExpression (int value)
        {
            return new CubicExpression (0, 0, 0, value);
        }

        public override string ToString ()
        {
            var (a, b, c, d) = Terms;
            string bSign, cSign, dSign;

            if (b.Numerator < 0)
            {
                //negative 
                bSign = "-";
            }
            else
            {
                //positive
                bSign = "+";
            }

            if (((Fraction)c!).Numerator < 0)
            {
                //negative 
                cSign = "-";
            }
            else
            {
                //positive
                cSign = "+";
            }

            if (((Fraction)d!).Numerator < 0)
            {
                //negative 
                dSign = "-";
            }
            else
            {
                //positive
                dSign = "+";
            }

            return $"{a}x3 {bSign} {b.Abs ()}x2 {cSign} {((Fraction)c).Abs ()}x {dSign} {((Fraction)d).Abs ()}";

        }
    }
}
