using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using static MathAPI.Helpers.PolynomialHelpers;

namespace MathAPI.MathTypes
{
    public readonly struct Polynomial : IEnumerable, IEnumerable<Term>, IEquatable<Polynomial>
    {
        public Polynomial (IEnumerable<Term> terms)
        {
            Terms = new Dictionary<int, Term>();
            Degree = terms.GetDegree();
            
            foreach (var term in terms)
            {
                if (!Terms.ContainsKey(term.Degree))
                {
                    Terms.Add(term.Degree, term);
                    continue;
                }
                Terms[term.Degree] = term.AddcompatibleTerms(term);
            }

            var nonRedundantTerms = Terms.Values as IEnumerable<Term>;
            var sortedNonRedundantTerms = from t in nonRedundantTerms
                                          orderby t.Degree descending
                                          where t.Coefficient != 0
                                          select t;
            var nonRedundantDic = sortedNonRedundantTerms.ToDictionary(term => term.Degree);
            Terms = nonRedundantDic;
        }

        public Polynomial(IDictionary<int, Term> terms) : this (terms.Values)
        { }

        public Polynomial(params Term[] terms) : this (terms.ToList())
        { }
        
        // Key: Degree, Value: Actual Term
        public IDictionary<int, Term> Terms { get; }
        public int Degree { get; }

        // Need to sort items!
        public Term this[int position] => 
            Terms.Values.ToList()[position];

        public IEnumerator<Term> GetEnumerator()
        {
            foreach (Term item in Terms.Values)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Equals(Polynomial other)
        {
            return Terms.SequenceEqual(other.Terms) &&
                Degree == other.Degree
                ? true : false;
        }

        public override bool Equals(object obj)
        {
            if (obj is Polynomial p)
            {
                return Equals(p);
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash = 17;

            foreach (Term item in Terms.Values)
            {
                hash <<= item.Degree;
                hash += (int)item.Coefficient.Numerator / 31;
                hash <<= (item.Degree.GetHashCode() + 19);
            }

            return hash;
        }

        public static bool operator ==(Polynomial term, Polynomial other)
        {
            return term.Equals(other);
        }

        public static bool operator !=(Polynomial term, Polynomial other)
        {
            return !(term == other);
        }

        public static Polynomial operator +(Polynomial polynomial, Polynomial other)
        {
            var newTerms = polynomial.Terms;
            foreach (var degree in other.Terms.Keys)
            {
                if (!polynomial.Terms.ContainsKey(degree))
                {
                    newTerms.Add(degree, other.Terms[degree]);
                }
                else
                {
                    var term = polynomial.Terms[degree];
                    var oTerm = other.Terms[degree];
                    polynomial.Terms[degree] = term.AddcompatibleTerms(oTerm);
                    newTerms = polynomial.Terms;
                }
            }

            return new Polynomial(newTerms);
        }

        public static Polynomial operator -(Polynomial polynomial, Polynomial other)
        {
            var newTerms = polynomial.Terms;
            foreach (var degree in other.Terms.Keys)
            {
                if (!polynomial.Terms.ContainsKey(degree))
                {
                    newTerms.Add(degree, -other.Terms[degree]);
                }
                else
                {
                    var term = polynomial.Terms[degree];
                    var oTerm = other.Terms[degree];
                    // Subtract.
                    polynomial.Terms[degree] = term.AddcompatibleTerms(-oTerm);
                    newTerms = polynomial.Terms;
                }
            }

            return new Polynomial(newTerms);
        }

        public static Polynomial operator -(Polynomial polynomial)
        {
            var newTerms = polynomial.Terms;

            foreach (var term in polynomial.Terms.Keys)
            {
                newTerms[term] = -polynomial.Terms[term];
            }

            return new Polynomial(newTerms);
        }

        public static Polynomial operator *(Polynomial polynomial, Polynomial other)
        {
            var terms = new List<Term>();

            foreach (var t1 in polynomial.Terms.Values)
            {
                foreach (var t2 in other.Terms.Values)
                {
                    var newTerm = t1 * t2;
                    terms.Add(newTerm);
                }
            }

            return new Polynomial(terms);
        }

        public static PolynomialFraction operator /(Polynomial polynomial, Polynomial other)
        {
            return new PolynomialFraction(polynomial, other);
        }

        public static implicit operator Polynomial(Term term)
        {
            return new Polynomial(term);
        }

        public static implicit operator Polynomial(int term)
        {
            return new Term(0, term);
        }

        private readonly struct Root
        {
            public HashSet<Fraction> Set { get; }
        }
    }
}
