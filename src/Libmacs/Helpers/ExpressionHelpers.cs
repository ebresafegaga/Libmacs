using MathAPI.MathTypes;

namespace MathAPI.Helpers
{
    public static class ExpressionHelpers
    {
        public static CubicEquation AsEquation(this CubicExpression expression) => 
            new CubicEquation(expression.Terms.a, expression.Terms.b,
                (Fraction)expression.Terms.c!, (Fraction)expression.Terms.d!, 0);

        public static QuadraticEquation AsEquation(this QuadraticExpression expression) => 
            new QuadraticEquation(expression.Terms.a, expression.Terms.b, 
                (Fraction)expression.Terms.c!, 0);
        
        public static bool IsConstant(this LinearExpression expression)
        {
            var (a, b, _, _) = expression.Terms;

            if (a.Numerator == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsConstant(this QuadraticExpression expression)
        {
            var (a, b, _, _) = expression.Terms;

            if (a.Numerator == 0 && b.Numerator == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsConstant(this CubicExpression expression)
        {
            var (a, b, c, d) = expression.Terms;

            if (a.Numerator == 0 && b.Numerator == 0 && ((Fraction)c!).Numerator == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
