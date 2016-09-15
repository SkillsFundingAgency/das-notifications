using System;
using MediatR;

namespace SFA.DAS.Notifications.Application.Commands.DispatchNotification
{
    public class DispatchNotificationCommand : IAsyncRequest
    {
        public string MessageType { get; set; }
        public string MessageId { get; set; }
    }
}
