namespace Event.Process.Services.Main.Model;

public class EventModel
{
    public Guid CorrelationId { get; set; }
    public object Payload { get; set; }
    public Metadata Metadata { get; set; }
}

public class Metadata
{
    public string Id { get; set; }
    public string Service { get; set; }
}

