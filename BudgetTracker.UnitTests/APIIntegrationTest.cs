using Microsoft.Playwright;

namespace BudgetTracker.UnitTests;
public class APIIntegrationTest
{
    public async void API_Fetch_returns_json()
    {
        var playWright = await Playwright.CreateAsync();
        // var requestContext = await playWright.APIRequest.NewContextAsync(
        //     new APIRequestNewContextOptions()
        //     {
        //         BaseUrl = "http://localhost:5103"
        //     });
    }
    
}
