using System.Text;
using Microsoft.Playwright;

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
        var response = await _fixture!.Request!.GetAsync("api/transactions");

        var data = await response.TextAsync();
       
        Assert.Equal("\"Hmmm, no transactions could be found here.\"", data);
    }

    [Theory]
    [InlineData("SampleData.CSV")]
    public async Task API_ShouldLoadCSVDataAndReturnJson(string partialPath)
    {
        var file = new FilePayload()
        {
            Name = partialPath,
            MimeType = "text/csv",
            Buffer = Encoding.UTF8.GetBytes(File.ReadAllText("TestData/" + partialPath))
        };
        var multipart = _fixture!.Request!.CreateFormData();
        multipart.Set("file", file);
        var response = await _fixture!.Request!.FetchAsync("api/transactions/add-from-csv",
         new() { Method = "post", Multipart = multipart });

        var status =  response.Ok;
        var message = await response.TextAsync();

        Assert.True(status);
        Assert.Equal("\"These records have already been added!\"", message);
    }
    
}
