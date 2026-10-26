namespace gymap.Common;

public class RequestCounter : IRequestCounter
{
    public Guid RequestId { get; } = Guid.NewGuid();
    private int _count = 0;

    public void Increment() => _count++;
    public int GetCount() => _count;
}