using System;

namespace SFA.DAS.Notifications.Domain.Entities
{
    public class NotificationContent
    {
        public string UserId { get; set; }
        public DateTime Timestamp { get; set; }
        public NotificationFormat Format { get; set; }
        public string TemplateId { get; set; }
        public string Data { get; set; }
    }
}
