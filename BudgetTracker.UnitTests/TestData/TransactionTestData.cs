using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.UnitTests.TestData
{
    public class TransactionTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { 5000.1m, 20.11m, 474.1m, 2524.1m };
            yield return new object[] { 500m, 32m, 32m, 0.97m, 32.58m, 67.2m, 11.18m };
            yield return new object[] { 2850m, 44.70m, 2524m  };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
