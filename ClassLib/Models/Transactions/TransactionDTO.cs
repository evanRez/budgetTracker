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
