using System;
using System.Collections;
using System.Collections.Generic;

namespace MathAPI.MathTypes
{
    public readonly struct PartialFraction : IEnumerable, IEnumerable<PolynomialFraction>
    {
        public IEnumerator<PolynomialFraction> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
