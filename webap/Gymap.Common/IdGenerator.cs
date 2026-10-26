namespace gymap.Common;

public class IdGenerator : IIdGenerator
{
    public int GetNextId(IEnumerable<int> existingIds)
    {
        return existingIds.Any() ? existingIds.Max() + 1 : 1;
    }
}