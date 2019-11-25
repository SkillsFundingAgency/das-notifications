using System;

namespace SFA.DAS.Notifications.Domain.Entities
{
    public class Notification
    {
        public string MessageId { get; set; }
        public string SystemId { get; set; }
        public DateTime Timestamp { get; set; }
        public NotificationFormat Format { get; set; }
        public NotificationStatus Status { get; set; }
        public string TemplateId { get; set; }
        public string Data { get; set; }
    }
}
