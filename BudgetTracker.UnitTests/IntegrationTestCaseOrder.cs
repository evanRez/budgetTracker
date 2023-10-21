using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace BudgetTracker.UnitTests
{
    public class IntegrationTestCaseOrder : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(
       IEnumerable<TTestCase> testCases) where TTestCase : ITestCase =>
       testCases.OrderBy(testCase => testCase.TestMethod.Method.Name);
    }
}