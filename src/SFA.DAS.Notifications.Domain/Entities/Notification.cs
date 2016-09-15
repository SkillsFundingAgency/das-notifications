using System;

namespace SFA.DAS.Notifications.Domain.Entities
{
    public class Notification
    {
        public string MessageId { get; set; }
        public NotificationContent Content { get; set; }
    }
}
