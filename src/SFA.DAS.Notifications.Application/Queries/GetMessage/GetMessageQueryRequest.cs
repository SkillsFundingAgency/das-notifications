using System;
using MediatR;

namespace SFA.DAS.Notifications.Application.Queries.GetMessage
{
    public class GetMessageQueryRequest : IAsyncRequest<GetMessageQueryResponse>
    {
        public string MessageType { get; set; }
        public string MessageId { get; set; }
    }
}
