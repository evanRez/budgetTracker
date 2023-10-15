using Microsoft.Playwright;

namespace BudgetTracker.UnitTests;
public class APIIntegrationTest : IClassFixture<APITestFixture>
{
    APITestFixture _fixture;
    public APIIntegrationTest(APITestFixture fixture)
    {
        _fixture = fixture;
    }
    // public async void API_Fetch_returns_json()
    // {
    //     var playWright = await Playwright.CreateAsync();
    //     var requestContext = await playWright.APIRequest.NewContextAsync(
    //         new APIRequestNewContextOptions()
    //         {
    //             BaseURL = "http://localhost:5103",
    //             IgnoreHTTPSErrors = true
    //         });

    //     var response = await requestContext.GetAsync("api/transactions");

    //     var data = await response.TextAsync();

    //     Assert.Equal("Hmmm, no transactions could be found here.", data);
    // }

    public async void API_Fetch_returns_json()
    {
       
        var response = await _fixture.Request.GetAsync("api/transactions");

        var data = await response.TextAsync();

        Assert.Equal("Hmmm, no transactions could be found here.", data);
    }
    
}
