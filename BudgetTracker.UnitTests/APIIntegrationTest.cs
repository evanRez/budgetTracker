using Microsoft.Playwright;
using System.Text;
using System.Text.Json;

namespace BudgetTracker.UnitTests;
[TestCaseOrderer(
    ordererTypeName: "BudgetTracker.UnitTests.IntegrationTestCaseOrder",
    ordererAssemblyName: "BudgetTracker.UnitTests")]
public class APIIntegrationTest : IClassFixture<APITestFixture>
{
    APITestFixture _fixture;
    public APIIntegrationTest(APITestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Theory]
    [InlineData("SampleData.CSV")]
    public async Task A_API_ShouldLoadCSVDataAndReturnJson(string partialPath)
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
        var jsonData = await response.JsonAsync();
        
        var whatIsIt = jsonData.Value;
        JsonElement? trxn = null;
        foreach (var traderJoe in jsonData?.EnumerateArray())
        {
            if (traderJoe.TryGetProperty("description", out var descr) == true)
            {
                if (descr.GetString() == "TRADER JOE")
                {
                    trxn = traderJoe;
                }
            }
        }
        Assert.True(status);
        //Assert.NotNull(trxn);
        Assert.Equal("66.15", trxn?.GetProperty("spentAmount").ToString());
    }

    [Fact]
     public async Task B_API_Fetch_all_returns_json()
    {
        var response = await _fixture!.Request!.GetAsync("api/transactions");

        //var data = await response.TextAsync();

        var data = await response.JsonAsync();
       
        JsonElement? trxn = null;
        foreach (JsonElement traderJoe in data?.EnumerateArray())
        {
            if (traderJoe.TryGetProperty("description", out var descr) == true)
            {
                if (descr.GetString() == "TRADER JOE")
                {
                    trxn = traderJoe;
                }
            }
        }
        Assert.NotNull(trxn);
        Assert.Equal("66.15", trxn?.GetProperty("spentAmount").ToString());

        //Assert.Equal()
        //Assert.Equal("\"Hmmm, no transactions could be found here.\"", data);
    }



    [Theory]
    [InlineData("SampleData.CSV")]
    public async Task C_API_ShouldRemoveCSVDataAndReturnEnum(string partialPath)
    {
        var file = new FilePayload()
        {
            Name = partialPath,
            MimeType = "text/csv",
            Buffer = Encoding.UTF8.GetBytes(File.ReadAllText("TestData/" + partialPath))
        };
        var multipart = _fixture!.Request!.CreateFormData();
        multipart.Set("file", file);
        var response = await _fixture!.Request!.FetchAsync("api/transactions/delete-from-csv",
         new() { Method = "delete", Multipart = multipart });

        var status =  response.Ok;

        Assert.True(status);
    }
    
}
