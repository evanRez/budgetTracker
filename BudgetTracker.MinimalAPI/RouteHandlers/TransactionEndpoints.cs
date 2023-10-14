using BudgetTracker.MinimalAPI.DataAccess;
using ClassLib;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.MinimalAPI.RouteHandlers
{
    public static class TransactionEndpoints
    {
        public static void MapTransactionEndpoints(this IEndpointRouteBuilder app)
        {
            var trxs = app.MapGroup("api/transactions");
            trxs.MapGet("", GetAllTransactions);
        }
        
        public static async Task<Results<Ok<List<TransactionDTO>>,NotFound<string>>> GetAllTransactions(BudgetTrackerDb db)
        {
            var transactions =  await db.Transactions.ToListAsync();
            if (transactions.Any())
            {
                return TypedResults.Ok(transactions);
            }
            else 
            {
                return TypedResults.NotFound("Hmmm, no transactions could be found here.");
            }
        }

            // app.MapGet("/transactions/{id}", async (int id, BudgetTrackerDb db) =>
            // await db.Transactions.FindAsync(id)
            //     is TransactionDTO transaction
            //         ? Results.Ok(transaction)
            //         : Results.NotFound());

            // app.MapPost("/transactions", async (TransactionDTO transaction, BudgetTrackerDb db) =>
            // {
            //     db.Transactions.Add(transaction);
            //     await db.SaveChangesAsync();

            //     return Results.Created($"/transactions/{transaction.Id}", transaction);
            // });

            // app.MapDelete("/transactions/{id}", async (int id, BudgetTrackerDb db) =>
            // {
            //     if (await db.Transactions.FindAsync(id) is TransactionDTO todo)
            //     {
            //         db.Transactions.Remove(todo);
            //         await db.SaveChangesAsync();
            //         return Results.NoContent();
            //     }

            //     return Results.NotFound();
            // });
        //}
    }
}
