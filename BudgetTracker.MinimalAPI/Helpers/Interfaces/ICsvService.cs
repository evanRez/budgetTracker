namespace BudgetTracker.MinimalAPI.Helpers.Interfaces;
public interface ICsvService
{
    public IEnumerable<T> ReadCSV<T>(Stream file);
}
