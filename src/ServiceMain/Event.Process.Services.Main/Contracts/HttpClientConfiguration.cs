namespace Event.Process.Services.Main.Contracts;

public class HttpClientConfiguration
{
    public string UrlBase { get; set; } 
    public string Route { get; set; }
    public string HttpMethod { get; set; }
}