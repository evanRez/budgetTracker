using BudgetTracker.MinimalAPI.DataAccess;
using BudgetTracker.MinimalAPI.RouteHandlers;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BudgetTrackerDb>();

var app = builder.Build();

app.MapTransactionEndpoints();

app.MapGet("/", () => "Hello World!");

app.Run();


            
