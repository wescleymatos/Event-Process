namespace Event.Process.Services.Main.Contracts;

public class ServicesMappingConfiguration
{
    public List<Dictionary<string, EventMap>> Services { get; set; }
}

public class EventMap
{
    public string EventName { get; set; }
    public string ClientName { get; set; }
}
