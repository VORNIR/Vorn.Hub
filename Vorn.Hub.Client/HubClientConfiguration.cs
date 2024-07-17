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
    }
}