using MathAPI.MathTypes;
using System.Collections.Generic;

namespace MathAPI.Helpers
{
    public class PolynomialBuilder
    {
        public PolynomialBuilder()
        {
            Polynomial = 0;   
        }

        public PolynomialBuilder (Polynomial polynomial)
        {
            Polynomial = polynomial;
        }

        public PolynomialBuilder (Term term)
        {
            Polynomial = term;
        }

        public Polynomial Polynomial { get; private set; }

        public void Append (in Term term)
        {
            if (Polynomial.Terms.ContainsKey (term.Degree))
            {
                // Simple addition.
                Polynomial.Terms[term.Degree] = term.AddcompatibleTerms(Polynomial.Terms[term.Degree]);
            }
            else
            {
                // Add to the Terms dic.
                var dic = Polynomial.Terms as Dictionary<int, Term>;
                dic.Add(term.Degree, term);
                Polynomial = new Polynomial(dic);
            }
        }

        public void Append (in Polynomial polynomial)
        {
            foreach (var term in polynomial)
            {
                Append(term);
            }
        }
    }
}
