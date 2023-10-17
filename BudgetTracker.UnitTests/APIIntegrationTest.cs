using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Playwright;
using Microsoft.Playwright.Core;
using Xunit;

namespace BudgetTracker.UnitTests;
public class APIIntegrationTest : IClassFixture<APITestFixture>
{
    APITestFixture _fixture;
    public APIIntegrationTest(APITestFixture fixture)
    {
        _fixture = fixture;
    }
    [Fact]
    public async Task API_Fetch_returns_json()
    {
        if (_fixture is null)
        {
            Assert.True(false);
            return;
        }
        var response = await _fixture!.Request!.GetAsync("api/transactions");

        var data = await response.TextAsync();
       
        Assert.Equal("\"Hmmm, no transactions could be found here.\"", data);
    }

    [Theory]
    [InlineData("SampleData.CSV")]
    public async Task API_ShouldLoadCSVDataAndReturnJson(string partialPath)
    {
        //    var multipart = _fixture!.Request!.CreateFormData();
        //    multipart.Set("Transaction Date", "12/25/22");
        //    multipart.Set("Posted Date","12/27/22");
        //    multipart.Set("Description", "MOB PAYMENT RECEIVED");
        //    multipart.Set("Debit", "");
        //    multipart.Set("Credit", "1021.94");
        var file = new FilePayload()
        {
            Name = partialPath,
            MimeType = "text/csv",
            Buffer = Encoding.UTF8.GetBytes(File.ReadAllText("TestData/" + partialPath))
        };
        var multipart = _fixture!.Request!.CreateFormData();
        multipart.Set("fileField", file);
        var response = await _fixture!.Request!.PostAsync("api/transactions/add-from-csv",
         new() { Multipart = multipart });

        var status =  response.Status;

        Assert.Equal(200, status);
    }
    
}
