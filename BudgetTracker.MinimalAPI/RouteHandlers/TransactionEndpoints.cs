using BudgetTracker.MinimalAPI.DataAccess;
using BudgetTracker.MinimalAPI.DataAccess.Interfaces;
using BudgetTracker.MinimalAPI.Helpers.Interfaces;
using ClassLib.Models.Transactions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BudgetTracker.MinimalAPI.RouteHandlers
{
    //TODO: Find a way to do this without adding auth0 user id to each request
    public static class TransactionEndpoints
    {
        public static void MapTransactionEndpoints(this IEndpointRouteBuilder app)
        {
            var trxs = app.MapGroup("api/transactions");
                //.RequireAuthorization("write:transaction");
            trxs.MapGet("", GetAllTransactions );
            trxs.MapGet("{id}", GetTransaction );
            trxs.MapPost("", AddTransaction );
            trxs.MapDelete("{id}", DeleteTransaction );
            trxs.MapPut("{id}", UpdateTransaction );
            trxs.MapPost("/add-from-csv", AddTransactionsCSV );
            trxs.MapDelete("/delete-from-csv", DeleteTransactionsCSV );
        }
        
        public static async Task<Results<Ok<List<TransactionDTO>>,NotFound<string>>> GetAllTransactions([FromServices] BudgetTrackerDb db, ClaimsPrincipal user, [FromServices] IUserService userService)
        {
            var userId = user.Claims.SingleOrDefault(x => x.Type == "auth0_user_id")?.Value;

            var userDto = await userService.FindOrCreateUser(user);

            var transactions =  await db.Transactions.Where(x => x.UserId == userId).ToListAsync();
            if (transactions.Any())
            {
                return TypedResults.Ok(transactions);
            }
            else 
            {
                return TypedResults.NotFound("Hmmm, no transactions could be found here.");
            }
        }

        public static async Task<Results<Ok<TransactionDTO>, NotFound<string>>> GetTransaction(BudgetTrackerDb db, int id, ClaimsPrincipal user)
        {
            var userId = user.Claims.SingleOrDefault(x => x.Type == "auth0_user_id")?.Value;
            return await db.Transactions.Where(x => x.UserId == userId && x.Id == id).FirstOrDefaultAsync()
                is TransactionDTO transaction
                    ? TypedResults.Ok(transaction)
                    : TypedResults.NotFound($"Could not find a transaction with Id: {id}");
        }

        public static async Task<Results<Created<TransactionDTO>,BadRequest>> AddTransaction(BudgetTrackerDb db, TransactionDTO dto, ClaimsPrincipal user)
        {
            var userId = user.Claims.SingleOrDefault(x => x.Type == "auth0_user_id")?.Value;
            dto.UserId = userId;

            if (await db.Transactions.FirstOrDefaultAsync(
                t => t.InitiatedDate == dto.InitiatedDate 
                && t.PostedDate == dto.PostedDate 
                && t.Description == dto.Description
                && t.SpentAmount == dto.SpentAmount
                && t.PaidBackAmount == dto.PaidBackAmount
                && t.UserId == userId) != null)
            {
                return TypedResults.BadRequest();
            }
            await db.Transactions.AddAsync(dto);
            await db.SaveChangesAsync();
            
            return TypedResults.Created($"api/transactions/{dto.Id}", dto);
        }

        public static async Task<Results<Ok<TransactionDTO>, NotFound<string>>> DeleteTransaction(BudgetTrackerDb db, int id, ClaimsPrincipal user)
        {
            var userId = user.Claims.SingleOrDefault(x => x.Type == "auth0_user_id")?.Value;
            if (await db.Transactions.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId) is TransactionDTO trnx)
            {
                db.Transactions.Remove(trnx);
                await db.SaveChangesAsync();
                return TypedResults.Ok(trnx);
            }

            return TypedResults.NotFound($"Could not delete Transaction {id}.");
        }

        public static async Task<Results<Ok<TransactionDTO>, NotFound<string>>> UpdateTransaction(BudgetTrackerDb db, TransactionDTO trnx, ClaimsPrincipal user)
        {
            var userId = user.Claims.SingleOrDefault(x => x.Type == "auth0_user_id")?.Value;
            var userEmail = user.Claims.SingleOrDefault(x => x.Type == "auth0_email")?.Value;
            trnx.UserId = userId;
            //db.Update(trnx);
            var rec = await db.Transactions.FirstOrDefaultAsync(x => x.Id == trnx.Id && x.UserId == userId);

            if (rec is null) return TypedResults.NotFound($"No transaction found for Id: {trnx.Id} associated with {userEmail}");

            db.Entry(rec).CurrentValues.SetValues(trnx);

            await db.SaveChangesAsync();

            return TypedResults.Ok(rec);
        }

        public static async Task<Results<Ok<List<TransactionDTO>>, Ok<string>,BadRequest<string>>> AddTransactionsCSV([FromForm] IFormFile file, [FromServices] BudgetTrackerDb db, [FromServices] ICsvService csvService, ClaimsPrincipal user)
        {
            var userId = user.Claims.SingleOrDefault(x => x.Type == "auth0_user_id")?.Value;
            try 
            {
                using var stream = file.OpenReadStream();
                var trxns = csvService.ReadCSV<TransactionDTO>(stream).ToList();
                //Manaually iterating over and setting the user Id to match the one coming from httpcontext
                trxns.ForEach(t => t.UserId = userId);
               
                var filteredTrxns = trxns
                    .Where( trx => !db.Transactions
                        .Any(t => t.InitiatedDate == trx.InitiatedDate
                            && t.PostedDate == trx.PostedDate
                            && t.Description == trx.Description
                            && t.SpentAmount == trx.SpentAmount
                            && t.PaidBackAmount == trx.PaidBackAmount
                            && t.UserId == trx.UserId)).ToList();

                if (!filteredTrxns.Any())
                {
                    return TypedResults.Ok("These records have already been added!");
                }
                await db.Transactions.AddRangeAsync(filteredTrxns);
                await db.SaveChangesAsync();

                var newRecordsWithId = db.Transactions.AsEnumerable()
                    .Where(x => filteredTrxns.Any(
                        nr => nr.Description == x.Description
                        && nr.PostedDate == x.PostedDate
                        && nr.InitiatedDate == x.InitiatedDate
                        && nr.SpentAmount == x.SpentAmount)
                        && x.UserId == userId)
                        .ToList();

                return TypedResults.Ok(newRecordsWithId);
            }
            catch (Exception ex)
            {
                var err = ex.Data["CsvHelper"];
                return TypedResults.BadRequest($"Could not parse the provided .csv file: {err}");
            }
        }

        public static async Task<Results<Ok<List<TransactionDTO>>,Ok<string>, BadRequest<string>>> DeleteTransactionsCSV([FromForm] IFormFile file, [FromServices] BudgetTrackerDb db, [FromServices] ICsvService csvService, ClaimsPrincipal user)
        {
            var userId = user.Claims.SingleOrDefault(x => x.Type == "auth0_user_id")?.Value;
            try 
            {
                using var stream = file.OpenReadStream();
                var trxns = csvService.ReadCSV<TransactionDTO>(stream).ToList();
                trxns.ForEach(t => t.UserId = userId);
                var trxComparer = new TransactionComparer();

                //TODO: Improve performance, convert to async
                var filteredTrxns = db.Transactions
                    .AsEnumerable()
                    .Intersect(trxns, trxComparer)
                    .ToList();

                if (!filteredTrxns.Any())
                {
                    return TypedResults.Ok("No records match, nothing to delete");
                }

                db.Transactions.RemoveRange(filteredTrxns);
                await db.SaveChangesAsync();
                return TypedResults.Ok(filteredTrxns);
            }
            catch (Exception ex)
            {
                var err = ex.Data["CsvHelper"];
                return TypedResults.BadRequest($"Could not parse the provided .csv file: {err}: {ex}");
            }
        }

    }
}
