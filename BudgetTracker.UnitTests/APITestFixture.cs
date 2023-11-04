using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using System.Reflection;

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

        var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .AddUserSecrets(Assembly.GetExecutingAssembly())
               .Build();

        var clientCredentials = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", configuration.GetSection("Auth0:ClientIdM2M").Value.ToString() },
            { "client_secret", configuration.GetSection("Auth0:ClientSecretM2M").Value.ToString() },
            { "audience", configuration.GetSection("Auth0:Audience").Value.ToString() }
        };

        var requestContext = await Playwright.CreateAsync();

        var tokenUrl = $"https://{configuration.GetSection("Auth0:Domain").Value.ToString()}";

        var bearerContext = await requestContext.APIRequest.NewContextAsync(
            new APIRequestNewContextOptions()
            {
                BaseURL = tokenUrl,
                IgnoreHTTPSErrors = true,
            });


        var bearerPost = await bearerContext.PostAsync("/oauth/token", new APIRequestContextOptions()
        {
            DataObject = clientCredentials
        });

        var tokenBody = await bearerPost.JsonAsync();

        var accessToken = tokenBody.GetValueOrDefault().GetProperty("access_token").ToString();


        var playwrightContext = await Playwright.CreateAsync();
        Request = await playwrightContext.APIRequest.NewContextAsync(
            new APIRequestNewContextOptions()
            {
                BaseURL = "http://localhost:5103",
                IgnoreHTTPSErrors = true,
                ExtraHTTPHeaders = new Dictionary<string, string>
                {
                    {"Authorization", $"Bearer {accessToken}" }
                }
            });
    } 
    
}
