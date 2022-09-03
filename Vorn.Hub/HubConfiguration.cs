namespace Vorn.Hub;
public class HubConfiguration
{
    public const string Section = "Haas";
    public string Authority { get; init; }
    public string Endpoint { get; init; }
    public string ClientId { get; init; }
    public string ClientSecret { get; init; }
}
