using System;
using MediatR;
using SFA.DAS.Notifications.Domain.Entities;

namespace SFA.DAS.Notifications.Application.Queries.GetMessage
{
    public class GetMessageQueryRequest : IAsyncRequest<GetMessageQueryResponse>
    {
        public NotificationFormat Format { get; set; }
        public string MessageId { get; set; }
    }
}
