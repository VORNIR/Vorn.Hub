public class HubServerConfiguration
{
    public const string Section = "Vorn:Hub";
    public string Authority { get; init; }
    public string HubUri { get; init; } = "/hub";
    public bool UseMessagePack { get; init; } = false;
    public bool EnableDetailedErrors { get; init; } = false;
}
