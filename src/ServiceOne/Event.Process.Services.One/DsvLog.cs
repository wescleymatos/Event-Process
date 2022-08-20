using WatchDog;

namespace Event.Process.Services.One
{
    public class DsvLog<T> : ILog<T>
    {
        public void Log(string message, params object?[] args)
        {
            string format = string.Format(message, args);
            WatchLogger.Log(format);
        }
    }
}
