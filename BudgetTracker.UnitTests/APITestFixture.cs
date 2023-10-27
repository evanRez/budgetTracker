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

        var headers = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", configuration.GetSection("Auth0:Domain").Value.ToString() },
            { "client_secret", configuration.GetSection("Auth0:ClientSecret").Value.ToString() },
            { "audience", configuration.GetSection("Auth0:Audience").Value.ToString() },
            { "access_token_url", configuration.GetSection("Auth0:TokenUrl").Value.ToString() },
            { "scope", "write:transaction" }
        };



        var playwrightContext = await Playwright.CreateAsync();
        Request = await playwrightContext.APIRequest.NewContextAsync(
            new APIRequestNewContextOptions()
            {
                BaseURL = "http://localhost:5103",
                IgnoreHTTPSErrors = true,
                ExtraHTTPHeaders = headers
            });
    } 
    
}
