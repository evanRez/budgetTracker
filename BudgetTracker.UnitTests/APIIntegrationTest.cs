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
    [InlineData("TestData/SampleData.CSV")]
    public async Task API_ShouldLoadCSVDataAndReturnJson(string partialPath)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), partialPath);
        var file = new FilePayload()
        {
            Name = "f.js",
            MimeType = "text/javascript",
            Buffer = System.Text.Encoding.UTF8.GetBytes(filePath)
        };
        
        var multipart = _fixture!.Request!.CreateFormData();
        multipart.Set("fileField", file);
        //multipart.Set("fileField", filePath);
        var response = await _fixture!.Request!.FetchAsync("api/transactions/add-from-csv", new() { Method = "post", Multipart = multipart });

        var data = await response.JsonAsync();

        var hasValue = data.HasValue;
        Assert.True(hasValue);
    }
    
}
