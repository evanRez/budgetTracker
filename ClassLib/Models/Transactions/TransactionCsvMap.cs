using CsvHelper.Configuration;

namespace ClassLib.Models.Transactions;

public class TransactionCsvMap : ClassMap<TransactionDTO>
{
    public TransactionCsvMap()
    {
        Map(t => t.InitiatedDate).Name("Transaction Date");
        Map(t => t.PostedDate).Name("Posted Date");
        Map(t => t.Description).Name("Description");
        Map(t => t.SpentAmount).Name("Debit");
        Map(t => t.PaidBackAmount).Name("Credit");
    }
}
    
