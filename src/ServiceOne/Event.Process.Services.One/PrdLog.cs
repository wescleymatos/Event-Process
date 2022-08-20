using WatchDog;

namespace Event.Process.Services.One
{
    public class PrdLog : ILog
    {
        public void Log(string message)
        {
            WatchLogger.Log("Mensagem de PRD");
        }
    }
}
