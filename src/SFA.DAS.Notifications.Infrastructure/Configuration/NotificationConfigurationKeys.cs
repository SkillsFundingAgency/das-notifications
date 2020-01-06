using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Notifications.Infrastructure.Configuration
{
    public static class NotificationConfigurationKeys
    {
        public const string Notifications = "SFA.DAS.Notifications";
        public static string NServiceBusConfiguration = $"{Notifications}:NServiceBusConfiguration";
        
        //Local SMTP Configuration for local email service.
        public static string SmtpConfiguration = $"{Notifications}:SmtpConfiguration";

        public const string NotificationsTemplates = "SFA.DAS.Notifications-Templates";
        
    }
}
