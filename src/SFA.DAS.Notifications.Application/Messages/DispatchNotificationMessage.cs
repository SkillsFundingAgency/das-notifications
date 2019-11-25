using SFA.DAS.Notifications.Domain2.Entities;

namespace SFA.DAS.Notifications.Application.Messages
{
    public class DispatchNotificationMessage
    {
        public string MessageId { get; set; }
        public NotificationFormat Format { get; set; }
    }
}
