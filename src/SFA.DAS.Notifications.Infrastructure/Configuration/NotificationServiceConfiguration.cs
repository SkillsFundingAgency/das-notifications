namespace SFA.DAS.Notifications.Infrastructure.Configuration
{
    public class NotificationServiceConfiguration
    {
        public string NotificationServiceApiKey { get; set; }
        public string DatabaseConnectionString { get; set; }
        public SmtpConfiguration SmtpConfiguration { get; set; }
        public NServiceBusConfiguration NServiceBusConfiguration { get; set; }
        public string EmailService { get; set; }
    }
}
