using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.OpenApi.Models;
using Microsoft.Playwright;
using System.Configuration;
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

        //      headers: { 'content-type': 'application/x-www-form-urlencoded'},
        //data: new URLSearchParams({
        //  grant_type: 'client_credentials',
        //  client_id: '{yourClientId}',
        //  client_secret: 'YOUR_CLIENT_SECRET',
        //  audience: '{yourApiIdentifier}'

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
            { "audience", configuration.GetSection("Auth0:Audience").Value.ToString() },
            //{ "access_token_url", configuration.GetSection("Auth0:TokenUrl").Value.ToString() },
            { "scope", "write:transaction" }
        };

        var requestContext = await Playwright.CreateAsync();

        var bearerContext = await requestContext.APIRequest.NewContextAsync(
            new APIRequestNewContextOptions()
            {
                BaseURL = "http://localhost:5103/",
                IgnoreHTTPSErrors = true,
            });
        var bearerPost = await bearerContext.PostAsync("/oauth/token", new APIRequestContextOptions()
        {
            DataObject = clientCredentials
        });

        var tokenBody = await bearerPost.JsonAsync();

        var token2 = tokenBody.GetValueOrDefault();
        //await requestContext.GetAsync("https://api.example.com/login");
        //// Save storage state into a variable.
        //var state = await requestContext.StorageStateAsync();

        //// Create a new context with the saved storage state.
        //var context = await Browser.NewContextAsync(new() { StorageState = state });



        var playwrightContext = await Playwright.CreateAsync();
        Request = await playwrightContext.APIRequest.NewContextAsync(
            new APIRequestNewContextOptions()
            {
                BaseURL = "http://localhost:5103",
                IgnoreHTTPSErrors = true,
                ExtraHTTPHeaders = clientCredentials
            });
    } 
    
}
