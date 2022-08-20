using WatchDog;

namespace Event.Process.Services.One
{
    public class DsvLog : ILog
    {
        public void Log(string message)
        {
            WatchLogger.Log(message);
        }
    }
}
