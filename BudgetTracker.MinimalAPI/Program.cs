using BudgetTracker.MinimalAPI.DataAccess;
using BudgetTracker.MinimalAPI.RouteHandlers;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BudgetTrackerDb>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapTransactionEndpoints();

app.MapGet("/", () => "Hello World!");

app.Run();


            
