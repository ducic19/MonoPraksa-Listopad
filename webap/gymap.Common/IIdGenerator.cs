namespace gymap.Common;

public interface IIdGenerator
{
    int GetNextId(IEnumerable<int> existingIds);
}
