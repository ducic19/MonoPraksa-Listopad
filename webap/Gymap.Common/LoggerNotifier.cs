
using gymap.Common;

namespace Gymap.Common;

public class LoggerNotifier : ILoggerNotifier
{
    public void Notify(string message)
    {
        Console.WriteLine($"[LOG - TRANSIENT]: {message}");
    }
}