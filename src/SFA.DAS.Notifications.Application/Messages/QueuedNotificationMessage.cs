using System;

namespace SFA.DAS.Notifications.Application.Messages
{
    public class QueuedNotificationMessage
    {
        public string MessageType { get; set; }
        public string MessageId { get; set; }
    }
}
