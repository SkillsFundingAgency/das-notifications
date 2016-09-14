using System;

namespace SFA.DAS.Notifications.Domain.Entities
{
    public class Notification //todo: remove this type and use email / sms entities instead?
    {
        public string MessageType { get; set; }
        public string MessageId { get; set; }
        public NotificationContent Content { get; set; }
    }
}