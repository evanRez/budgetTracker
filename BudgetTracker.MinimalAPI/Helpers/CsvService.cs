using CsvHelper;
using ClassLib.Models.Transactions;
using System.Globalization;
using BudgetTracker.MinimalAPI.Helpers.Interfaces;

namespace BudgetTracker.MinimalAPI.Helpers;
public class CsvService : ICsvService
{
    public IEnumerable<T> ReadCSV<T>(Stream file)
    {
        var reader = new StreamReader(file);
        var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        
        csv.Context.RegisterClassMap<TransactionCsvMap>();

        var records = csv.GetRecords<T>();
        return records;
    }
}