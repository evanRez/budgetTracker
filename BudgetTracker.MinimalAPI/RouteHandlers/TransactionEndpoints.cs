using BudgetTracker.MinimalAPI.DataAccess;
using BudgetTracker.MinimalAPI.Helpers;
using ClassLib.Models.Transactions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.MinimalAPI.RouteHandlers
{
    public static class TransactionEndpoints
    {
        public static void MapTransactionEndpoints(this IEndpointRouteBuilder app)
        {
            var trxs = app.MapGroup("api/transactions");
            trxs.MapGet("", GetAllTransactions );
            trxs.MapGet("{id}", GetTransaction );
            trxs.MapPost("", AddTransaction );
            trxs.MapDelete("{id}", DeleteTransaction );
            trxs.MapPut("{id}", UpdateTransaction );
            trxs.MapPost("/add-from-csv", AddTransactionsCSV );
            trxs.MapDelete("/dlete-from-csv", DeleteTransactionsCSV );
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

        public static async Task<Results<Ok<TransactionDTO>, NotFound<string>>> GetTransaction(BudgetTrackerDb db, int id)
        {
            return await db.Transactions.FindAsync(id)
                is TransactionDTO transaction
                    ? TypedResults.Ok(transaction)
                    : TypedResults.NotFound($"Could not find a transaction with Id: {id}");
        }

        public static async Task<Results<Created<TransactionDTO>,BadRequest>> AddTransaction(BudgetTrackerDb db, TransactionDTO dto)
        {
            if (await db.Transactions.FirstOrDefaultAsync(
                t => t.InitiatedDate == dto.InitiatedDate 
                && t.PostedDate == dto.PostedDate 
                && t.Description == dto.Description
                && t.SpentAmount == dto.SpentAmount
                && t.PaidBackAmount == dto.PaidBackAmount) != null)
            {
                return TypedResults.BadRequest();
            }
            await db.Transactions.AddAsync(dto);
            await db.SaveChangesAsync();
            
            return TypedResults.Created($"api/transactions/{dto.Id}", dto);
        }

        public static async Task<Results<Ok<TransactionDTO>, NotFound<string>>> DeleteTransaction(BudgetTrackerDb db, int id)
        {
            if (await db.Transactions.FindAsync(id) is TransactionDTO trnx)
            {
                db.Transactions.Remove(trnx);
                await db.SaveChangesAsync();
                // return Results.Ok(new TransactionDTO(trnx));
                return TypedResults.Ok(trnx);
            }

            return TypedResults.NotFound($"Could not delete Transaction {id}.");
        }

        public static async Task<Results<Ok<TransactionDTO>, NotFound<string>>> UpdateTransaction(BudgetTrackerDb db, TransactionDTO trnx)
        {
            var rec = await db.Transactions.FindAsync(trnx.Id);

            if (rec is null) return TypedResults.NotFound($"No transaction found for {trnx.Id}");

            rec.Description = trnx.Description;
            rec.PostedDate = trnx.PostedDate;
            rec.InitiatedDate = trnx.InitiatedDate;
            rec.PaidBackAmount = trnx.PaidBackAmount;

            await db.SaveChangesAsync();

            return TypedResults.Ok(rec);
        }

        public static async Task<Results<Ok<List<TransactionDTO>>, Ok<string>,BadRequest<string>>> AddTransactionsCSV([FromForm] IFormFile file, [FromServices] BudgetTrackerDb db, [FromServices] CsvService csvService )
        {
            try 
            {
                using var stream = file.OpenReadStream();
                var trxns = csvService.ReadCSV<TransactionDTO>(stream);
                var formattedTrxns = trxns.ToList();
                var filteredTrxns = formattedTrxns
                    .Where(trx => !db.Transactions
                        .Any(t => t.InitiatedDate == trx.InitiatedDate
                            && t.PostedDate == trx.PostedDate
                            && t.Description == trx.Description
                            && t.SpentAmount == trx.SpentAmount
                            && t.PaidBackAmount == trx.PaidBackAmount))
                    .ToList();
                if (!filteredTrxns.Any())
                {
                    return TypedResults.Ok("These records have already been added!");
                }
                await db.Transactions.AddRangeAsync(filteredTrxns);
                await db.SaveChangesAsync();
                return TypedResults.Ok(formattedTrxns);
            }
            catch (Exception ex)
            {
                //https://stackoverflow.com/questions/21609348/in-csvhelper-how-to-catch-a-conversion-error-and-know-what-field-and-what-row-it
                var err = ex.Data["CsvHelper"];
                return TypedResults.BadRequest($"Could not parse the provided .csv file: {err}");
            }
        }

        public static async Task<Results<Ok<List<TransactionDTO>>,Ok<string>, BadRequest<string>>> DeleteTransactionsCSV([FromServices] BudgetTrackerDb db, [FromServices] CsvService csvService,[FromForm] IFormFile file)
        {
            try 
            {
                using var stream = file.OpenReadStream();
                var trxns = csvService.ReadCSV<TransactionDTO>(stream);
                var formattedTrxns = trxns.ToList();
                var filteredTrxns = formattedTrxns
                    .Where(trx => db.Transactions
                        .Any(t => t.InitiatedDate == trx.InitiatedDate
                            && t.PostedDate == trx.PostedDate
                            && t.Description == trx.Description
                            && t.SpentAmount == trx.SpentAmount
                            && t.PaidBackAmount == trx.PaidBackAmount))
                    .ToList();
                if (!filteredTrxns.Any())
                {
                    return TypedResults.Ok("No records match, nothing to delete");
                }
                db.Transactions.RemoveRange(filteredTrxns);
                await db.SaveChangesAsync();
                return TypedResults.Ok(formattedTrxns);
            }
            catch (Exception ex)
            {
                var err = ex.Data["CsvHelper"];
                return TypedResults.BadRequest($"Could not parse the provided .csv file: {err}");
            }
        }

    }
}
