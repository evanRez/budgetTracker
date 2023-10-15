namespace Namespace;
public interface ICsvService
{
    public IEnumerable<T> ReadCSV<T>(Stream file);
}
