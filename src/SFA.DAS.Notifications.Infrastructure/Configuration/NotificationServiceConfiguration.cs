namespace SFA.DAS.Notifications.Infrastructure.Configuration
{
    public class NotificationServiceConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public SmtpConfiguration SmtpConfiguration { get; set; }
        public NotifyServiceConfiguration NotifyServiceConfiguration { get; set; }
        public NServiceBusConfiguration NServiceBusConfiguration { get; set; }
        public string EmailService { get; set; }
    }
}
