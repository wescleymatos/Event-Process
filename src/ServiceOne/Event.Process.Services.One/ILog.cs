namespace Event.Process.Services.One
{
    public interface ILog<T> 
    {
        void Log(string message, params object?[] args);
    }
}
