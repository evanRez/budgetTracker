using System.ComponentModel;

namespace ClassLib;

public class TransactionDTO
{
    public int Id { get; set; }
    public DateOnly InitiatedDate { get; set; }
    public DateOnly PostedDate { get; set; }
    public required String Description { get; set; }
    public Decimal? SpentAmount {get; set;}
    public Decimal? PaidBackAmount { get; set;}
}
