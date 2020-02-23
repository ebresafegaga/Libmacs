#pragma warning disable CS0660
#pragma warning disable CS0661

namespace MathAPI.MathTypes
{
    public sealed class LinearEquation : Equation
    {
        public LinearEquation (Fraction rhs, Fraction constant, params Fraction[] unknowns)
        {
            _rhs = rhs;
            _constant = constant;
            _coefficient = Simplify(unknowns);

            _terms.Add(_rhs);
            _terms.Add(_constant);
            _terms.Add(_coefficient);
        }

        public Fraction Solve()
        {
            Fraction num = _rhs - (_constant);
            Fraction ans = num / _coefficient;
            return ans;
        }

        private Fraction Simplify(params Fraction[] unknowns)
        {
            Fraction sum = default;
            foreach (Fraction item in unknowns)
            {
                sum += item;
            }
            return sum;
        }

        public override string ToString()
        {
            string sign = string.Empty;
            if (_constant < 0)
            {
                sign = "-";
            }
            else
            {
                sign = "+";
            }
            return $"{_coefficient}{CoefficientChar} {sign} {_constant} = {_rhs}";
        }
    }
}
