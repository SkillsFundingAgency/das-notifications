using System;
using SFA.DAS.Notifications.Domain.Entities;

namespace SFA.DAS.Notifications.Application.Queries.GetMessage
{
    public class GetMessageQueryResponse
    {
        public string MessageType { get; set; }
        public string MessageId { get; set; }
        public NotificationContent Content { get; set; }
    }
}
