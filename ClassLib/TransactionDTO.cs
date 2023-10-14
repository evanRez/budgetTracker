namespace ClassLib;

public class TransactionDTO
{
    public int Id { get; set; }
    public DateOnly InitiatedDate { get; set; }
    public DateOnly PostedDate { get; set; }
    public required string Description { get; set; }
    public decimal? SpentAmount {get; set;}
    public decimal? PaidBackAmount { get; set;}
}
