using BudgetTracker.MinimalAPI.Auth;
using BudgetTracker.MinimalAPI.DataAccess;
using BudgetTracker.MinimalAPI.DataAccess.Interfaces;
using BudgetTracker.MinimalAPI.DataAccess.Services;
using BudgetTracker.MinimalAPI.Helpers;
using BudgetTracker.MinimalAPI.Helpers.Interfaces;
using BudgetTracker.MinimalAPI.RouteHandlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BudgetTrackerDb>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICsvService, CsvService>();
//builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<IUserService, UserService>();

builder.WebHost.UseUrls( "http://0.0.0.0:5103", "https://0.0.0.0:7148");

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
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});



builder.Services.AddControllersWithViews()
                .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

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


            
