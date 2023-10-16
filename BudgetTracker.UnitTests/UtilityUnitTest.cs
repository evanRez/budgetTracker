using BudgetTracker.MinimalAPI.Helpers;
using BudgetTracker.UnitTests.TestData;
using ClassLib.Models.Transactions;
using Microsoft.Playwright;

namespace BudgetTracker.UnitTests;

public class UtilityUnitTest : IClassFixture<APITestFixture>
{
    [Theory]
    [InlineData("11/09/2022", "11/24/2022", "11/15/2022")]
    [InlineData("01/01/2022", "01/01/2023", "04/15/2022")]
    [InlineData("09/11/2023", "09/15/2023", "09/13/2023")]
    public void TransactionIsInSpecifiedDateRange(string startDateString, string endDateString, string dateString)
    {
        var utility = new Utility();

        var startDate = DateOnly.Parse(startDateString);
        var endDate = DateOnly.Parse(endDateString);
        var date = DateOnly.Parse(dateString);

        var result = utility.IsWithinDateRange(startDate, endDate, date);
        Assert.True(result);

    }
    [Theory]
    [InlineData("11/09/2022", "11/24/2022", "08/15/2022")]
    [InlineData("01/01/2022", "01/01/2023", "01/01/2024")]
    [InlineData("09/11/2023", "09/15/2023", "09/16/2023")]
    public void TransactionIsNOTinSpecifiedDateRange(string startDateString, string endDateString, string dateString)
    {
        var utility = new Utility();

        var startDate = DateOnly.Parse(startDateString);
        var endDate = DateOnly.Parse(endDateString);
        var date = DateOnly.Parse(dateString);

        var result = utility.IsWithinDateRange(startDate, endDate, date);
        Assert.False(result);

    }
    [Theory]
    [ClassData(typeof(TransactionTestData))]
    public void SpendingIsWithinBudget(params decimal[] transactions)
    {
        var utility = new Utility();

        var result = utility.IsWithinBudget(transactions.First(), transactions.Skip(1).ToArray());
        Assert.True(result);
    }
    [Theory]
    [InlineData("TestData/SampleData.CSV")]
    public void CSVWillParse(string csvLeaf)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), csvLeaf);
        using (FileStream fs = File.Open(filePath, FileMode.Open))
        {
            var csvService = new CsvService();
            var records = csvService.ReadCSV<TransactionDTO>(fs).ToList();
            Assert.True(records.Count() == 5);
            Assert.Equal("MOB PAYMENT RECEIVED", records[0].Description);
        }
    }
}