using System;
using SFA.DAS.Notifications.Application.DataEntities;

namespace SFA.DAS.Notifications.Application.Queries.GetMessage
{
    public class GetMessageQueryResponse
    {
        public string MessageType { get; set; }
        public string MessageId { get; set; }
        public MessageContent Content { get; set; }
    }
}
