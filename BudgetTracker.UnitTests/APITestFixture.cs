using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;

namespace BudgetTracker.UnitTests;
public class APITestFixture : IAsyncLifetime
{
    public IAPIRequestContext? Request = null;

    public async Task DisposeAsync()
    {
        if (Request is not null)
        {
            await Request.DisposeAsync();
        }
    }

    public async Task InitializeAsync()
    {
        var playwrightContext = await Playwright.CreateAsync();
        Request = await playwrightContext.APIRequest.NewContextAsync(
            new APIRequestNewContextOptions()
            {
                BaseURL = "http://localhost:5103",
                IgnoreHTTPSErrors = true
            });
    } 
    
}
