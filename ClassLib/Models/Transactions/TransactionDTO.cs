using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration.Attributes;

namespace ClassLib.Models.Transactions;

public class TransactionDTO
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Name("Transaction Date")]
    public DateOnly InitiatedDate { get; set; }

    [Required]
    [Name("Posted Date")]
    public DateOnly PostedDate { get; set; }

    [Required]
    [Name("Description")]
    public required string Description { get; set; }

    [Name("Debit")]
    public decimal? SpentAmount {get; set;}
    
    [Name("Credit")]
    public decimal? PaidBackAmount { get; set;}
}

public class TransactionComparer : IEqualityComparer<TransactionDTO>
{
    public bool Equals(TransactionDTO? x, TransactionDTO? y)
    {
        if (x == null || y == null) return false;
        return x.Description == y.Description 
        && x.InitiatedDate == y.InitiatedDate
        && x.PostedDate == y.PostedDate
        && x.SpentAmount == y.SpentAmount
        && x.PaidBackAmount == y.PaidBackAmount;
    }

    public int GetHashCode(TransactionDTO trx)
    {
        if (trx == null) return 0;
        return trx.Description.GetHashCode() 
        			^ trx.InitiatedDate.GetHashCode()
                    ^ trx.PostedDate.GetHashCode()
                    ^ trx.SpentAmount.GetHashCode()
                    ^ trx.PaidBackAmount.GetHashCode();
    }
}