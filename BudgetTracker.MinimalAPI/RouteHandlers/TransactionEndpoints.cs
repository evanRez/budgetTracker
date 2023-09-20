using BudgetTracker.MinimalAPI.DataAccess;
using ClassLib;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace BudgetTracker.MinimalAPI.RouteHandlers
{
    public class TransactionEndpoints
    {
        private readonly BudgetTrackerDb _db;

        public TransactionEndpoints(BudgetTrackerDb db) 
        { 
            _db = db;
        }
        public void Map(WebApplication app)
        {
            app.MapGet("/transactions", async (BudgetTrackerDb db) =>
                await db.Transactions.ToListAsync());

            app.MapGet("/transactions/{id}", async (int id, BudgetTrackerDb db) =>
            await db.Transactions.FindAsync(id)
                is TransactionDTO transaction
                    ? Results.Ok(transaction)
                    : Results.NotFound());

            app.MapPost("/transactions", async (TransactionDTO transaction, BudgetTrackerDb db) =>
            {
                db.Transactions.Add(transaction);
                await db.SaveChangesAsync();

                return Results.Created($"/transactions/{transaction.Id}", transaction);
            });

            app.MapDelete("/transactions/{id}", async (int id, BudgetTrackerDb db) =>
            {
                if (await db.Transactions.FindAsync(id) is TransactionDTO todo)
                {
                    db.Transactions.Remove(todo);
                    await db.SaveChangesAsync();
                    return Results.NoContent();
                }

                return Results.NotFound();
            });
        }
    }
}
