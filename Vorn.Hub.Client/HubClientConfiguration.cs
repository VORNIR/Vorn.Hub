namespace Vorn.Hub
{
    public class HubClientConfiguration
    {
        public const string Section = "Vorn:Hub";
        public string Authority { get; set; }
        public string Endpoint { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public bool UseMessagePack { get; set; } = false;
        public int ServerTimeoutInSeconds { get; set; } = 60;
        public int KeepAliveIntervalInSeconds { get; set; } = 10;
    }
}