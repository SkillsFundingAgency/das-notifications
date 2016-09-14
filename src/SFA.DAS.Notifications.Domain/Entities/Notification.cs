using System;

namespace SFA.DAS.Notifications.Domain.Entities
{
    public class Notification //todo: remove this type and use separate email / sms domain entities instead?
    {
        public string MessageType { get; set; }
        public string MessageId { get; set; }
        public NotificationContent Content { get; set; }
    }
}
