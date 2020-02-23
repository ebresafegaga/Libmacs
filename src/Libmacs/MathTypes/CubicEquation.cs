#pragma warning disable CS0660
#pragma warning disable CS0661

namespace MathAPI.MathTypes
{
    public sealed class CubicEquation : Equation
    {
        private (Fraction a, Fraction b, Fraction c, Fraction d) Terms { get; set; }

        public CubicEquation(Fraction a, Fraction b, Fraction c, Fraction d, Fraction rhs)
        {
            _rhs = rhs;
            _constant = d;
            Terms = (a, b, c, d);

            // Assuming rhs == 0, is true always.
            _terms.Add(a);
            _terms.Add(b);
            _terms.Add(c);
            _terms.Add(d);
        }

        public (Fraction x1, Fraction x2, Fraction x3) Solve()
        {
            int min = -100, max = 100;

            // first factor
            int factor = default;
            CubicExpression cubic = new CubicExpression(Terms.a, Terms.b, Terms.c, Terms.d);

            for (int i = min; i < max; i++)
            {
                if (cubic.Substitute(i) == 0)
                {
                    factor = i;
                    break;
                }
            }

            //var f = new Fraction(-factor, 1);
            LinearExpression linear = new LinearExpression(1, -factor);
            Fraction x1 = default, x2 = default;
            // rem should be == 0
            (QuadraticExpression quadratic, Fraction rem) = cubic / linear;
            if (rem == 0)
            {
                QuadraticEquation equation = new QuadraticEquation(
                    quadratic.Terms.a, 
                    quadratic.Terms.b, 
                    (Fraction)quadratic.Terms.c, 0);
                (x1, x2) = equation.Solve();
            }

            return (factor, x1, x2);
        }
    }
}
