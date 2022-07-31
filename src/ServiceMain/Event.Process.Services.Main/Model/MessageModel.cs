namespace Event.Process.Services.Main.Model;

public class MessageModel
{
    public string MessageId { get; set; }
    public DateTimeOffset Stored { get; set; }
    public DateTimeOffset? Processed { get; set; }
    public TimeSpan? TimeSpent() => this.Processed?.Subtract(this.Stored) ?? null;
}
