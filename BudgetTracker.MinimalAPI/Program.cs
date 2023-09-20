using BudgetTracker.MinimalAPI.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BudgetTracker.MinimalAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration["ConnectionStrings:awsConnection"];

            builder.Services.AddDbContext<BudgetTrackerDb>(options => options.UseNpgsql(connectionString));

            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}