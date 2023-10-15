using ClassLib.Models.Transactions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BudgetTracker.MinimalAPI.DataAccess
{
    public sealed class BudgetTrackerDb : DbContext
    {
        public BudgetTrackerDb(DbContextOptions<BudgetTrackerDb> options) : base(options) { }
        public DbSet<TransactionDTO> Transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .Build();

            var connectionString = configuration["ConnectionStrings:Postgres"];

            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}
