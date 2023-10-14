using BudgetTracker.MinimalAPI.DataAccess;
using BudgetTracker.MinimalAPI.RouteHandlers;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.MinimalAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
           
            builder.Services.AddDbContext<BudgetTrackerDb>();

            var app = builder.Build();

            app.MapTransactionEndpoints();

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}