namespace Blackbaud.HeadlessDataSync.Services
{ 
    /// <summary>
    /// Stores app-wide configuration properties.
    /// </summary>
    public class AppSettings
    {
        public string AuthBaseUri { get; set; }
        public string AuthClientId { get; set; }
        public string AuthClientSecret { get; set; }
        public string SkyApiSubscriptionKey { get; set; }
        public string SkyApiBaseUri { get; set; }
    }
}
