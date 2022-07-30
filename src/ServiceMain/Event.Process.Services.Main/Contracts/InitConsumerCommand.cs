namespace Event.Process.Services.Main.Contracts;

public class InitConsumerCommand
{
    public string QueueName { get; set; }
    public int Size { get; set; }
    public int MessagesPerSecond { get; set; }
}
