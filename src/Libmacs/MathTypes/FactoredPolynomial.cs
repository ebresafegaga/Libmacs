using System;
using System.Collections;
using System.Collections.Generic;

namespace MathAPI.MathTypes
{
    public readonly struct FactoredPolynomial : IEnumerable, IEnumerable<BinomialTerm>
    {
        public IEnumerator<BinomialTerm> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
