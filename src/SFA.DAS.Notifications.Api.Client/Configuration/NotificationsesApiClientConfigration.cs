namespace SFA.DAS.Notifications.Api.Client.Configuration
{
    public class NotificationsApiClientConfiguration : INotificationsApiClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
        public string Tenant { get; set; }
    }
}