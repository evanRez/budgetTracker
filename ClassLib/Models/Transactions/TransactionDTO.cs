using CsvHelper.Configuration.Attributes;

namespace ClassLib.Models.Transactions;

public class TransactionDTO
{
    public int Id { get; set; }

    [Name("Transaction Date")]
    public DateOnly InitiatedDate { get; set; }

    [Name("Posted Date")]
    public DateOnly PostedDate { get; set; }

    [Name("Description")]
    public required string Description { get; set; }

    [Name("Debit")]
    public decimal? SpentAmount {get; set;}
    
    [Name("Credit")]
    public decimal? PaidBackAmount { get; set;}
}
