using MathAPI.MathTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathAPI.Helpers
{
    public static class SurdHelpers
    {
        public static Surd AddCompatibleSurds(IEnumerable<Surd> surds)
        {
            if (!(surds.Count() > 0))
            {
                throw new ArgumentException("surds cannot be empty", nameof(surds));
            }

            Fraction root = surds.First().Root;
            foreach (var surd in surds)
            {
                if (surd.Root != root)
                {
                    throw new ArgumentException("Surds are not compatible", nameof(surds));
                }
            }
            
            Fraction constant = default;
            foreach (var surd in surds)
            {
                constant += surd.Constant;
            }

            return new Surd(root, constant);
        }

        public static Surd Reduce(this Surd initialSurd)
        {
            int root = (int)initialSurd.Root.Numerator;
            int constant = (int)initialSurd.Constant.Numerator;

            var surd = new Surd(root, constant);

            while (surd.IsReduceable())
            {
                var factors = ((int)surd.Root.Numerator).GetFactors();
                factors.Remove(1);
                factors.Remove((int)surd.Root.Numerator);

                Fraction perfect = default;
                Fraction other = default;
                int sqrt = default;

                // Perfect squares which don't have factors that are perfect squares.
                if (surd.Root.IsPerfectSquare())
                {
                    sqrt = (int)Math.Sqrt(surd.Root.Value);
                    surd = new Surd(1, (surd.Constant * sqrt));
                    continue;
                }

                foreach (Fraction item in factors)
                {
                    // Get the first perfect square.
                    if (item.IsPerfectSquare())
                    {
                        perfect = item;
                        other = root / perfect;
                        sqrt = (int)Math.Sqrt(perfect.Value);
                        break;
                    }
                }
                surd = new Surd(other, (surd.Constant * sqrt));
                root = (int)surd.Root.Numerator;
            }
            return surd;
        }

        public static bool IsPerfectSquare(this Fraction fraction)
        {
            var sqrt = Math.Sqrt(fraction.Value);
            var sqrtString = sqrt.ToString();
            if (!sqrtString.Contains("."))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // If current number under the sqrt is a perfect square, return true. 
        // Else check it factors, if they are any which are perfect square, return true.
        // Else false.
        public static bool IsReduceable(this Surd surd)
        {
            // Do we need to know this?
            int count = 0;
            // surd should be in the form: sqrt (surd.Numerator)/1
            // So no harm.
            int number = (int)surd.Root.Numerator;
            var fac = number.GetFactors();

            // Not really necessary to remove 1 and the number itself, 
            // but I guess it saves two iterations and probably at least 2 CPU cycles. Who knows? Mesure? yes TODO!
            fac.Remove(1);
            fac.Remove(number);

            // Reducable if the root on it's own is a perfect square and it's not zero.
            if (surd.Root.IsPerfectSquare() && surd.Root != 1)
            {
                return true;
            }

            // Implicitly casting int to Fraction. It's totally okay guys 😂.
            foreach (Fraction item in fac)
            {
                if (item.IsPerfectSquare())
                {
                    ++count;
                }
            }

            if (count > 0)
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
