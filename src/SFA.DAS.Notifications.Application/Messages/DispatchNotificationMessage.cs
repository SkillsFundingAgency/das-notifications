using System;
using SFA.DAS.Notifications.Domain.Entities;

namespace SFA.DAS.Notifications.Application.Messages
{
    public class DispatchNotificationMessage
    {
        public string MessageId { get; set; }
        public NotificationFormat Format { get; set; }
    }
}
