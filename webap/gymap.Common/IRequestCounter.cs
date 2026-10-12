namespace gymap.Common;

public interface IRequestCounter
{
    Guid RequestId { get; }
    void Increment();
    int GetCount();
}