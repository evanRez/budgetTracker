using BudgetTracker.MinimalAPI.Auth;
using BudgetTracker.MinimalAPI.DataAccess;
using BudgetTracker.MinimalAPI.Helpers;
using BudgetTracker.MinimalAPI.Helpers.Interfaces;
using BudgetTracker.MinimalAPI.RouteHandlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BudgetTrackerDb>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICsvService, CsvService>();



var domain = $"https://{builder.Configuration["Auth0:Domain"]}/";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.Authority = domain;
    options.Audience = builder.Configuration["Auth0:Audience"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.NameIdentifier
    };
    
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("write:transaction", policy => policy.Requirements.Add(new
    HasScopeRequirement("write:transaction", domain)));
});

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHealthChecks(); Once the app grows, see how we can use  this
app.UseAuthentication();
app.UseAuthorization();
app.MapTransactionEndpoints();

app.MapGet("/", () => "Hello World!");

app.Run();


            
