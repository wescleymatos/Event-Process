namespace Event.Process.Services.One
{
    public class PrdLog<T> : ILog<T>
    {
        private readonly ILogger<T> _logger;

        public PrdLog(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void Log(string message, params object?[] args)
        {
            _logger.LogInformation(message, args);
        }
    }
}
