namespace WebApplication3.Common;

// 1. Singleton primjeri
public interface IIdGenerator
{
    int GetNextId(IEnumerable<int> existingIds);
}

public class IdGenerator : IIdGenerator
{
    public int GetNextId(IEnumerable<int> existingIds)
    {
        return existingIds.Any() ? existingIds.Max() + 1 : 1;
    }
}

// 2. Scoped primjeri
public interface IRequestCounter
{
    Guid RequestId { get; }
    void Increment();
    int GetCount();
}

public class RequestCounter : IRequestCounter
{
    public Guid RequestId { get; } = Guid.NewGuid();
    private int _count = 0;

    public void Increment() => _count++;
    public int GetCount() => _count;
}

// 3. Transient primjeri
public interface ILoggerNotifier
{
    void Notify(string message);
}

public class LoggerNotifier : ILoggerNotifier
{
    public void Notify(string message)
    {
        Console.WriteLine($"[LOG - TRANSIENT]: {message}");
    }
}